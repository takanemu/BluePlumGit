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
    using BluePlumGit.Entitys;
    using Common.Library.Enums;
    using GordiasClassLibrary.Entitys;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interface;
    using Livet.Commands;
    using Livet.Messaging.Windows;

    /// <summary>
    /// リポジトリ初期化ビューモデル
    /// </summary>
    public class InitializeRepositoryWindowViewModel : TacticsViewModel<InitializeRepositoryWindowViewModelProperty, InitializeRepositoryWindowViewModelCommand>, IWindowResult
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
            this.Propertys.IsEntryOnly = true;
        }
        #endregion

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            RepositoryEntity repositoryEntity = new RepositoryEntity
            {
                Name = this.Propertys.RepositoyName,
                Path = this.Propertys.FolderPath,
            };

            InitializeRepositoryEntity initializeRepositoryEntity = new InitializeRepositoryEntity
            {
                Mode = this.Propertys.IsEntryOnly == true ? InitializeRepositoryEnum.EntryOnly : InitializeRepositoryEnum.InitializeAndEntry,
                Entity = repositoryEntity,
            };

            WindowResultEntity windowResultEntity = new WindowResultEntity
            {
                Button = WindowButtonEnum.OK,
                Result = initializeRepositoryEntity,
            };
            this.Responce = windowResultEntity;
            this.Propertys.CanClose = true;
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
            this.Propertys.CanClose = true;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
            this.Responce = null;
        }
        #endregion

        #region WindowCloseCancelCommand
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        private void WindowCloseCancel()
        {
            // TODO:自分で閉じる方法が無い？
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
    public class InitializeRepositoryWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual string RepositoyName { get; set; }

        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// CanClose
        /// </summary>
        public virtual bool CanClose { get; set; }

        /// <summary>
        /// 登録のみ
        /// </summary>
        public virtual bool IsEntryOnly { get; set; }

        /// <summary>
        /// 初期化後に登録
        /// </summary>
        public virtual bool IsInitializeAndEntry { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class InitializeRepositoryWindowViewModelCommand
    {
        /// <summary>
        /// OkButtonコマンド
        /// </summary>
        public TacticsCommand OkButton { get; set; }

        /// <summary>
        /// CancelButtonコマンド
        /// </summary>
        public TacticsCommand CancelButton { get; set; }

        /// <summary>
        /// ウインドウクローズキャンセルコマンド
        /// </summary>
        public TacticsCommand WindowCloseCancel { get; set; }
    }
    #endregion
}
