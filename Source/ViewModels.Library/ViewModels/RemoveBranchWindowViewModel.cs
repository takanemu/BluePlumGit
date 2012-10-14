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
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using BluePlumGit.Entitys;
    using BluePlumGit.Utility;
    using Common.Library.Enums;
    using GordiasClassLibrary.Collections;
    using GordiasClassLibrary.Entitys;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interface;
    using Livet.Messaging.Windows;
    using NGit;
    using NGit.Api;

    #region メインクラス
    /// <summary>
    /// ブランチの削除
    /// </summary>
    public class RemoveBranchWindowViewModel : TacticsViewModel<RemoveBranchWindowViewModelProperty, RemoveBranchWindowViewModelCommand>, IWindowParameter, IWindowResult
    {
        /// <summary>
        /// カレントリポジトリ
        /// </summary>
        private Git git;

        /// <summary>
        /// ブランチリスト
        /// </summary>
        private CleanupObservableCollection<BranchEntity> branchCollection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RemoveBranchWindowViewModel()
        {
            this.branchCollection = new CleanupObservableCollection<BranchEntity>();
        }

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
            this.git = (Git)this.Parameter;

            this.UpdateBranchList();
        }
        #endregion

        /// <summary>
        /// ブランチコレクションのプロパティ
        /// </summary>
        public ICollectionView BranchCollectionView
        {
            get
            {
                return this.branchCollection.View;
            }
        }

        #region 削除ボタン処理
        /// <summary>
        /// 削除ボタン処理
        /// </summary>
        [Command]
        private void RemoveButton()
        {
            this.Responce = new WindowResultEntity
            {
                Button = WindowButtonEnum.DELETE,
                Result = this.BranchCollectionView.CurrentItem,
            };
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        /// <summary>
        /// ブランチリストの更新
        /// </summary>
        private void UpdateBranchList()
        {
            IList<Ref> list = this.git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();
            string current = RepositotyUtility.GetCurrentBranch(this.git);

            this.branchCollection.Clear();

            foreach (Ref branch in list)
            {
                BranchEntity be = new BranchEntity
                {
                    ID = 0,
                    Name = Path.GetFileName(branch.GetName()),
                    Path = branch.GetName(),
                };
                if (current != be.Name)
                {
                    this.branchCollection.Add(be);
                }
            }
            this.BranchCollectionView.MoveCurrentToFirst();
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
    public class RemoveBranchWindowViewModelProperty : TacticsProperty
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
    public class RemoveBranchWindowViewModelCommand
    {
        /// <summary>
        /// 削除Buttonコマンド
        /// </summary>
        public TacticsCommand RemoveButton { get; private set; }
    }
    #endregion
}
