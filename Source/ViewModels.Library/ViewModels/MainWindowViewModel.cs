#region Apache License
//
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
//
#endregion

namespace BluePlumGit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using BluePlumGit.Entitys;
    using BluePlumGit.Enums;
    using BluePlumGit.Messaging.Windows;
    using BluePlumGit.Models;
    using GordiasClassLibrary.Collections;
    using GordiasClassLibrary.Headquarters;
    using Livet.Commands;
    using NGit;
    using NGit.Api;
    using NGit.Storage.File;
    using NGit.Util;
    using Sharpen;
    using Common.Library.Enums;
    using System.Text;
    using NGit.Transport;
    using BluePlumGit.Utility;

    public class MainWindowViewModel : TacticsViewModel<MainWindowViewModelProperty, MainWindowViewModelCommand>
    {
        /*コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ
         *  
         * を使用してください。
         */

        /*ViewModelからViewを操作したい場合は、
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信してください。
         */

        /*
         * UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         */

        /*
         * Modelからの変更通知などの各種イベントをそのままViewModelで購読する事はメモリリークの
         * 原因となりやすく推奨できません。ViewModelHelperの各静的メソッドの利用を検討してください。
         */

        /// <summary>
        /// モデル
        /// </summary>
        private MainWindowModel _model;

        /// <summary>
        /// 登録リポジトリリスト
        /// </summary>
        private CleanupObservableCollection<RepositoryEntity> repositorysCollection;

        /// <summary>
        /// カレントブランチリスト
        /// </summary>
        private CleanupObservableCollection<BranchEntity> branchCollection;

        /// <summary>
        /// カレントリポジトリ
        /// </summary>
        private Git git;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this._model = new MainWindowModel();

            this.repositorysCollection = new CleanupObservableCollection<RepositoryEntity>();
            this.branchCollection = new CleanupObservableCollection<BranchEntity>();
        }

        /// <summary>
        /// リポジトリコレクションビューのプロパティ
        /// </summary>
        public ICollectionView RepositoryCollectionView
        {
            get
            {
                return this.repositorysCollection.View;
            }
        }

        /// <summary>
        /// ブランチコレクションのプロパティ
        /// </summary>
        public ICollectionView BranchCollectionView
        {
            get
            {
                return this.branchCollection.View;
            }
        }

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
            this.RepositoryCollectionView.CurrentChanged += new EventHandler(RepositoryCollectionViewCurrentChangedHandler);
            this.BranchCollectionView.CurrentChanged += new EventHandler(BranchCollectionViewCurrentChangedHandler);
        }
        #endregion

        #region Loadedメソッド
        /// <summary>
        /// Loadedメソッド
        /// </summary>
        public void Loaded()
        {
            var result = _model.OpenDataBase();

            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    this.repositorysCollection.Add(item);
                }
                this.RepositoryCollectionView.MoveCurrentToPosition(0);

                this.git = this.GetCurrentRepository();
                this.UpdateBranchList();
            }
        }
        #endregion

        #region リポジトリの登録
        /// <summary>
        /// リポジトリの登録
        /// </summary>
        [Command]
        private void RepositoryRegistration()
        {
            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.INITIALIZE,
            });

            if (message.Response != null)
            {
                InitializeRepositoryEntity initializeRepositoryEntity = (InitializeRepositoryEntity)message.Response.Result;
                RepositoryEntity entity = initializeRepositoryEntity.Entity;

                string gitdirName = entity.Path + "/" + Constants.DOT_GIT;

                FilePath gitdir = new FilePath(gitdirName);

                if (initializeRepositoryEntity.Mode == InitializeRepositoryEnum.EntryOnly)
                {
                    // リポジトリの登録のみ
                    entity.ID = this._model.GetRepositoryCount() + 1;

                    // dbの登録
                    this._model.AddRepository(entity.ID, entity.Name, gitdir);

                    // リスト追加
                    this.repositorysCollection.Add(entity);
                    this.RepositoryCollectionView.MoveCurrentTo(entity);
                }
                else
                {
                    // リポジトリの初期化と登録
                    if (!gitdir.Exists())
                    {
                        gitdir = gitdir.GetCanonicalFile();

                        FileRepository db = new FileRepository(gitdir);

                        db.Create();

                        entity.ID = this._model.GetRepositoryCount() + 1;

                        // dbの登録
                        this._model.AddRepository(entity.ID, entity.Name, gitdir);

                        // リスト追加
                        this.repositorysCollection.Add(entity);
                        this.RepositoryCollectionView.MoveCurrentTo(entity);
                    }
                    else
                    {
                        MessageBox.Show(".gitディレクトリが、既に存在します。");
                    }
                }
            }
        }
        #endregion

        #region リポジトリの削除
        /// <summary>
        /// リポジトリの削除
        /// </summary>
        [Command]
        private void RepositoryRemove()
        {
        }
        #endregion

        #region 設定変更コマンド
        /// <summary>
        /// 設定変更コマンド
        /// </summary>
        [Command]
        private void Config()
        {
            RepositoryEntity entity = (RepositoryEntity)this.RepositoryCollectionView.CurrentItem;

            FilePath dir = new FilePath(entity.Path);
            FilePath dotGit = new FilePath(dir, Constants.DOT_GIT);

            FileRepositoryBuilder builder = new FileRepositoryBuilder();

            builder.SetWorkTree(dir);
            builder.FindGitDir(dir);
            builder.SetMustExist(true);

            FileRepository repository = builder.Build();

            StoredConfig config = repository.GetConfig();

            string name = config.GetString("user", null, "name");
        }
        #endregion

        /// <summary>
        /// ブランチコンボボックをリポジトリと同期する
        /// </summary>
        private void SyncCurrentBranch2Combobox()
        {
            string name = RepositotyUtility.GetCurrentBranch(this.git);

            foreach (BranchEntity entity in this.branchCollection)
            {
                if (name == entity.Name)
                {
                    this.BranchCollectionView.MoveCurrentTo(entity);
                    break;
                }
            }
        }

        /// <summary>
        /// カレントGitリポジトリの取得
        /// </summary>
        /// <returns>Gitクラス</returns>
        private Git GetCurrentRepository()
        {
            RepositoryEntity selectedRepository = (RepositoryEntity)this.RepositoryCollectionView.CurrentItem;

            FileRepository db = new FileRepository(selectedRepository.Path);

            Git git = new Git(db);

            return git;
        }

        /// <summary>
        /// ブランチリストの更新
        /// </summary>
        private void UpdateBranchList()
        {
            IList<Ref> list = this.git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();

            this.branchCollection.Clear();

            foreach (Ref branch in list)
            {
                BranchEntity be = new BranchEntity
                {
                    ID = 0,
                    Name = Path.GetFileName(branch.GetName()),
                    Path = branch.GetName(),
                };
                this.branchCollection.Add(be);
            }
            this.SyncCurrentBranch2Combobox();
        }

        #region ブランチの作成
        /// <summary>
        /// ブランチの作成
        /// </summary>
        [Command]
        private void CreateBranch()
        {
            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CREATE_BRANCH,
            });

            if (message.Response != null)
            {
                if ((WindowButtonEnum)message.Response.Button == WindowButtonEnum.OK)
                {
                    BranchEntity entity = (BranchEntity)message.Response.Result;

                    this.git.BranchCreate().SetName(entity.Name).Call();
                    this.UpdateBranchList();
                }
            }
        }
        #endregion

        #region ブランチの削除
        /// <summary>
        /// ブランチの削除
        /// </summary>
        [Command]
        private void RemoveBranch()
        {
            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.REMOVE_BRANCH,
                Parameter = this.git,
            });

            if (message.Response != null)
            {
                if ((WindowButtonEnum)message.Response.Button == WindowButtonEnum.DELETE)
                {
                    BranchEntity entity = (BranchEntity)message.Response.Result;

                    string[] names = { entity.Name };

                    try
                    {
                        this.git.BranchDelete().SetBranchNames(names).Call();
                        this.UpdateBranchList();
                    }
                    catch (NGit.Api.Errors.NotMergedException exception)
                    {
                        MessageBox.Show("注意：マージされていないブランチを削除することは制限されています。\n強制的に削除したい場合には、強制削除のチェックボックをオンにしてください。");
                    }
                }
            }
        }
        #endregion

        #region コミットコマンド
        /// <summary>
        /// コミットコマンド
        /// </summary>
        [Command]
        private void Commit()
        {
            
        }
        #endregion

        #region 複製コマンド
        /// <summary>
        /// 複製コマンド
        /// </summary>
        [Command]
        private void RemoteRepositoryClone()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == result)
            {
                string gitdirName = dialog.SelectedPath + "/" + Constants.DOT_GIT;

                FilePath gitdir = new FilePath(gitdirName);

                if (!gitdir.Exists())
                {
                    FilePath directory = gitdir.GetCanonicalFile();

                    CloneCommand command = Git.CloneRepository();

                    command.SetDirectory(directory);
                    command.SetURI("git@192.168.11.47:livetsample.git");
                    command.Call();
                }
                else
                {
                    MessageBox.Show(".gitディレクトリが、既に存在します。");
                }
            }
        }
        #endregion

        #region 公開鍵の作成
        /// <summary>
        /// 公開鍵の作成
        /// </summary>
        [Command]
        private void KeypairGeneration()
        {
            FilePath keyfile = new FilePath("RSAKey");

            if (keyfile.Exists())
            {
                this.OpenKeyDispWindow(keyfile);
                return;
            }

            int type = Tamir.SharpSsh.jsch.KeyPair.RSA;

            // Output file name
            String filename = "RSAKey";
            // Signature comment
            String comment = "";

            try
            {
                // Create a new JSch instance
                Tamir.SharpSsh.jsch.JSch jsch = new Tamir.SharpSsh.jsch.JSch();

                // Prompt the user for a passphrase for the private key file
                String passphrase = "";

                // Generate the new key pair
                Tamir.SharpSsh.jsch.KeyPair kpair = Tamir.SharpSsh.jsch.KeyPair.genKeyPair(jsch, type);
                // Set a passphrase
                kpair.setPassphrase(passphrase);
                // Write the private key to "filename"
                kpair.writePrivateKey(filename);
                // Write the public key to "filename.pub"
                kpair.writePublicKey(filename + ".pub", comment);
                // Print the key fingerprint
                // Console.WriteLine("Finger print: " + kpair.getFingerPrint());
                // Free resources
                kpair.dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            this.OpenKeyDispWindow(keyfile);
        }
        #endregion

        #region ウインドウクローズキャンセル処理
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        private void WindowCloseCancel()
        {
            // TODO:終了時保存保護処理
            Environment.Exit(0);
        }
        #endregion

        /// <summary>
        /// 鍵表示ウインドウのオープン
        /// </summary>
        /// <param name="keyfile"></param>
        private void OpenKeyDispWindow(FilePath keyfile)
        {
            StreamReader sr = new StreamReader("RSAKey", Encoding.UTF8);

            string text = sr.ReadToEnd();

            sr.Close();

            RSAKeyEntity key = new RSAKeyEntity
            {
                Text = text,
            };
            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.KEYDISP,
                Parameter = key,
            });
        }

        /// <summary>
        /// リポジトリコンボボックスのカレントアイテムが変更された時の処理
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントパラメーター</param>
        private void RepositoryCollectionViewCurrentChangedHandler(object sender, EventArgs e)
        {
            if(this.git == null)
            {
                this.git = GetCurrentRepository();
            }
            this.UpdateBranchList();
        }

        /// <summary>
        /// ブランチコンボボックスのカレントアイテムが変更された時の処理
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントパラメーター</param>
        private void BranchCollectionViewCurrentChangedHandler(object sender, EventArgs e)
        {
            BranchEntity entity = (BranchEntity)this.BranchCollectionView.CurrentItem;

            if (entity != null)
            {
                this.git.Checkout().SetName(entity.Name).Call();
            }
        }
    }

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class MainWindowViewModelProperty : TacticsProperty
    {
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class MainWindowViewModelCommand
    {
        /// <summary>
        /// リポジトリの登録
        /// </summary>
        public TacticsCommand RepositoryRegistration { get; set; }

        /// <summary>
        /// リポジトリの削除
        /// </summary>
        public TacticsCommand RepositoryRemove { get; set; }

        /// <summary>
        /// ブランチの作成
        /// </summary>
        public TacticsCommand CreateBranch { get; set; }

        /// <summary>
        /// ブランチの削除
        /// </summary>
        public TacticsCommand RemoveBranch { get; set; }

        /// <summary>
        /// ウインドウクローズキャンセルコマンド
        /// </summary>
        public TacticsCommand WindowCloseCancel { get; set; }

        /// <summary>
        /// 公開鍵作成
        /// </summary>
        public TacticsCommand KeypairGeneration { get; set; }

        /// <summary>
        /// リモートリポジトリのクローン
        /// </summary>
        public TacticsCommand RemoteRepositoryClone { get; set; }

        /// <summary>
        /// 設定変更
        /// </summary>
        public TacticsCommand Config { get; set; }

        /// <summary>
        /// コミット
        /// </summary>
        public TacticsCommand Commit { get; set; }
    }
    #endregion
}
