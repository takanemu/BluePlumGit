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
    using System;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using GitlabTool.Win32;
    
    /// <summary>
    /// ResizeGripBehavior
    /// </summary>
    internal class ResizeGripBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// isEnabled
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Initialized += (sender, e) =>
            {
                var window = Window.GetWindow(this.AssociatedObject);
                window.StateChanged += (sender2, e2) =>
                {
                    this.isEnabled = window.WindowState == WindowState.Normal;
                };
                window.SourceInitialized += (sender2, e2) =>
                {
                    var source = (HwndSource)HwndSource.FromVisual(window);
                    source.AddHook(this.WndProc);
                };
            };
        }

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="hwnd">ウインドウハンドル</param>
        /// <param name="msg">メッセージ</param>
        /// <param name="wParam">ワードパラメーター</param>
        /// <param name="lParam">ロングパラメーター</param>
        /// <param name="handled">ハンドル</param>
        /// <returns>戻り値</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WM.NCHITTEST)
            {
                var ptScreen = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
                var ptClient = this.AssociatedObject.PointFromScreen(ptScreen);

                if (new Rect(0, 0, this.AssociatedObject.ActualWidth, this.AssociatedObject.ActualHeight).Contains(ptClient))
                {
                    handled = true;
                    return (IntPtr)HitTestValues.HTBOTTOMRIGHT;
                }
            }
            return IntPtr.Zero;
        }
    }
}
