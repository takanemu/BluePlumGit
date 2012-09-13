using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
using BluePlumGit.Interface;
using System.Windows.Data;
using System.ComponentModel;

namespace BluePlumGit.Collections
{
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

            this.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CleanupObservableCollection_CollectionChanged);
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
                this.CollectionChanged -= CleanupObservableCollection_CollectionChanged;
                this.collectionViewSource = null;
            }
            this.disposed = true;
        }
    }
}
