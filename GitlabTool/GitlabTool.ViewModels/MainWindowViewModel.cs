#region Apache License
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
#endregion

namespace GitlabTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using Commno.Library;
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Common.Library.Messaging.Windows;
    using Gitlab;
    using GitlabTool;
    using GitlabTool.Models;
    using Gordias.Library.Collections;
    using Gordias.Library.Headquarters;
    using log4net;
    using NGit;
    using Sharpen;

    #region メインクラス
    /// <summary>
    /// メインウインドウビューモデル
    /// </summary>
    public class MainWindowViewModel : TacticsViewModel<MainWindowViewModelProperty, MainWindowViewModelCommand>
    {
        /// <summary>
        /// ログ
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        }
        #endregion

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

        #region 設定変更コマンド
        /// <summary>
        /// 設定変更コマンド
        /// </summary>
        [Command]
        private void Config()
        {
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

        #region 複製コマンド
        /// <summary>
        /// 複製コマンド
        /// </summary>
        [Command]
        private void RepositoryClone()
        {
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

                        this.model.CloneRepository(entity, monitor);
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
        }
        #endregion

        #region リポジトリ削除
        /// <summary>
        /// リポジトリ削除
        /// </summary>
        [Command]
        private void RepositoryRemove()
        {
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

        #region ウインドウクローズキャンセル処理
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        private void WindowCloseCancel()
        {
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
        /// コミット
        /// </summary>
        public TacticsCommand Commit { get; private set; }

        /// <summary>
        /// リポジトリ削除
        /// </summary>
        public TacticsCommand RepositoryRemove { get; private set; }
    }
    #endregion
}
