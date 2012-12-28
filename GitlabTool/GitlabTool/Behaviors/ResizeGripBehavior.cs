
namespace GitlabTool.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using GitlabTool.Win32;
    
    /// <summary>
    /// 
    /// </summary>
    internal class ResizeGripBehavior : Behavior<FrameworkElement>
	{
        /// <summary>
        /// 
        /// </summary>
		private bool isEnabled;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
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
