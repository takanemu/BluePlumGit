﻿
namespace GitlabTool.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interactivity;
    using GitlabTool.MetroChrome;
    
    /// <summary>
    /// 
    /// </summary>
    internal class MetroChromeBehavior : Behavior<Window>
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += (sender, e) =>
            {
                var left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
                var right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
                var top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
                var bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);

                left.Show();
                right.Show();
                top.Show();
                bottom.Show();

                left.Update();
                right.Update();
                top.Update();
                bottom.Update();
            };
        }
    }
}
