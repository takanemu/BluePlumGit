
namespace BluePlumGit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Livet;
    using Livet.Commands;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;

    public class InitializeRepositoryWindowViewModel : ViewModel
    {
        public void FolderSelected(FolderSelectionMessage message)
        {
            string path = message.Response;
        }


        #region OkButtonCommand
        private Livet.Commands.ViewModelCommand _OkButtonCommand;

        public Livet.Commands.ViewModelCommand OkButtonCommand
        {
            get
            {
                if (_OkButtonCommand == null)
                {
                    _OkButtonCommand = new Livet.Commands.ViewModelCommand(OkButton, CanOkButton);
                }
                return _OkButtonCommand;
            }
        }

        public bool CanOkButton()
        {
            return true;
        }

        public void OkButton()
        {
            this.CanClose = true;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        #region CancelButtonCommand
        private Livet.Commands.ViewModelCommand _CancelButtonCommand;

        public Livet.Commands.ViewModelCommand CancelButtonCommand
        {
            get
            {
                if (_CancelButtonCommand == null)
                {
                    _CancelButtonCommand = new Livet.Commands.ViewModelCommand(CancelButton, CanCancelButton);
                }
                return _CancelButtonCommand;
            }
        }

        public bool CanCancelButton()
        {
            return true;
        }

        public void CancelButton()
        {
            this.CanClose = true;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

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
        }
        #endregion

        #region CanClose変更通知プロパティ
        private bool _CanClose;

        public bool CanClose
        {
            get
            { return _CanClose; }
            set
            { 
                if (EqualityComparer<bool>.Default.Equals(_CanClose, value))
                    return;
                _CanClose = value;
                RaisePropertyChanged("CanClose");
            }
        }
        #endregion




        
    }
}
