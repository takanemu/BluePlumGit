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
    using System.Collections.Generic;
    using Livet;
    using Livet.Commands;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;

    public class InitializeRepositoryWindowViewModel : ViewModel
    {
        /// <summary>
        /// フォルダーの選択
        /// </summary>
        /// <param name="message">フォルダー選択メッセージ</param>
        public void FolderSelected(FolderSelectionMessage message)
        {
            this.FolderPath = message.Response;
        }

        #region RepositoyName変更通知プロパティ
        private string _RepositoyName;

        public string RepositoyName
        {
            get
            { return _RepositoyName; }
            set
            {
                if (EqualityComparer<string>.Default.Equals(_RepositoyName, value))
                {
                    return;
                }
                _RepositoyName = value;
                RaisePropertyChanged("RepositoyName");
            }
        }
        #endregion

        #region FolderPath変更通知プロパティ
        private string _FolderPath;

        public string FolderPath
        {
            get
            {
                return _FolderPath;
            }
            
            set
            {
                if (EqualityComparer<string>.Default.Equals(_FolderPath, value))
                {
                    return;
                }
                _FolderPath = value;
                RaisePropertyChanged("FolderPath");
            }
        }
        #endregion

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
            this.Result = true;
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
            this.Result = false;
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
            // TODO:自分で閉じる方法が無い？
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
                {
                    return;
                }
                _CanClose = value;
                RaisePropertyChanged("CanClose");
            }
        }
        #endregion

        public bool Result { get; set; }
    }
}
