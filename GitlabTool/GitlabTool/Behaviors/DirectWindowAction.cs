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

namespace GitlabTool.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;
    
    /// <summary>
    /// DirectWindowAction
    /// </summary>
    internal class DirectWindowAction : TriggerAction<FrameworkElement>
    {
        #region WindowAction 依存関係プロパティ
        /// <summary>
        /// WindowAction 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty WindowActionProperty =
            DependencyProperty.Register("WindowAction", typeof(WindowAction), typeof(DirectWindowAction), new UIPropertyMetadata(WindowAction.Active));
        
        /// <summary>
        /// WindowAction 依存関係プロパティ
        /// </summary>
        public WindowAction WindowAction
        {
            get
            {
                return (WindowAction)this.GetValue(DirectWindowAction.WindowActionProperty);
            }

            set
            {
                this.SetValue(DirectWindowAction.WindowActionProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="parameter">パラメーター</param>
        protected override void Invoke(object parameter)
        {
            var window = Window.GetWindow(this.AssociatedObject);

            if (window != null)
            {
                switch (this.WindowAction)
                {
                    case WindowAction.Active:
                        window.Activate();
                        break;
                    case WindowAction.Close:
                        window.Close();
                        break;
                    case WindowAction.Maximize:
                        window.WindowState = WindowState.Maximized;
                        break;
                    case WindowAction.Minimize:
                        window.WindowState = WindowState.Minimized;
                        break;
                    case WindowAction.Normalize:
                        window.WindowState = WindowState.Normal;
                        break;
                    case WindowAction.OpenSystemMenu:
                        var point = this.AssociatedObject.PointToScreen(new Point(0, this.AssociatedObject.ActualHeight));
                        SystemCommands.ShowSystemMenu(window, point);
                        break;
                }
            }
        }
    }
}
