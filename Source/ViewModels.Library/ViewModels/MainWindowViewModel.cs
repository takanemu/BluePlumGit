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

        //protected internal FileRepository db;
        //private readonly IList<Repository> toClose = new AList<Repository>();

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

        /// <summary>Creates a new empty repository within a new empty working directory.</summary>
        /// <remarks>Creates a new empty repository within a new empty working directory.</remarks>
        /// <returns>the newly created repository, opened for access</returns>
        /// <exception cref="System.IO.IOException">the repository could not be created in the temporary area
        /// 	</exception>
        //protected internal virtual FileRepository CreateWorkRepository()
        //{
        //    return CreateRepository(false);
        //}

        /// <summary>Creates a new empty repository.</summary>
        /// <remarks>Creates a new empty repository.</remarks>
        /// <param name="bare">
        /// true to create a bare repository; false to make a repository
        /// within its working directory
        /// </param>
        /// <returns>the newly created repository, opened for access</returns>
        /// <exception cref="System.IO.IOException">the repository could not be created in the temporary area
        /// 	</exception>
        //private FileRepository CreateRepository(bool bare)
        //{
        //    FilePath gitdir = CreateUniqueTestGitDir(bare);
        //    FileRepository db = new FileRepository(gitdir);
        //    //NUnit.Framework.Assert.IsFalse(gitdir.Exists());
        //    db.Create();
        //    //toClose.AddItem(db);

        //    return db;
        //}

        //protected internal virtual FilePath CreateUniqueTestGitDir(bool bare)
        //{
        //    string gitdirName = CreateUniqueTestFolderPrefix();
        //    if (!bare)
        //    {
        //        gitdirName += "/";
        //    }
        //    gitdirName += Constants.DOT_GIT;
        //    FilePath gitdir = new FilePath(trash, gitdirName);
        //    return gitdir.GetCanonicalFile();
        //}

        #region Initialize
        /// <summary>
        /// Initializeコマンド
        /// </summary>
        public void Initialize()
        {
            this.RepositoryCollectionView.CurrentChanged += new EventHandler(RepositoryCollectionViewCurrentChangedHandler);
            this.BranchCollectionView.CurrentChanged += new EventHandler(BranchCollectionViewCurrentChangedHandler);
        }

        #endregion

        #region Loaded
        /// <summary>
        /// Loadedコマンド
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

                RepositoryEntity selectedRepository = (RepositoryEntity)this.RepositoryCollectionView.CurrentItem;

                FileRepository db = new FileRepository(selectedRepository.Path);

                this.git = new Git(db);

                IList<Ref> list = git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();

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
        }
        #endregion

        #region リポジトリの登録
        /// <summary>
        /// リポジトリの登録
        /// </summary>
        [Command]
        public void RepositoryRegistration()
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
        public void RepositoryRemove()
        {
        }
        #endregion

        #region ConfigCommand
        private ViewModelCommand configCommand;

        public ViewModelCommand ConfigCommand
        {
            get
            {
                if (this.configCommand == null)
                {
                    this.configCommand = new ViewModelCommand(Config, CanConfig);
                }
                return this.configCommand;
            }
        }

        public bool CanConfig()
        {
            return true;
        }

        /// <summary>
        /// 設定値コマンド
        /// </summary>
        public void Config()
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
            string name = this.git.GetRepository().GetBranch();

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
        public void CreateBranch()
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

        #region CommitCommand
        private ViewModelCommand commitCommand;

        public ViewModelCommand CommitCommand
        {
            get
            {
                if (this.commitCommand == null)
                {
                    this.commitCommand = new ViewModelCommand(Commit, CanCommit);
                }
                return this.commitCommand;
            }
        }

        public bool CanCommit()
        {
            return true;
        }

        /// <summary>
        /// コミットコマンド
        /// </summary>
        public void Commit()
        {
            
        }
        #endregion


        #region CloneCommand
        private ViewModelCommand cloneCommand;

        public ViewModelCommand CloneCommand
        {
            get
            {
                if (this.cloneCommand == null)
                {
                    this.cloneCommand = new ViewModelCommand(Clone, CanClone);
                }
                return this.cloneCommand;
            }
        }

        public bool CanClone()
        {
            return true;
        }

        /// <summary>
        /// 複製コマンド
        /// </summary>
        public void Clone()
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
                    command.SetURI("https://github.com/takanemu/BluePlumGit.git");
                    command.Call();
                }
                else
                {
                    MessageBox.Show(".gitディレクトリが、既に存在します。");
                }
            }
        }
        #endregion

        private readonly FilePath trash = new FilePath(new FilePath("target"), "trash");

        protected internal virtual FilePath CreateTempDirectory(string name)
        {
            string gitdirName = CreateUniqueTestFolderPrefix();
            FilePath parent = new FilePath(trash, gitdirName);
            FilePath directory = new FilePath(parent, name);
            FileUtils.Mkdirs(directory);

            return directory.GetCanonicalFile();
        }

        private string CreateUniqueTestFolderPrefix()
        {
            return System.Guid.NewGuid().ToString();
        }



        #region WindowCloseCancelCommand
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        public void WindowCloseCancel()
        {
            // TODO:終了時保存保護処理
            Environment.Exit(0);
        }
        #endregion



        /// <summary>
        /// リポジトリコンボボックスのカレントアイテムが変更された時の処理
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントパラメーター</param>
        void RepositoryCollectionViewCurrentChangedHandler(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ブランチコンボボックスのカレントアイテムが変更された時の処理
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントパラメーター</param>
        void BranchCollectionViewCurrentChangedHandler(object sender, EventArgs e)
        {
            BranchEntity entity = (BranchEntity)this.BranchCollectionView.CurrentItem;

            this.git.Checkout().SetName(entity.Name).Call();
        }

    }

    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class MainWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// プロパティ１
        /// </summary>
        public virtual string TestProperty { get; set; }
    }

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
        /// ウインドウクローズキャンセルコマンド
        /// </summary>
        public TacticsCommand WindowCloseCancel { get; set; }
    }
}
