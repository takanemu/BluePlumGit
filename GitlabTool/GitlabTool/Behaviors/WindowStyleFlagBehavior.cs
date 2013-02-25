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

namespace GitlabTool.Behaviors
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;

    [Flags]
    public enum WindowStyleFlag : uint
    {
        WS_SYSMENU = 0x00080000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
    }

    public class WindowStyleFlagBehavior : Behavior<Window>
    {
        const int GWL_STYLE = -16;

        [DllImport("user32")]
        private static extern uint GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32")]
        private static extern uint SetWindowLong(IntPtr hWnd, int index, WindowStyleFlag dwLong);

        public bool MinimizeBox
        {
            get { return (bool)GetValue(MinimizeBoxProperty); }
            set { this.SetValue(MinimizeBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimizeBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimizeBoxProperty =
            DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(WindowStyleFlagBehavior), new UIPropertyMetadata(true));

        public bool MaximizeBox
        {
            get { return (bool)GetValue(MaximizeBoxProperty); }
            set { this.SetValue(MaximizeBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximizeBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximizeBoxProperty =
            DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(WindowStyleFlagBehavior), new UIPropertyMetadata(true));

        public bool ControlBox
        {
            get { return (bool)GetValue(ControlBoxProperty); }
            set { this.SetValue(ControlBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlBoxProperty =
            DependencyProperty.Register("ControlBox", typeof(bool), typeof(WindowStyleFlagBehavior), new UIPropertyMetadata(true));


        protected override void OnAttached()
        {
            base.OnAttached();
            ((Window)this.AssociatedObject).SourceInitialized += new EventHandler(this.WindowStyleFlagBehavior_SourceInitialized);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            ((Window)this.AssociatedObject).SourceInitialized -= this.WindowStyleFlagBehavior_SourceInitialized;
        }

        protected WindowStyleFlag GetWindowStyle(WindowStyleFlag windowStyle)
        {
            WindowStyleFlag style = windowStyle;
            if (this.MinimizeBox)
                style |= WindowStyleFlag.WS_MINIMIZEBOX;
            else
                style &= ~WindowStyleFlag.WS_MINIMIZEBOX;

            if (this.MaximizeBox)
                style |= WindowStyleFlag.WS_MAXIMIZEBOX;
            else
                style &= ~WindowStyleFlag.WS_MAXIMIZEBOX;

            if (this.ControlBox)
                style |= WindowStyleFlag.WS_SYSMENU;
            else
                style &= ~WindowStyleFlag.WS_SYSMENU;

            return style;
        }

        void WindowStyleFlagBehavior_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this.AssociatedObject)).Handle;

            WindowStyleFlag original = (WindowStyleFlag)GetWindowLong(handle, GWL_STYLE);
            WindowStyleFlag current = this.GetWindowStyle(original);

            if (original != current)
            {
                SetWindowLong(handle, GWL_STYLE, current);
            }
        }
    }
}
