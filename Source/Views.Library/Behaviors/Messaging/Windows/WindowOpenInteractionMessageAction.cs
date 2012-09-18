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

namespace BluePlumGit.Behaviors.Messaging.Windows
{
    using System;
    using System.Windows;
    using BluePlumGit.Entitys;
    using BluePlumGit.Enums;
    using BluePlumGit.Messaging.Windows;
    using BluePlumGit.ViewModels;
    using BluePlumGit.Views;
    using Livet.Behaviors.Messaging;
    using Livet.Messaging;

    public class WindowOpenInteractionMessageAction : InteractionMessageAction<DependencyObject>
    {
        protected override void InvokeAction(InteractionMessage message)
        {
            if (!(message is WindowOpenMessage))
            {
                return;
            }
            WindowOpenMessage windowOpenMessage = (WindowOpenMessage)message;

            //Window window = (Window)Activator.CreateInstance(windowOpenMessage.WindowType);

            Window window = CreateWindow(windowOpenMessage.WindowType);

            // モーダルウィンドウ設定
            window.Owner = (Window)this.AssociatedObject;

            Nullable<bool> dialogResult = window.ShowDialog();

            InitializeRepositoryWindowViewModel vm = (InitializeRepositoryWindowViewModel)window.DataContext;

            if (vm.Result)
            {
                RepositoryEntity entity = new RepositoryEntity
                {
                    Name = vm.RepositoyName,
                    Path = vm.FolderPath,
                };
                windowOpenMessage.Response = entity;
            }
            else
            {
                windowOpenMessage.Response = null;
            }
        }

        private Window CreateWindow(WindowTypeEnum type)
        {
            Window result = null;

            switch (type)
            {
                case WindowTypeEnum.INITIALIZE:
                    result = new InitializeRepositoryWindow();
                    break;
            }
            return result;
        }
    }
}
