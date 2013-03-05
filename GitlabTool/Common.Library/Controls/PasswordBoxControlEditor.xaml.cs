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

namespace Common.Library.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

    /// <summary>
    /// PasswordBoxControlEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class PasswordBoxControlEditor : UserControl, ITypeEditor
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PasswordBoxControlEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 値
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(PasswordBoxControlEditor),
                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 値
        /// </summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// バインディング設定
        /// </summary>
        /// <param name="propertyItem">アイテム情報</param>
        /// <returns>カスタムコントロール</returns>
        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this, PasswordBoxControlEditor.ValueProperty, binding);

            return this;
        }
    }
}
