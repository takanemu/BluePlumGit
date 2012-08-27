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

        private Git git;
        protected internal FileRepository db;
        private readonly IList<Repository> toClose = new AList<Repository>();

        public MainWindowViewModel()
        {
            db = CreateWorkRepository();
            trash = db.WorkTree;
            git = new Git(db);
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
            FilePath directory = CreateTempDirectory("testCloneRepository");
            CloneCommand command = Git.CloneRepository();
            command.SetDirectory(directory);
            command.SetURI("file://" + git.GetRepository().WorkTree.GetPath());
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

    }
}
