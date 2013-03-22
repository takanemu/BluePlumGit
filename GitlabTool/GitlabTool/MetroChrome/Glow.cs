#region License
// <copyright>
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
// </copyright>
#endregion

namespace GitlabTool.MetroChrome
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    
    /// <summary>
    /// Glow
    /// </summary>
    public class Glow : Control
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        static Glow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Glow), new FrameworkPropertyMetadata(typeof(Glow)));
        }

        #region GlowColor 依存関係プロパティ
        /// <summary>
        /// GlowColor
        /// </summary>
        public static readonly DependencyProperty GlowColorProperty =
            DependencyProperty.Register("GlowColor", typeof(Color), typeof(Glow), new UIPropertyMetadata(Colors.Transparent));
        
        /// <summary>
        /// GlowColor
        /// </summary>
        public Color GlowColor
        {
            get { return (Color)this.GetValue(Glow.GlowColorProperty); }
            set { this.SetValue(Glow.GlowColorProperty, value); }
        }

        #endregion

        #region IsGlow 依存関係プロパティ
        /// <summary>
        /// IsGlow
        /// </summary>
        public static readonly DependencyProperty IsGlowProperty =
            DependencyProperty.Register("IsGlow", typeof(bool), typeof(Glow), new UIPropertyMetadata(true));

        /// <summary>
        /// IsGlow
        /// </summary>
        public bool IsGlow
        {
            get { return (bool)this.GetValue(Glow.IsGlowProperty); }
            set { this.SetValue(Glow.IsGlowProperty, value); }
        }
        #endregion

        #region Orientation 依存関係プロパティ
        /// <summary>
        /// Orientation
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Glow), new UIPropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// Orientation
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(Glow.OrientationProperty); }
            set { this.SetValue(Glow.OrientationProperty, value); }
        }
        #endregion
    }
}
