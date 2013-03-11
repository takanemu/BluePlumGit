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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Data;

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

        /// <summary>
        /// Git
        /// </summary>
        private Git git;

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
            this.Propertys.Branchs = this.branchs.View;

            RepositoryEntity entity = (RepositoryEntity)this.Parameter;

            if (entity != null)
            {
                this.Propertys.FolderPath = entity.Location;
                this.UpdateBranchList(this.Propertys.FolderPath);
            }
            this.Propertys.Branchs.Filter = new System.Predicate<object>(FilterCallback);
        }
        #endregion

        /// <summary>
        /// フィルター関数
        /// </summary>
        /// <param name="item">項目</param>
        /// <returns>true表示</returns>
        private bool FilterCallback(object item)
        {
            if (this.Propertys.IsAllDisp)
            {
                return true;
            }
            BranchEntity entity = (BranchEntity)item;

            if (entity.Name.LastIndexOf("master") != -1)
            {
                return false;
            }
            return true;
        }

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            ListCollectionView view = (ListCollectionView)this.Propertys.Branchs;

            string[] names = new string[view.Count];

            int i = 0;

            foreach(var item in view)
            {
                names[i++] = ((BranchEntity)item).Name;
            }

            try
            {
                this.git.BranchDelete().SetForce(this.Propertys.IsForceDelete).SetBranchNames(names).Call();
                this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
            }
            catch (NGit.Api.Errors.NotMergedException exception)
            {
                MessageBox.Show("注意：マージされていないブランチを削除することは制限されています。\n強制的に削除したい場合には、強制削除のチェックボックをオンにしてください。");
            }
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
            this.git = new Git(db);
            //IList<Ref> remote = this.git.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE).Call();
            IList<Ref> all = this.git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();

            foreach (Ref branch in all)
            {
                BranchEntity entity = new BranchEntity
                {
                    Name = branch.GetName(),
                };
                this.branchs.Add(entity);
            }
        }

        /// <summary>
        /// 表示更新
        /// </summary>
        [Property]
        private void IsAllDisp()
        {
            this.Propertys.Branchs.Refresh();
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

        /// <summary>
        /// 強制削除
        /// </summary>
        public virtual bool IsForceDelete { get; set; }

        /// <summary>
        /// すべて表示
        /// </summary>
        public virtual bool IsAllDisp { get; set; }
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
