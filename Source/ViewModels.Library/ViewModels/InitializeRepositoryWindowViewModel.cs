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
    using System.Collections.Generic;
    using GordiasClassLibrary.Headquarters;
    using Livet;
    using Livet.Commands;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;

    /// <summary>
    /// リポジトリ初期化ビューモデル
    /// </summary>
    public class InitializeRepositoryWindowViewModel : TacticsViewModel<InitializeRepositoryWindowViewModelProperty, InitializeRepositoryWindowViewModelCommand>
    {
        /// <summary>
        /// フォルダーの選択
        /// </summary>
        /// <param name="message">フォルダー選択メッセージ</param>
        public void FolderSelected(FolderSelectionMessage message)
        {
            this.Propertys.FolderPath = message.Response;
        }

        #region OkButtonCommand
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        public void OkButton()
        {
            this.Propertys.CanClose = true;
            this.Result = true;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        #region CancelButtonCommand
        /// <summary>
        /// Cancelボタン処理
        /// </summary>
        [Command]
        public void CancelButton()
        {
            this.Propertys.CanClose = true;
            this.Result = false;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        #region WindowCloseCancelCommand
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        public void WindowCloseCancel()
        {
            // TODO:自分で閉じる方法が無い？
        }
        #endregion

        /// <summary>
        /// 結果プロパティ(falseならキャンセルボタンが押された)
        /// </summary>
        public bool Result { get; set; }
    }

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
    }

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
}
