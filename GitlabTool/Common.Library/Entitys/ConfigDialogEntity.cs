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

namespace Common.Library.Entitys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Library.Controls;
    using Common.Library.Enums;
    using Gitlab;

    /// <summary>
    /// アプリケーション設定
    /// </summary>
    [DisplayName("アプリケーション設定")]
    public class ConfigDialogEntity : ICloneable 
    {
        /// <summary>
        /// サーバーアドレス
        /// </summary>
        [Category("Core")]
        [DisplayName("Server Url")]
        [Description("Gitlab Server のURLを設定してください。")]
        public string ServerUrl
        {
            get;
            set;
        }

        /// <summary>
        /// パスワード
        /// </summary>
        [Category("Core")]
        [DisplayName("Password")]
        [Description("Gitlab Server のPasswordを設定してください。")]
        [Editor(typeof(PasswordBoxControlEditor), typeof(PasswordBoxControlEditor))]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// テーマカラー
        /// </summary>
        [Category("Core")]
        [DisplayName("Theme")]
        [Description("ウインドウのテーマを設定してください。")]
        public AccentEnum Accent
        {
            get;
            set;
        }

        /// <summary>
        /// 通信ライブラリバージョン
        /// </summary>
        [Category("Core")]
        [DisplayName("API Version")]
        [Description("通信ライブラリのバージョンを設定してください。")]
        public ApiVersionEnum ApiVersion
        {
            get;
            set;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns>複製されたインスタンス</returns>
        public virtual object Clone()
        {
            ConfigDialogEntity instance = (ConfigDialogEntity)Activator.CreateInstance(GetType());

            instance.ServerUrl = this.ServerUrl;
            instance.Password = this.Password;
            instance.Accent = this.Accent;
            instance.ApiVersion = this.ApiVersion;

            return instance;
        }
    }
}
