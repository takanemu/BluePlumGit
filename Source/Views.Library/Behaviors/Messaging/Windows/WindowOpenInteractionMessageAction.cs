﻿
namespace BluePlumGit.Behaviors.Messaging.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Livet.Behaviors.Messaging;
    using System.Windows;
    using Livet.Messaging;
    using BluePlumGit.Messaging.Windows;
    using BluePlumGit.ViewModels;
    using BluePlumGit.Entitys;
    using BluePlumGit.Enums;
    using BluePlumGit.Views;

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