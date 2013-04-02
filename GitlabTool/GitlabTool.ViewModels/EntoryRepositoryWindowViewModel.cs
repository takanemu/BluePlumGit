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

namespace GitlabTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Gordias.Library.Entitys;
    using Gordias.Library.Headquarters;
    using Gordias.Library.Interface;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;
    using log4net;

    #region メインクラス
    public class EntoryRepositoryWindowViewModel : TacticsViewModel<EntoryRepositoryWindowViewModelProperty, EntoryRepositoryWindowViewModelCommand>, IWindowParameter, IWindowResult
    {
        /// <summary>
        /// ログ
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            logger.Info("操作：OKボタン");

            RepositoryEntity entity = new RepositoryEntity
            {
                Name = this.Propertys.RepositoryName,
                Location = this.Propertys.FolderPath,
                Path = this.Propertys.GitAddress,
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
            logger.Info("操作：Cancelボタン");

            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
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
    public class EntoryRepositoryWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// リポジトリ名称
        /// </summary>
        public virtual string RepositoryName { get; set; }

        /// <summary>
        /// リポジトリアドレス
        /// </summary>
        public virtual string GitAddress { get; set; }

        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class EntoryRepositoryWindowViewModelCommand
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
