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

namespace Gordias.Library.Collections
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Data;
    using Gordias.Library.Interface;

    /// <summary>
    /// クリーンナップ処理付きコレクション
    /// </summary>
    /// <typeparam name="ItemType">アイテム型</typeparam>
    public class CleanupObservableCollection<ItemType> : ObservableCollection<ItemType>
    {
        /// <summary>
        /// 廃棄済みフラグ
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// CollectionViewSource
        /// </summary>
        private CollectionViewSource collectionViewSource;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CleanupObservableCollection()
            : base()
        {
            this.collectionViewSource = new CollectionViewSource();
            this.collectionViewSource.Source = this;

            this.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.CleanupObservableCollection_CollectionChanged);
        }

        /// <summary>
        /// デストラクタ<br/>
        /// </summary>
        ~CleanupObservableCollection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// ICollectionViewの公開
        /// </summary>
        public ICollectionView View
        {
            get
            {
                return this.collectionViewSource.View;
            }
        }

        /// <summary>
        /// ファイナラーザー<br/>
        /// メモリの廃棄処理を行います。<br/>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// コレクション変更処理
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        private void CleanupObservableCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IList list = null;
            Action<ICleanup> action = (ICleanup vm) => { };

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                list = e.NewItems as IList;

                action = (ICleanup vm) =>
                {
                    // 初期化処理の呼び出し
                    vm.Initialize();
                };
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                list = e.OldItems as IList;

                action = (ICleanup vm) =>
                {
                    // 破棄処理の呼び出し
                    vm.Uninitialize();
                };
            }
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item is ICleanup)
                    {
                        action((ICleanup)item);
                    }
                }
            }
        }

        /// <summary>
        /// ファイナラーザー<br/>
        /// メモリの廃棄処理を行います。<br/>
        /// </summary>
        /// <param name="disposing">廃棄処理フラグ(true=直ぐに廃棄する / false=廃棄しない)</param><br/>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
                this.CollectionChanged -= this.CleanupObservableCollection_CollectionChanged;
                this.collectionViewSource = null;
            }
            this.disposed = true;
        }
    }
}
