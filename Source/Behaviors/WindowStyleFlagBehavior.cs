using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace BluePlumGit.Behaviors
{
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
            set { SetValue(MinimizeBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimizeBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimizeBoxProperty =
            DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(WindowStyleFlagBehavior), new UIPropertyMetadata(true));

        public bool MaximizeBox
        {
            get { return (bool)GetValue(MaximizeBoxProperty); }
            set { SetValue(MaximizeBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximizeBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximizeBoxProperty =
            DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(WindowStyleFlagBehavior), new UIPropertyMetadata(true));

        public bool ControlBox
        {
            get { return (bool)GetValue(ControlBoxProperty); }
            set { SetValue(ControlBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlBoxProperty =
            DependencyProperty.Register("ControlBox", typeof(bool), typeof(WindowStyleFlagBehavior), new UIPropertyMetadata(true));


        protected override void OnAttached()
        {
            base.OnAttached();
            ((Window)this.AssociatedObject).SourceInitialized += new EventHandler(WindowStyleFlagBehavior_SourceInitialized);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            ((Window)this.AssociatedObject).SourceInitialized -= WindowStyleFlagBehavior_SourceInitialized;
        }

        protected WindowStyleFlag GetWindowStyle(WindowStyleFlag windowStyle)
        {
            WindowStyleFlag style = windowStyle;
            if (MinimizeBox)
                style |= WindowStyleFlag.WS_MINIMIZEBOX;
            else
                style &= ~WindowStyleFlag.WS_MINIMIZEBOX;

            if (MaximizeBox)
                style |= WindowStyleFlag.WS_MAXIMIZEBOX;
            else
                style &= ~WindowStyleFlag.WS_MAXIMIZEBOX;

            if (ControlBox)
                style |= WindowStyleFlag.WS_SYSMENU;
            else
                style &= ~WindowStyleFlag.WS_SYSMENU;

            return style;
        }

        void WindowStyleFlagBehavior_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this.AssociatedObject)).Handle;

            WindowStyleFlag original = (WindowStyleFlag)GetWindowLong(handle, GWL_STYLE);
            WindowStyleFlag current = GetWindowStyle(original);

            if (original != current)
            {
                SetWindowLong(handle, GWL_STYLE, current);
            }
        }
    }
}
