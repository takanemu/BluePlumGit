
namespace GitlabTool.MetroChrome
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    
    /// <summary>
    /// 
    /// </summary>
    public class Glow : Control
	{
        /// <summary>
        /// 
        /// </summary>
		static Glow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Glow), new FrameworkPropertyMetadata(typeof(Glow)));
		}

		#region GlowColor 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlowColorProperty =
            DependencyProperty.Register("GlowColor", typeof(Color), typeof(Glow), new UIPropertyMetadata(Colors.Transparent));
        
        /// <summary>
        /// 
        /// </summary>
        public Color GlowColor
		{
			get { return (Color)this.GetValue(Glow.GlowColorProperty); }
			set { this.SetValue(Glow.GlowColorProperty, value); }
		}

		#endregion

		#region IsGlow 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsGlowProperty =
            DependencyProperty.Register("IsGlow", typeof(bool), typeof(Glow), new UIPropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
		public bool IsGlow
		{
			get { return (bool)this.GetValue(Glow.IsGlowProperty); }
			set { this.SetValue(Glow.IsGlowProperty, value); }
		}
		#endregion

		#region Orientation 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Glow), new UIPropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// 
        /// </summary>
		public Orientation Orientation
		{
			get { return (Orientation)this.GetValue(Glow.OrientationProperty); }
			set { this.SetValue(Glow.OrientationProperty, value); }
		}
		#endregion

	}
}
