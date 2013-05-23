#region License
// <copyright>
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion

namespace GitlabTool.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using Commno.Library;
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Common.Library.Messaging.Windows;
    using Gitlab;
    using GitlabTool.Models;
    using Gordias.Library.Collections;
    using Gordias.Library.Headquarters;
    using log4net;
    using NGit;
    using Sharpen;
    using NGit.Storage.File;
    using NGit.Api.Errors;
    using NGit.Api;
    using NGit.Treewalk;
    using NGit.Revwalk;
    using System.Collections.Generic;
    using NGit.Diff;

    #region メインクラス
    /// <summary>
    /// メインウインドウビューモデル
    /// </summary>
    public class MainWindowViewModel : TacticsViewModel<MainWindowViewModelProperty, MainWindowViewModelCommand>
    {
        /// <summary>
        /// ログ
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string PRIVATE_FILE = "github_rsa";

        private static readonly string PUBLIC_FILE = "github_rsa.pub";

        /// <summary>
        /// モデル
        /// </summary>
        private MainWindowModel model;

        /// <summary>
        /// グローバルコンフィグ
        /// </summary>
        private GrobalConfigEntity globalConfig;

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private ConfigEntity config;

        /// <summary>
        /// リポジトリリスト
        /// </summary>
        private CleanupObservableCollection<RepositoryEntity> repositories;

        private string privateKeyText;

        private string publicKeyText;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            logger.Info("アプリケーション起動");

            this.model = new MainWindowModel();
        }

        #region WindowState 変更通知プロパティ

        private WindowState windowState;

        public WindowState WindowState
        {
            get { return this.windowState; }
            set
            {
                if (this.windowState != value)
                {
                    this.windowState = value;
                    this.IsMaximized = value == WindowState.Maximized;
                    this.CanNormalize = value == WindowState.Maximized;
                    this.CanMaximize = value == WindowState.Normal;
                    this.RaisePropertyChanged(() => WindowState);
                }
            }
        }
        #endregion
        
        #region IsMaximized 変更通知プロパティ

        private bool isMaximized;

        public bool IsMaximized
        {
            get { return this.isMaximized; }
            set
            {
                if (this.isMaximized != value)
                {
                    this.isMaximized = value;
                    this.RaisePropertyChanged(() => this.IsMaximized);
                }
            }
        }

        #endregion

        #region CanMaximize 変更通知プロパティ

        private bool canMaximize = true;

        public bool CanMaximize
        {
            get { return this.canMaximize; }
            set
            {
                if (this.canMaximize != value)
                {
                    this.canMaximize = value;
                    this.RaisePropertyChanged(() => this.CanMaximize);
                }
            }
        }

        #endregion

        #region CanMinimize 変更通知プロパティ

        private bool canMinimize = true;

        public bool CanMinimize
        {
            get { return this.canMinimize; }
            set
            {
                if (this.canMinimize != value)
                {
                    this.canMinimize = value;
                    this.RaisePropertyChanged(() => this.CanMinimize);
                }
            }
        }

        #endregion

        #region CanNormalize 変更通知プロパティ

        private bool canNormalize = false;

        public bool CanNormalize
        {
            get { return this.canNormalize; }
            set
            {
                if (this.canNormalize != value)
                {
                    this.canNormalize = value;
                    this.RaisePropertyChanged(() => this.CanNormalize);
                }
            }
        }

        #endregion

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
            this.repositories = new CleanupObservableCollection<RepositoryEntity>();
            this.Propertys.Repositories = this.repositories.View;

            foreach (var repo in this.config.Repository)
            {
                this.repositories.Add(repo);
            }

            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string privateKey = Path.Combine(userFolder, ".ssh", PRIVATE_FILE);
            string publicKey = Path.Combine(userFolder, ".ssh", PUBLIC_FILE);

            this.privateKeyText = this.ReadTextFile(privateKey);
            this.publicKeyText = this.ReadTextFile(publicKey);
        }
        #endregion

        /// <summary>
        /// テキストファイルの読み込み
        /// </summary>
        /// <param name="filename">ファイル名</param>
        /// <returns>テキスト</returns>
        private string ReadTextFile(string filename)
        {
            string text = string.Empty;

            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    text = sr.ReadToEnd();
                }
            }
            return text;
        }

        #region Loadedメソッド
        /// <summary>
        /// Loadedメソッド
        /// </summary>
        public void Loaded()
        {
            this.config = this.model.OpenConfig();

            DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);

            this.globalConfig = this.model.LoadGrobalConfig();

            if (this.globalConfig == null)
            {
                this.Config();
            }

            if (this.globalConfig != null && this.globalConfig.EMail != null)
            {
                this.Propertys.GravatarId = this.globalConfig.EMail;
            }
        }
        #endregion

        #region 設定変更
        /// <summary>
        /// 設定変更
        /// </summary>
        [Command]
        private void Config()
        {
            logger.Info("操作：設定変更");

            ConfigDialogEntity param = new ConfigDialogEntity
            {
                ServerUrl = this.config.ServerUrl,
                Password = this.config.Password,
                Accent = this.config.Accent,
                ApiVersion = this.config.ApiVersion,
            };

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CONFIG,
                Parameter = param,
            });

            if (message.Response != null)
            {
                ConfigDialogEntity entity = (ConfigDialogEntity)message.Response.Result;

                this.config.ServerUrl = entity.ServerUrl;
                this.config.Password = entity.Password;
                this.config.ApiVersion = entity.ApiVersion;
                
                // 画面テーマ更新
                if (this.config.Accent != entity.Accent)
                {
                    this.config.Accent = entity.Accent;
                    DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);
                }

                // 設定保存
                this.model.SaveConfig(this.config);
            }
        }
        #endregion

        #region 複製
        /// <summary>
        /// 複製
        /// </summary>
        [Command]
        private void RepositoryClone()
        {
            logger.Info("操作：複製");

            // キーファイルのチェック
            if (!this.CheckKeyFile())
            {
                MessageBox.Show("鍵ファイルが見つかりません。GitHub for Windowsをインストールしてください。");
                return;
            }
            if (this.globalConfig == null)
            {
                // .gitglobalが存在しない
                MessageBox.Show(".gitglobalファイルが存在しません。GitHub for Windowsをインストールしてください。");
                return;
            }
            object[] param = new object[]
            {
                this.config.ServerUrl,
                this.config.Password,
                this.globalConfig.EMail,
                this.config.ApiVersion == ApiVersionEnum.VERSION2 ? "v2" : "v3",
            };

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CLONE_REPOSITORY,
                Parameter = param
            });

            if (message.Response != null)
            {
                if ((WindowButtonEnum)message.Response.Button == WindowButtonEnum.OK)
                {
                    CloneEntity entity = (CloneEntity)message.Response.Result;
                    string gitdirName = Path.Combine(entity.Path, Constants.DOT_GIT);
                    FilePath gitdir = new FilePath(gitdirName);

                    if (!gitdir.Exists())
                    {
                        BusyIndicatorProgressMonitor monitor = new BusyIndicatorProgressMonitor();

                        monitor.StartAction = () =>
                        {
                            this.OpenBusyIndicator("リポジトリの複製中です。");
                        };
                        monitor.UpdateAction = (string taskName, int cmp, int totalWork, int pcnt) =>
                        {
                            this.UpdateBusyIndicator(taskName, cmp, totalWork, pcnt);
                        };
                        monitor.CompleteAction = () =>
                        {
                            this.CloseBusyIndicator();

                            RepositoryEntity repository = new RepositoryEntity
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                Name = entity.Name,
                                Path = entity.Url,
                                Location = entity.Path,
                            };

                            this.repositories.Add(repository);
                            this.config.Repository.Add(repository);
                        
                            // 設定保存
                            this.model.SaveConfig(this.config);
                        };

                        this.model.CloneRepository(entity, this.privateKeyText, this.publicKeyText, monitor);
                    }
                    else
                    {
                        MessageBox.Show(".gitディレクトリが、既に存在します。");
                    }
                }
            }
        }
        #endregion

        #region 空フォルダの登録
        /// <summary>
        /// 空フォルダの登録
        /// </summary>
        [Command]
        private void EmptyFolderKeep()
        {
            logger.Info("操作：空フォルダの登録");

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.MANAGED_EMPTY_FOLDER,
                Parameter = this.Propertys.Repositories.CurrentItem,
            });
        }
        #endregion

        #region 排他ファイルの作成
        /// <summary>
        /// 排他ファイルの作成
        /// </summary>
        [Command]
        private void GitIgnore()
        {
            logger.Info("操作：排他ファイルの作成");

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CREATE_GITIGNORE,
                Parameter = this.Propertys.Repositories.CurrentItem,
            });
        }
        #endregion

        #region ブランチの削除
        /// <summary>
        /// ブランチの削除
        /// </summary>
        [Command]
        private void BranchRemove()
        {
            logger.Info("操作：ブランチの削除");

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.REMOVE_BRANCH,
                Parameter = this.Propertys.Repositories.CurrentItem,
            });
        }
        #endregion

        #region コミット
        /// <summary>
        /// コミット
        /// </summary>
        [Command]
        private void Commit()
        {
            logger.Info("操作：コミット");

            WindowOpenMessage massage = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.COMMIT,
                Parameter = this.Propertys.Repositories.CurrentItem,
            });
        }
        #endregion

        #region リポジトリ登録
        /// <summary>
        /// リポジトリ登録
        /// </summary>
        [Command]
        private void RepositoryEntory()
        {
            logger.Info("操作：リポジトリ登録");

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.ENTORY_REPOSITORY,
                Parameter = this.Propertys.Repositories.CurrentItem,
            });

            if (message.Response != null)
            {
                if ((WindowButtonEnum)message.Response.Button == WindowButtonEnum.OK)
                {
                    RepositoryEntity entity = (RepositoryEntity)message.Response.Result;
                    RepositoryEntity repository = new RepositoryEntity
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Name = entity.Name,
                        Path = entity.Path,
                        Location = entity.Location,
                    };

                    this.repositories.Add(repository);
                    this.config.Repository.Add(repository);

                    // 設定保存
                    this.model.SaveConfig(this.config);
                }
            }
        }
        #endregion

        #region リポジトリ削除
        /// <summary>
        /// リポジトリ削除
        /// </summary>
        [Command]
        private void RepositoryRemove()
        {
            logger.Info("操作：リポジトリ削除");

            // 削除確認
            MessageBoxResult result = MessageBox.Show("削除実行してもよろしいですか？", "リポジトリ削除", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.OK)
            {
                this.config.Repository.Remove((RepositoryEntity)this.Propertys.Repositories.CurrentItem);
                this.repositories.Remove((RepositoryEntity)this.Propertys.Repositories.CurrentItem);
                this.model.SaveConfig(this.config);
            }
        }
        #endregion

        #region 最適化
        /// <summary>
        /// 最適化
        /// </summary>
        [Command]
        private void Optimization()
        {
            logger.Info("操作：最適化");

            if (this.Propertys.Repositories.CurrentItem != null)
            {
                RepositoryEntity entity = (RepositoryEntity)this.Propertys.Repositories.CurrentItem;

                FilePath path = new FilePath(entity.Location, @".git");
                FileRepository db = new FileRepository(path);

                NGit.Storage.File.GC gc = new NGit.Storage.File.GC(db);
                NGit.Storage.File.GC.RepoStatistics statistics = gc.GetStatistics();

                // ばらばらなオブジェクトの数
                logger.Debug("numberOfLooseObjects = " + statistics.numberOfLooseObjects);
                // ？
                logger.Debug("numberOfLooseRefs = " + statistics.numberOfLooseRefs);
                // パックされたオブジェクトの数
                logger.Debug("numberOfPackedObjects = " + statistics.numberOfPackedObjects);
                // ゴミファイル
                logger.Debug("numberOfPackedRefs = " + statistics.numberOfPackedRefs);
                // パックの数
                logger.Debug("numberOfPackFiles = " + statistics.numberOfPackFiles);
                // ばらばらなオブジェクトの使用するディスク容量
                logger.Debug("sizeOfLooseObjects = " + statistics.sizeOfLooseObjects);
                // パックされたオブジェクトの使用するディスク容量
                logger.Debug("sizeOfPackedObjects = " + statistics.sizeOfPackedObjects);

                string message;

                message = "リポジトリの最適化を行いますか？";

                message += "\n\nばらばらなオブジェクトの数 = " + statistics.numberOfLooseObjects;
                message += "\nパックされたオブジェクトの数 = " + statistics.numberOfPackedObjects;
                message += "\nゴミファイル = " + statistics.numberOfPackedRefs;
                message += "\nパックの数 = " + statistics.numberOfPackFiles;
                message += "\nばらばらなオブジェクトの使用するディスク容量 = " + statistics.sizeOfLooseObjects;
                message += "\nパックされたオブジェクトの使用するディスク容量 = " + statistics.sizeOfPackedObjects;

                MessageBoxResult result = MessageBox.Show(message, string.Empty, MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    BusyIndicatorProgressMonitor monitor = new BusyIndicatorProgressMonitor();

                    monitor.StartAction = () =>
                    {
                        this.OpenBusyIndicator("リポジトリ圧縮中");
                    };
                    monitor.UpdateAction = (string taskName, int cmp, int totalWork, int pcnt) =>
                    {
                        this.UpdateBusyIndicator(taskName, cmp, totalWork, pcnt);
                    };
                    monitor.CompleteAction = () =>
                    {
                        this.CloseBusyIndicator();
                    };
                    gc.SetProgressMonitor(monitor);

                    BackgroundWorker bw = new BackgroundWorker();

                    bw.DoWork += (s, evt) =>
                    {
                        monitor.StartAction();

                        try
                        {
                            gc.Repack();
                        }
                        catch (JGitInternalException)
                        {
                            // TODO:
                        }
                    };
                    bw.RunWorkerCompleted += (s, evt) =>
                    {
                        monitor.CompleteAction();
                    };
                    bw.RunWorkerAsync();
                }
            }
        }
        #endregion

        #region 同期
        /// <summary>
        /// 同期
        /// </summary>
        [Command]
        private void Fetch()
        {
            logger.Info("操作：同期");

            // キーファイルのチェック
            if (!this.CheckKeyFile())
            {
                MessageBox.Show("鍵ファイルが見つかりません。GitHub for Windowsをインストールしてください。");
                return;
            }
            if (this.globalConfig == null)
            {
                // .gitglobalが存在しない
                MessageBox.Show(".gitglobalファイルが存在しません。GitHub for Windowsをインストールしてください。");
                return;
            }
            if (this.Propertys.Repositories.CurrentItem == null)
            {
                return;
            }
            RepositoryEntity repository = (RepositoryEntity)this.Propertys.Repositories.CurrentItem;

            FilePath path = new FilePath(repository.Location, @".git");
            FileRepository db = new FileRepository(path);
            Git git = new Git(db);

            CloneEntity entity = new CloneEntity
            {
                UserName = this.globalConfig.EMail,
                PassWord = this.config.Password,
            };

            BusyIndicatorProgressMonitor monitor = new BusyIndicatorProgressMonitor();

            monitor.StartAction = () =>
            {
                this.OpenBusyIndicator("リポジトリの同期中です。");
            };
            monitor.UpdateAction = (string taskName, int cmp, int totalWork, int pcnt) =>
            {
                this.UpdateBusyIndicator(taskName, cmp, totalWork, pcnt);
            };
            monitor.CompleteAction = () =>
            {
                DiffCommand diff = git.Diff();

                diff.SetOldTree(GetTreeIterator(git, "HEAD"));
                diff.SetNewTree(GetTreeIterator(git, "FETCH_HEAD"));

                IList<DiffEntry> list = diff.Call();

                if (list.Count > 0)
                {
                    MessageBox.Show("リモートリポジトリとローカルリポジトリに違いがあります。");
                }
                else
                {
                    MessageBox.Show("ローカルリポジトリは、最新の状態です。");
                }
                this.CloseBusyIndicator();

            };
            this.model.Fetch(git, entity, this.privateKeyText, this.publicKeyText, monitor);
        }

        /// <summary>
        /// 特定ブランチのツリーを取得する
        /// </summary>
        /// <param name="git">Git</param>
        /// <param name="name">ブランチ名</param>
        /// <returns></returns>
        private AbstractTreeIterator GetTreeIterator(Git git, string name)
        {
            Repository db = git.GetRepository();
            ObjectId id = db.Resolve(name);

            if (id == null)
            {
                throw new ArgumentException(name);
            }
            CanonicalTreeParser p = new CanonicalTreeParser();
            ObjectReader or = db.NewObjectReader();
            try
            {
                p.Reset(or, new RevWalk(db).ParseTree(id));
                return p;
            }
            finally
            {
                or.Release();
            }
        }
        #endregion

        #region 公開鍵
        /// <summary>
        /// 公開鍵
        /// </summary>
        [Command]
        private void PublicKey()
        {
            logger.Info("操作：公開鍵");

            // キーファイルのチェック
            if (!this.CheckKeyFile())
            {
                MessageBox.Show("鍵ファイルが見つかりません。GitHub for Windowsをインストールしてください。");
                return;
            }
            if (this.globalConfig == null)
            {
                // .gitglobalが存在しない
                MessageBox.Show(".gitglobalファイルが存在しません。GitHub for Windowsをインストールしてください。");
                return;
            }
            MessageBox.Show("公開鍵をクリップボードに格納します。サーバーへ登録してください。");

            Clipboard.SetText(this.publicKeyText);
        }
        #endregion

        #region ブランチ切り替え
        /// <summary>
        /// ブランチ切り替え
        /// </summary>
        [Command]
        private void BranchSelect()
        {
            logger.Info("操作：ブランチ切り替え");
        }
        #endregion

        #region ウインドウクローズキャンセル処理
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        private void WindowCloseCancel()
        {
            logger.Info("操作：ウインドウクローズ");

            Environment.Exit(0);
        }
        #endregion

        /// <summary>
        /// インジケーターを表示する
        /// </summary>
        /// <param name="title">タイトル</param>
        private void OpenBusyIndicator(string title)
        {
            this.Propertys.BusyDialogMessageTitle = title;
            this.Propertys.IsBusyDialog = true;
        }

        /// <summary>
        /// インジケーターの更新
        /// </summary>
        /// <param name="taskName">処理名称</param>
        /// <param name="cmp">完了数</param>
        /// <param name="totalWork">全タスク数</param>
        /// <param name="pcnt">パーセント</param>
        private void UpdateBusyIndicator(string taskName, int cmp, int totalWork, int pcnt)
        {
            this.Propertys.BusyDialogPcent = (double)pcnt;
            this.Propertys.BusyDialogProgressMessage = string.Format("{0} {1}/{2} {3}%", taskName, cmp, totalWork, pcnt);
        }

        /// <summary>
        /// インジケーターを閉じる
        /// </summary>
        private void CloseBusyIndicator()
        {
            this.Propertys.IsBusyDialog = false;
        }

        /// <summary>
        /// キーファイルのチェック
        /// </summary>
        private bool CheckKeyFile()
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string privateKey = Path.Combine(userFolder, ".ssh", PRIVATE_FILE);
            string publicKey = Path.Combine(userFolder, ".ssh", PUBLIC_FILE);

            if (!File.Exists(privateKey))
            {
                return false;
            }
            if (!File.Exists(publicKey))
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class MainWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// IsBusyDialog
        /// </summary>
        public virtual bool IsBusyDialog { get; set; }

        /// <summary>
        /// BusyDialogMessageTitle
        /// </summary>
        public virtual string BusyDialogMessageTitle { get; set; }

        /// <summary>
        /// BusyDialogProgressMessage
        /// </summary>
        public virtual string BusyDialogProgressMessage { get; set; }

        /// <summary>
        /// BusyDialogPcent
        /// </summary>
        public virtual double BusyDialogPcent { get; set; }

        /// <summary>
        /// Gravatarアイコン表示用ID
        /// </summary>
        public virtual string GravatarId { get; set; }

        /// <summary>
        /// プロジェクトリスト
        /// </summary>
        public virtual ICollectionView Repositories { get; set; }

        /// <summary>
        /// リポジトリ名
        /// </summary>
        public virtual string RepositoryName { get; set; }
        
        /// <summary>
        /// リポジトリのクローンされた場所
        /// </summary>
        public virtual string RepositoryLocation { get; set; }
        
        /// <summary>
        /// リモートリポジトリのアドレス
        /// </summary>
        public virtual string RepositoryPath { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class MainWindowViewModelCommand
    {
        /// <summary>
        /// ウインドウクローズキャンセルコマンド
        /// </summary>
        public TacticsCommand WindowCloseCancel { get; private set; }

        /// <summary>
        /// 設定変更
        /// </summary>
        public TacticsCommand Config { get; private set; }

        /// <summary>
        /// リポジトリのクローン
        /// </summary>
        public TacticsCommand RepositoryClone { get; private set; }

        /// <summary>
        /// 空フォルダをGitで管理させる
        /// </summary>
        public TacticsCommand EmptyFolderKeep { get; private set; }

        /// <summary>
        /// 排他ファイルの作成
        /// </summary>
        public TacticsCommand GitIgnore { get; private set; }

        /// <summary>
        /// ブランチの削除
        /// </summary>
        public TacticsCommand BranchRemove { get; private set; }

        /// <summary>
        /// ブランチの切り替え
        /// </summary>
        public TacticsCommand BranchSelect { get; private set; }

        /// <summary>
        /// コミット
        /// </summary>
        public TacticsCommand Commit { get; private set; }

        /// <summary>
        /// リポジトリ登録
        /// </summary>
        public TacticsCommand RepositoryEntory { get; private set; }

        /// <summary>
        /// リポジトリ削除
        /// </summary>
        public TacticsCommand RepositoryRemove { get; private set; }

        /// <summary>
        /// 最適化
        /// </summary>
        public TacticsCommand Optimization { get; private set; }

        /// <summary>
        /// 同期
        /// </summary>
        public TacticsCommand Fetch { get; private set; }

        /// <summary>
        /// 公開鍵をクリップボードにコピー
        /// </summary>
        public TacticsCommand PublicKey { get; private set; }
    }
    #endregion
}
