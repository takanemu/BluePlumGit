﻿#region Apache License
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

namespace GitlabTool.Behaviors
{
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Iconを設定する
    /// </summary>
    public class IconSetBehavior : Behavior<Window>
    {
        /// <summary>
        /// リソース名
        /// </summary>
        public string ResourceName { private get; set; }

        /// <summary>
        /// ビヘイビアがアタッチされた時の処理
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            //現在のコードを実行しているAssemblyを取得する
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            Window window = (Window)this.AssociatedObject;

            //ソフトウェアの左上のアイコンの指定
            window.Icon = BitmapFrame.Create(myAssembly.GetManifestResourceStream(this.ResourceName));
        }
    }
}
