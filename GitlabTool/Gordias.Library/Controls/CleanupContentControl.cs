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

namespace Gordias.Library.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using Gordias.Library.Interface;

    /// <summary>
    /// クリーンナップ処理付きContentControl
    /// </summary>
    public class CleanupContentControl : ContentControl 
    {
        /// <summary>
        /// コントロールのコンテンツを格納するオブジェクト
        /// </summary>
        public new Object Content
        {
            get
            {
                return base.Content;
            }

            set
            {
                if (base.Content != null && base.Content is ICleanup)
                {
                    // 破棄処理呼び出し
                    ((ICleanup)base.Content).Uninitialize();
                }
                base.Content = value;

                if (value != null && value is ICleanup)
                {
                    // 初期化処理呼び出し
                    ((ICleanup)value).Initialize();
                }
            }
        }
    }
}
