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

namespace GitlabTool.Behaviors.Messaging.Windows
{
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Common.Library.Messaging.Windows;
    using GitlabTool.ViewModels;
    using GitlabTool.Views;
    using Gordias.Library.Interface;
    using Livet.Behaviors.Messaging;
    using Livet.Messaging;
    using System;
    using System.Windows;

    /// <summary>
    /// ウインドウオープンメッセージアクションクラス
    /// </summary>
    public class WindowOpenInteractionMessageAction : InteractionMessageAction<DependencyObject>
    {
        /// <summary>
        /// メッセージアクション
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            if (!(message is WindowOpenMessage))
            {
                return;
            }
            WindowOpenMessage windowOpenMessage = (WindowOpenMessage)message;

            Window window = this.CreateWindow(windowOpenMessage.WindowType);

            // モーダルウィンドウ設定
            window.Owner = (Window)this.AssociatedObject;

            IWindowParameter context = window.DataContext as IWindowParameter;

            if (context != null)
            {
                context.Parameter = windowOpenMessage.Parameter;
            }

            //Nullable<bool> dialogResult = window.ShowDialog();
            window.ShowDialog();

            IWindowResult viewModel = window.DataContext as IWindowResult;

            if (viewModel != null)
            {
                windowOpenMessage.Response = viewModel.Responce;
            }
            else
            {
                windowOpenMessage.Response = null;
            }
        }

        /// <summary>
        /// ウインドウクラスの生成
        /// </summary>
        /// <param name="type">ウインドウ種別</param>
        /// <returns>ウインドウインスタンス</returns>
        private Window CreateWindow(WindowTypeEnum type)
        {
            Window result = null;
            
            switch (type)
            {
                //case WindowTypeEnum.ENTORY_REPOSITORY:
                //    result = new InitializeRepositoryWindow();
                //    break;
                //case WindowTypeEnum.REMOVE_REPOSITORY:
                //    result = new RemoveRepositoryWindow();
                //    break;
                case WindowTypeEnum.CLONE_REPOSITORY:
                    result = new CloneRepositoryWindow();
                    break;
                //case WindowTypeEnum.CREATE_BRANCH:
                //    result = new CreateBranchWindow();
                //    break;
                //case WindowTypeEnum.REMOVE_BRANCH:
                //    result = new RemoveBranchWindow();
                //    break;
                case WindowTypeEnum.CONFIG:
                    result = new ConfigSettingWindow();
                    break;
                //case WindowTypeEnum.KEYDISP:
                //    result = new KeyDispWindow();
                //    break;
                //case WindowTypeEnum.COMMIT:
                //    result = new CommitWindow();
                //    break;
                case WindowTypeEnum.CREATE_GITIGNORE:
                    result = new CreateIgnoreFileWindow();
                    break;
                case WindowTypeEnum.MANAGED_EMPTY_FOLDER:
                    result = new EmptyFolderKeepWindow();
                    break;
            }
            return result;
        }
    }
}
