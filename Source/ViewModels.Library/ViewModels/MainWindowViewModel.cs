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

        private Git git;
        protected internal FileRepository db;
        private readonly IList<Repository> toClose = new AList<Repository>();

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
        protected internal virtual FileRepository CreateWorkRepository()
        {
            return CreateRepository(false);
        }

        /// <summary>Creates a new empty repository.</summary>
        /// <remarks>Creates a new empty repository.</remarks>
        /// <param name="bare">
        /// true to create a bare repository; false to make a repository
        /// within its working directory
        /// </param>
        /// <returns>the newly created repository, opened for access</returns>
        /// <exception cref="System.IO.IOException">the repository could not be created in the temporary area
        /// 	</exception>
        private FileRepository CreateRepository(bool bare)
        {
            FilePath gitdir = CreateUniqueTestGitDir(bare);
            FileRepository db = new FileRepository(gitdir);
            //NUnit.Framework.Assert.IsFalse(gitdir.Exists());
            db.Create();
            //toClose.AddItem(db);

            return db;
        }

        protected internal virtual FilePath CreateUniqueTestGitDir(bool bare)
        {
            string gitdirName = CreateUniqueTestFolderPrefix();
            if (!bare)
            {
                gitdirName += "/";
            }
            gitdirName += Constants.DOT_GIT;
            FilePath gitdir = new FilePath(trash, gitdirName);
            return gitdir.GetCanonicalFile();
        }


        #region LoadedCommand
        /// <summary>
        /// Loadedコマンド
        /// </summary>
        [Command]
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

                git = new Git(db);

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
                this.BranchCollectionView.MoveCurrentToPosition(0);
            }
        }
        #endregion

        #region InitCommand
        /// <summary>
        /// リポジトリの初期化
        /// </summary>
        [Command]
        public void Init()
        {
            WindowOpenMessage result = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.INITIALIZE,
            });

            if (result.Response != null)
            {
                RepositoryEntity entity = result.Response;

                string gitdirName = entity.Path + "/" + Constants.DOT_GIT;

                FilePath gitdir = new FilePath(gitdirName);

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
        #endregion

        #region RepositoryEntoryCommand
        private ViewModelCommand repositoryEntoryCommand;

        public ViewModelCommand RepositoryEntoryCommand
        {
            get
            {
                if (this.repositoryEntoryCommand == null)
                {
                    this.repositoryEntoryCommand = new ViewModelCommand(RepositoryEntory, CanRepositoryEntory);
                }
                return this.repositoryEntoryCommand;
            }
        }

        public bool CanRepositoryEntory()
        {
            return true;
        }

        /// <summary>
        /// リポジトリの登録
        /// </summary>
        public void RepositoryEntory()
        {
            
        }
        #endregion

        #region RepositoryRemoveCommand
        private ViewModelCommand repositoryRemoveCommand;

        public ViewModelCommand RepositoryRemoveCommand
        {
            get
            {
                if (this.repositoryRemoveCommand == null)
                {
                    this.repositoryRemoveCommand = new ViewModelCommand(RepositoryRemove, CanRepositoryRemove);
                }
                return this.repositoryRemoveCommand;
            }
        }

        public bool CanRepositoryRemove()
        {
            return true;
        }

        /// <summary>
        /// リポジトリの削除
        /// </summary>
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

        #region MakeCranchCommand
        private ViewModelCommand makeCranchCommand;

        public ViewModelCommand MakeCranchCommand
        {
            get
            {
                if (this.makeCranchCommand == null)
                {
                    this.makeCranchCommand = new ViewModelCommand(MakeCranch, CanMakeCranch);
                }
                return this.makeCranchCommand;
            }
        }

        public bool CanMakeCranch()
        {
            return true;
        }

        /// <summary>
        /// ブランチの作成
        /// </summary>
        public void MakeCranch()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == result)
            {
                string gitdirName = dialog.SelectedPath + "/" + Constants.DOT_GIT;

                FilePath gitdir = new FilePath(gitdirName);

                if (gitdir.Exists())
                {
                    gitdir = gitdir.GetCanonicalFile();
                    FileRepository db = new FileRepository(gitdir);
                    Git git = new Git(db);

                    int localBefore = git.BranchList().Call().Count;

                    git.BranchCreate().SetName("TestBranch2").Call();

                    //Ref newBranch = this.CreateBranch(git, "testbranch", false, "master", null);
                }
                else
                {
                    MessageBox.Show(".gitディレクトリが、存在しません。");
                }
            }
        }
        #endregion

        private Ref CreateBranch(Git actGit, string name, bool force, string startPoint, CreateBranchCommand.SetupUpstreamMode? mode)
        {
            CreateBranchCommand cmd = actGit.BranchCreate();
            cmd.SetName(name);
            cmd.SetForce(force);
            cmd.SetStartPoint(startPoint);
            cmd.SetUpstreamMode(mode != null ? mode.Value : CreateBranchCommand.SetupUpstreamMode.NOT_SET);

            return cmd.Call();
        }


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
        private ViewModelCommand windowCloseCancelCommand;

        public ViewModelCommand WindowCloseCancelCommand
        {
            get
            {
                if (this.windowCloseCancelCommand == null)
                {
                    this.windowCloseCancelCommand = new ViewModelCommand(WindowCloseCancel, CanWindowCloseCancel);
                }
                return this.windowCloseCancelCommand;
            }
        }

        public bool CanWindowCloseCancel()
        {
            return true;
        }

        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        public void WindowCloseCancel()
        {
            // TODO:終了時保存保護処理
            Environment.Exit(0);
        }
        #endregion

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
        /// Loadedコマンド
        /// </summary>
        public TacticsCommand Loaded { get; set; }

        /// <summary>
        /// Initコマンド
        /// </summary>
        public TacticsCommand Init { get; set; }
    }
}
