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

namespace BluePlumGit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GordiasClassLibrary.Entitys;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interface;
    using Livet.Messaging.Windows;
    using Common.Library.Enums;
    using BluePlumGit.Entitys;

    public class CreateBranchWindowViewModel : TacticsViewModel<CreateBranchWindowViewModelProperty, CreateBranchWindowViewModelCommand>, IWindowResult
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
            BranchEntity branch = new BranchEntity
            {
                Name = this.Propertys.BranceName,
            };

            WindowResultEntity windowResultEntity = new WindowResultEntity
            {
                Button = WindowButtonEnum.OK,
                Result = branch,
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
        }
        #endregion

        /// <summary>
        /// 戻り値
        /// </summary>
        public WindowResultEntity Responce { get; set; }
    }

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class CreateBranchWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// ブランチ名
        /// </summary>
        public virtual string BranceName { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class CreateBranchWindowViewModelCommand
    {
        /// <summary>
        /// OkButtonコマンド
        /// </summary>
        public TacticsCommand OkButton { get; set; }

        /// <summary>
        /// CancelButtonコマンド
        /// </summary>
        public TacticsCommand CancelButton { get; set; }
    }
    #endregion
}
