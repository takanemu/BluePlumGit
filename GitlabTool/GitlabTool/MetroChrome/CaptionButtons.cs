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
    /// CaptionButtons
    /// </summary>
    public class CaptionButtons : Control
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        static CaptionButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionButtons), new FrameworkPropertyMetadata(typeof(CaptionButtons)));
        }

        #region CanMinimize 依存関係プロパティ
        /// <summary>
        /// CanMinimize
        /// </summary>
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register("CanMinimize", typeof(bool), typeof(CaptionButtons), new UIPropertyMetadata(true));
        
        /// <summary>
        /// CanMinimize
        /// </summary>
        public bool CanMinimize
        {
            get { return (bool)this.GetValue(CaptionButtons.CanMinimizeProperty); }
            set { this.SetValue(CaptionButtons.CanMinimizeProperty, value); }
        }
        #endregion

        #region CanMaximize 依存関係プロパティ
        /// <summary>
        /// CanMaximize
        /// </summary>
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.Register("CanMaximize", typeof(bool), typeof(CaptionButtons), new UIPropertyMetadata(true));
        
        /// <summary>
        /// CanMaximize
        /// </summary>
        public bool CanMaximize
        {
            get { return (bool)this.GetValue(CaptionButtons.CanMaximizeProperty); }
            set { this.SetValue(CaptionButtons.CanMaximizeProperty, value); }
        }
        #endregion

        #region CanNormalize 依存関係プロパティ
        /// <summary>
        /// CanNormalize
        /// </summary>
        public static readonly DependencyProperty CanNormalizeProperty =
            DependencyProperty.Register("CanNormalize", typeof(bool), typeof(CaptionButtons), new UIPropertyMetadata(false));

        /// <summary>
        /// CanNormalize
        /// </summary>
        public bool CanNormalize
        {
            get { return (bool)this.GetValue(CaptionButtons.CanNormalizeProperty); }
            set { this.SetValue(CaptionButtons.CanNormalizeProperty, value); }
        }
        #endregion
    }
}
