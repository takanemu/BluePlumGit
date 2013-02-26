
namespace GitlabTool.MetroChrome
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    
    /// <summary>
    /// 
    /// </summary>
    public class CaptionButtons : Control
    {
        /// <summary>
        /// 
        /// </summary>
        static CaptionButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionButtons), new FrameworkPropertyMetadata(typeof(CaptionButtons)));
        }

        #region CanMinimize 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register("CanMinimize", typeof(bool), typeof(CaptionButtons), new UIPropertyMetadata(true));
        
        /// <summary>
        /// 
        /// </summary>
        public bool CanMinimize
        {
            get { return (bool)this.GetValue(CaptionButtons.CanMinimizeProperty); }
            set { this.SetValue(CaptionButtons.CanMinimizeProperty, value); }
        }
        #endregion

        #region CanMaximize 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.Register("CanMaximize", typeof(bool), typeof(CaptionButtons), new UIPropertyMetadata(true));
        
        /// <summary>
        /// 
        /// </summary>
        public bool CanMaximize
        {
            get { return (bool)this.GetValue(CaptionButtons.CanMaximizeProperty); }
            set { this.SetValue(CaptionButtons.CanMaximizeProperty, value); }
        }
        #endregion

        #region CanNormalize 依存関係プロパティ
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanNormalizeProperty =
            DependencyProperty.Register("CanNormalize", typeof(bool), typeof(CaptionButtons), new UIPropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool CanNormalize
        {
            get { return (bool)this.GetValue(CaptionButtons.CanNormalizeProperty); }
            set { this.SetValue(CaptionButtons.CanNormalizeProperty, value); }
        }
        #endregion
    }
}
