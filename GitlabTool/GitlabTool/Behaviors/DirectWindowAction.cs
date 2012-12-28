
namespace GitlabTool.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;
    
    /// <summary>
    /// 
    /// </summary>
    internal class DirectWindowAction : TriggerAction<FrameworkElement>
	{
		#region WindowAction 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty WindowActionProperty =
            DependencyProperty.Register("WindowAction", typeof(WindowAction), typeof(DirectWindowAction), new UIPropertyMetadata(WindowAction.Active));
        
        /// <summary>
        /// 
        /// </summary>
        public WindowAction WindowAction
		{
			get { return (WindowAction)this.GetValue(DirectWindowAction.WindowActionProperty); }
			set { this.SetValue(DirectWindowAction.WindowActionProperty, value); }
		}
		#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
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
