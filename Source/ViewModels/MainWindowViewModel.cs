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
            //FilePath directory = CreateTempDirectory("testCloneRepository");
            CloneCommand command = Git.CloneRepository();

        }
        #endregion


    }
}
