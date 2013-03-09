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
    using Commno.Library.Entitys;
using Common.Library.Entitys;
using Gordias.Library.Collections;
using Gordias.Library.Headquarters;
using Gordias.Library.Interface;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using log4net;
using NGit;
using NGit.Api;
using NGit.Storage.File;
using Sharpen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

    #region メインクラス
    /// <summary>
    /// ブランチ削除ウインドウビューモデル
    /// </summary>
    public class BranchRemoveWindowViewModel : TacticsViewModel<BranchRemoveWindowViewModelProperty, BranchRemoveWindowViewModelCommand>, IWindowParameter
    {
        /// <summary>
        /// ログ
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// ブランチリスト
        /// </summary>
        private CleanupObservableCollection<BranchEntity> branchs = new CleanupObservableCollection<BranchEntity>();

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
            this.Propertys.Branchs = this.branchs.View;

            RepositoryEntity entity = (RepositoryEntity)this.Parameter;

            this.Propertys.FolderPath = entity.Location;
            this.UpdateBranchList(this.Propertys.FolderPath);
        }
        #endregion

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
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
        /// ブランチリストを更新
        /// </summary>
        /// <param name="folder">フォルダパス</param>
        private void UpdateBranchList(string folder)
        {
            this.branchs.Clear();

            FilePath path = new FilePath(folder, @".git");
            FileRepository db = new FileRepository(path);
            Git git = new Git(db);
            IList<Ref> list = git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();

            foreach (Ref branch in list)
            {
                BranchEntity entity = new BranchEntity
                {
                    Name = branch.GetName(),
                };
                this.branchs.Add(entity);
            }
        }

        /// <summary>
        /// フォルダーの選択
        /// </summary>
        /// <param name="message">フォルダー選択メッセージ</param>
        public void FolderSelected(FolderSelectionMessage message)
        {
            if (message.Response != null)
            {
                this.UpdateBranchList(message.Response);
            }
        }

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Parameter { get; set; }
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class BranchRemoveWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// ブランチリスト
        /// </summary>
        public virtual ICollectionView Branchs { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class BranchRemoveWindowViewModelCommand
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
