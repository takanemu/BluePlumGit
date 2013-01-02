#region Apache License
//
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
//
#endregion

namespace GitlabTool.ViewModels
{
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Gordias.Library.Entitys;
    using Gordias.Library.Headquarters;
    using Gordias.Library.Interface;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #region メインクラス
    /// <summary>
    /// クローンウインドウビューモデル
    /// </summary>
    public class CloneRepositoryWindowViewModel : TacticsViewModel<CloneRepositoryWindowViewModelProperty, CloneRepositoryWindowViewModelCommand>, IWindowResult
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
        }
        #endregion

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            CloneEntity entity = new CloneEntity
            {
                Name = this.Propertys.RepositoyName,
                Path = this.Propertys.FolderPath,
                Url = this.Propertys.RemoteRepositoyUrl,
                IsCredential = this.Propertys.IsCredential,
                UserName = this.Propertys.UserName,
                PassWord = this.Propertys.PassWord,
            };
            WindowResultEntity windowResultEntity = new WindowResultEntity
            {
                Button = WindowButtonEnum.OK,
                Result = entity,
            };
            this.Responce = windowResultEntity;
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
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
            this.Responce = null;
        }
        #endregion

        /// <summary>
        /// フォルダーの選択
        /// </summary>
        /// <param name="message">フォルダー選択メッセージ</param>
        public void FolderSelected(FolderSelectionMessage message)
        {
            this.Propertys.FolderPath = message.Response;
        }

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
    public class CloneRepositoryWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual string RepositoyName { get; set; }

        /// <summary>
        /// RemoteRepositoyUrl
        /// </summary>
        public virtual string RemoteRepositoyUrl { get; set; }

        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// IsCredential
        /// </summary>
        public virtual bool IsCredential { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// PassWord
        /// </summary>
        public virtual string PassWord { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class CloneRepositoryWindowViewModelCommand
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
