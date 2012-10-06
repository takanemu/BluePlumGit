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
    using BluePlumGit.Entitys;
    using GordiasClassLibrary.Collections;
    using GordiasClassLibrary.Entitys;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interface;
    using System.ComponentModel;
    using Common.Library.Enums;
    using Livet.Messaging.Windows;

    #region メインクラス
    /// <summary>
    /// リポジトリ削除ビューモデル
    /// </summary>
    public class RemoveRepositoryWindowViewModel : TacticsViewModel<RemoveRepositoryWindowViewModelProperty, RemoveRepositoryWindowViewModelCommand>, IWindowParameter, IWindowResult
    {
        /// <summary>
        /// 登録リポジトリリスト
        /// </summary>
        private CleanupObservableCollection<RepositoryEntity> repositorysCollection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RemoveRepositoryWindowViewModel()
        {
            this.repositorysCollection = new CleanupObservableCollection<RepositoryEntity>();
        }

        /// <summary>
        /// リポジトリコレクションビューのプロパティ
        /// </summary>
        public ICollectionView RepositoryCollectionView
        {
            get
            {
                return this.repositorysCollection.View;
            }
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
            List<RepositoryEntity> list = (List<RepositoryEntity>)this.Parameter;

            foreach(var item in list)
            {
                this.repositorysCollection.Add(item);
            }
            this.RepositoryCollectionView.MoveCurrentToFirst();
        }
        #endregion

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
                Result = this.RepositoryCollectionView.CurrentItem,
            };
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
    public class RemoveRepositoryWindowViewModelProperty : TacticsProperty
    {
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class RemoveRepositoryWindowViewModelCommand
    {
        /// <summary>
        /// 削除Buttonコマンド
        /// </summary>
        public TacticsCommand RemoveButton { get; private set; }
    }
    #endregion
}
