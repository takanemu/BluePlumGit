using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;

using BluePlumGit.Models;

using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Submodule;
using NGit.Util;
using Sharpen;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using BluePlumGit.Entitys;
using BluePlumGit.Messaging.Windows;

namespace BluePlumGit.ViewModels
{
    public class MainWindowViewModel : ViewModel
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


        private MainWindowModel _model;

        private Git git;
        protected internal FileRepository db;
        private readonly IList<Repository> toClose = new AList<Repository>();

        public MainWindowViewModel()
        {
            this._model = new MainWindowModel();
            this.RepositorysCollection = new ObservableCollection<RepositoryEntity>();
            /*
            db = CreateWorkRepository();
            trash = db.WorkTree;
            git = new Git(db);
            */
        }



        #region RepositorysCollection変更通知プロパティ
        private ObservableCollection<RepositoryEntity> _RepositorysCollection;

        public ObservableCollection<RepositoryEntity> RepositorysCollection
        {
            get
            {
                return _RepositorysCollection;
            }
            
            set
            {
                if (EqualityComparer<ObservableCollection<RepositoryEntity>>.Default.Equals(_RepositorysCollection, value))
                {
                    return;
                }
                _RepositorysCollection = value;
                RaisePropertyChanged("RepositorysCollection");
            }
        }
        #endregion

        #region SelectedRepository変更通知プロパティ
        private RepositoryEntity _SelectedRepository;

        public RepositoryEntity SelectedRepository
        {
            get
            {
                return _SelectedRepository;
            }
            
            set
            {
                if (EqualityComparer<RepositoryEntity>.Default.Equals(_SelectedRepository, value))
                {
                    return;
                }
                _SelectedRepository = value;
                RaisePropertyChanged("SelectedRepository");
            }
        }
        #endregion



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
        private ViewModelCommand _LoadedCommand;

        public ViewModelCommand LoadedCommand
        {
            get
            {
                if (_LoadedCommand == null)
                {
                    _LoadedCommand = new ViewModelCommand(Loaded);
                }
                return _LoadedCommand;
            }
        }

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
                    this.RepositorysCollection.Add(item);
                }
                this.SelectedRepository = this.RepositorysCollection.ElementAt(0);
            }
        }
        #endregion

        #region InitCommand
        private ViewModelCommand _InitCommand;

        public ViewModelCommand InitCommand
        {
            get
            {
                if (_InitCommand == null)
                {
                    _InitCommand = new ViewModelCommand(Init, CanInit);
                }
                return _InitCommand;
            }
        }

        public bool CanInit()
        {
            // TODO:.gitディレクトリがあればボタンを押させない。
            return true;
        }

        /// <summary>
        /// リポジトリの初期化
        /// </summary>
        public void Init()
        {
            this.Messenger.Raise(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
            });

            /*
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == result)
            {
                string gitdirName = dialog.SelectedPath + "/" + Constants.DOT_GIT;

                FilePath gitdir = new FilePath(gitdirName);

                if (!gitdir.Exists())
                {
                    gitdir = gitdir.GetCanonicalFile();

                    FileRepository db = new FileRepository(gitdir);

                    db.Create();

                    // TODO:dbの登録
                    this._model.AddRepository(3, "名無し", gitdir);
                }
                else
                {
                    MessageBox.Show(".gitディレクトリが、既に存在します。");
                }
            }
            */
        }
        #endregion

        #region MakeCranchCommand
        private ViewModelCommand _MakeCranchCommand;

        public ViewModelCommand MakeCranchCommand
        {
            get
            {
                if (_MakeCranchCommand == null)
                {
                    _MakeCranchCommand = new ViewModelCommand(MakeCranch, CanMakeCranch);
                }
                return _MakeCranchCommand;
            }
        }

        public bool CanMakeCranch()
        {
            return true;
        }

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
        private ViewModelCommand _CommitCommand;

        public ViewModelCommand CommitCommand
        {
            get
            {
                if (_CommitCommand == null)
                {
                    _CommitCommand = new ViewModelCommand(Commit, CanCommit);
                }
                return _CommitCommand;
            }
        }

        public bool CanCommit()
        {
            return true;
        }

        public void Commit()
        {
            
        }
        #endregion


        #region CloneCommand
        private ViewModelCommand _CloneCommand;

        public ViewModelCommand CloneCommand
        {
            get
            {
                if (_CloneCommand == null)
                {
                    _CloneCommand = new ViewModelCommand(Clone, CanClone);
                }
                return _CloneCommand;
            }
        }

        public bool CanClone()
        {
            return true;
        }

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
        private ViewModelCommand _WindowCloseCancelCommand;

        public ViewModelCommand WindowCloseCancelCommand
        {
            get
            {
                if (_WindowCloseCancelCommand == null)
                {
                    _WindowCloseCancelCommand = new ViewModelCommand(WindowCloseCancel, CanWindowCloseCancel);
                }
                return _WindowCloseCancelCommand;
            }
        }

        public bool CanWindowCloseCancel()
        {
            return true;
        }

        public void WindowCloseCancel()
        {
            Environment.Exit(0);
        }
        #endregion

    }
}
