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

namespace GitlabTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Commno.Library.Entitys;
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Gordias.Library.Entitys;
    using Gordias.Library.Headquarters;
    using Gordias.Library.Interface;
    using Livet.Messaging.Windows;

    #region メインクラス
    /// <summary>
    /// 設定ウインドウビューモデルクラス
    /// </summary>
    public class ConfigSettingWindowViewModel : TacticsViewModel<ConfigSettingViewModelProperty, ConfigSettingViewModelCommand>, IWindowParameter, IWindowResult
    {
        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
        }
        #endregion

        #region Loadedメソッド
        /// <summary>
        /// Loadedメソッド
        /// </summary>
        public void Loaded()
        {
            if (this.Parameter != null)
            {
                this.Propertys.Config = (ConfigDialogEntity)((ICloneable)this.Parameter).Clone();
            }
            else
            {
                ConfigDialogEntity config = new ConfigDialogEntity();

                config.ServerUrl = string.Empty;
                config.Password = string.Empty;
                config.Accent = AccentEnum.Blue;

                this.Propertys.Config = config;
            }
        }
        #endregion

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            this.Responce = new WindowResultEntity
            {
                Button = WindowButtonEnum.OK,
                Result = this.Propertys.Config,
            };
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        #region Cancelボタン処理
        /// <summary>
        /// Cancelボタン処理
        /// </summary>
        [Command]
        private void CancelButton()
        {
            this.Responce = null;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 戻り値
        /// </summary>
        public WindowResultEntity Responce { get; set; }
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class ConfigSettingViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// コンフィグ
        /// </summary>
        public virtual ConfigDialogEntity Config { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class ConfigSettingViewModelCommand
    {
        /// <summary>
        /// OkButtonコマンド
        /// </summary>
        public TacticsCommand OkButton { get; private set; }

        /// <summary>
        /// CancelButtonコマンド
        /// </summary>
        public TacticsCommand CancelButton { get; private set; }
    }
    #endregion
}
