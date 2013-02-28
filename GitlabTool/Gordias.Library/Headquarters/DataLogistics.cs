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

namespace Gordias.Library.Headquarters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using Gordias.Library.Utility;

    /// <summary>
    /// 大域変数管理クラスで入れ子管理するためのインターフェース<br/>
    /// </summary>
    public interface IEntityNotifyProperty : INotifyPropertyChanged
    {
        /// <summary>
        /// キー
        /// </summary>
        Enum Key { get; set; }
    }

    public class DataLogistics
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static readonly DataLogistics Instance = new DataLogistics();

        /// <summary>
        /// 値の格納テーブル
        /// </summary>
        private Dictionary<Enum, object> map = new Dictionary<Enum, object>();

        /// <summary>
        /// シングルトン化コード
        /// </summary>
        static DataLogistics()
        {
        }
        
        /// <summary>
        /// シングルトン化コード
        /// </summary>
        private DataLogistics()
        {
        }

        /// <summary>
        /// 更新イベントデリゲート
        /// </summary>
        /// <param name="sender">イベント元</param><br/>
        /// <param name="e">パラメーター</param><br/>
        public delegate void PropertyChangeEventHandler(object sender, PropertyChangeEventArgs e);

        public delegate void LogisticsPropertyChangeHandler(object data, PropertyChangeEventArgs e);

        /// <summary>
        /// 更新イベントハンドラ
        /// </summary>
        public event PropertyChangeEventHandler Change;

        /// <summary>
        /// データの格納<br/>
        /// INotifyPropertyChangedインターフェースを実装したクラスは<br/>
        /// 更新イベント発行対象外となる。更新イベントを発行する場合には<br/>
        /// クラス内で、PropertyChangedイベントを発行すること。<br/>
        /// </summary>
        /// <param name="key">データ格納キー</param><br/>
        /// <param name="value">格納データ</param><br/>
        public void SetValue(Enum key, object value)
        {
            if (value is IEntityNotifyProperty)
            {
                // IEntityNotifyPropertyインターフェースを実装したクラスは、更新でイベントの発行は行わない。
                IEntityNotifyProperty ienp = (IEntityNotifyProperty)value;

                if (this.map.ContainsKey(key))
                {
                    // 既に同じデータキーで登録がある場合には、イベントハンドラーを付け替える。
                    IEntityNotifyProperty old = (IEntityNotifyProperty)this.map[key];

                    // イベントハンドラを解除
                    old.PropertyChanged -= this.PropertyChangedHandler;

                    // イベントハンドラを再追加
                    ienp.PropertyChanged += new PropertyChangedEventHandler(this.PropertyChangedHandler);

                    // キーを設定
                    ienp.Key = key;

                    // 値を上書き
                    this.map[key] = value;
                }
                else
                {
                    // 新規にデータを格納する場合
                    // イベントハンドラを追加
                    ienp.PropertyChanged += new PropertyChangedEventHandler(this.PropertyChangedHandler);

                    // キーを設定
                    ienp.Key = key;

                    // キーと値を追加
                    this.map.Add(key, value);
                }
            }
            else
            {
                // 通常オブジェクトの設定では、更新イベントの発行は行われる。
                PropertyChangeEventArgs e = new PropertyChangeEventArgs();

                if (this.map.ContainsKey(key))
                {
                    // 更新
                    e.Change = true;

                    // 変更前の値を退避
                    e.Old = this.map[key];

                    // 値を上書き
                    this.map[key] = value;
                }
                else
                {
                    // 新規登録
                    e.Change = false;

                    // キーと値を追加
                    this.map.Add(key, value);
                }
                e.Key = key;
                e.Latest = value;

                // 値が更新された時だけイベントを発生する。
                if (e.Old != null && e.Old.Equals(e.Latest) == false)
                {
                    this.OnPropertyChange(e);    // イベント発火
                }
                else if (e.Latest != null && e.Latest.Equals(e.Old) == false)
                {
                    this.OnPropertyChange(e);    // イベント発火
                }
            }
        }

        /// <summary>
        /// プロパティ更新処理ハンドラー<br/>
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">パラメーター</param>
        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangeEventArgs ne = new PropertyChangeEventArgs();

            // 更新フラグ
            ne.Change = true;

            // キー
            ne.Key = ((IEntityNotifyProperty)sender).Key;

            // プロパティ名
            ne.PropertyName = e.PropertyName;

            Type t = sender.GetType();

            // 更新値
            ne.Latest = t.InvokeMember(e.PropertyName, BindingFlags.GetProperty, null, sender, null);

            this.OnPropertyChange(ne);    // イベント発火
        }

        /// <summary>
        /// イベントの発生<br/>
        /// </summary>
        /// <param name="e">データ変更イベントパラメーター</param><br/>
        protected virtual void OnPropertyChange(PropertyChangeEventArgs e)
        {
            if (this.Change != null)
            {
                this.Change(this, e);
            }
        }

        /// <summary>
        /// 格納データの取得<br/>
        /// </summary>
        /// <param name="key">データ取得キー</param><br/>
        /// <returns>格納データ</returns>
        public object GetValue(Enum key)
        {
            return this.map[key];
        }
    }

    /// <summary>
    /// 大域変数管理データ変更イベントパラメータークラス<br/>
    /// </summary>
    public class PropertyChangeEventArgs : EventArgs
    {
        /// <summary>
        /// 更新フラグ(true=更新/false=新規)
        /// </summary>
        public bool Change { get; set; }

        /// <summary>
        /// データ種別(格納キー)
        /// </summary>
        public Enum Key { get; set; }

        /// <summary>
        /// 更新前データ値
        /// </summary>
        public object Old { get; set; }

        /// <summary>
        /// 更新後データ値
        /// </summary>
        public object Latest { get; set; }

        /// <summary>
        /// プロパティ名(INotifyPropertyEntity)の場合のみ
        /// </summary>
        public string PropertyName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LogisticsPropertyChangedAttribute : System.Attribute
    {
        /// <summary>
        /// キー
        /// </summary>
        private Enum dataKey;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">変数識別キー</param>
        public LogisticsPropertyChangedAttribute(object value)
        {
            this.dataKey = value as Enum;
        }
        
        /// <summary>
        /// 属性パラメーター
        /// </summary>
        public Enum DataKey
        {
            get { return this.dataKey; }
        }

        /// <summary>
        /// 大域変数管理のイベントリスナーを構築する。<br/>
        /// イベントリスナーの構築は、属性タグによって指定する。<br/>
        /// </summary>
        /// <param name="target">構築対象クラス</param><br/>
        /// <param name="sweep">イベント管理クラス</param><br/>
        public static void Construction(object target, EventAggregator sweep)
        {
            Type type = target.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.IsDefined(typeof(LogisticsPropertyChangedAttribute), false))
                {
                    // 検査キーを取得する
                    Enum ekey = LogisticsPropertyChangedAttribute.GetDataKey(target, method.Name);

                    // ekeyの更新があった時だけ更新イベントを返す
                    if (ekey != null)
                    {
                        AnonymousDispather ad = new AnonymousDispather();

                        ad.DataKey = ekey;

                        Type handlerType = typeof(DataLogistics.LogisticsPropertyChangeHandler);

                        ad.Method = Delegate.CreateDelegate(handlerType, target, method);
                        
                        sweep.EntryEventListener(DataLogistics.Instance, "Change", new DataLogistics.PropertyChangeEventHandler(ad.ChangeHandler));
                    }
                }
            }
        }

        /// <summary>
        /// メソッドの属性パラメーターを取得する。
        /// </summary>
        /// <param name="target">メソッドを持ったクラス</param>
        /// <param name="name">メソッド名</param>
        /// <returns>属性パラメーター(列挙型)、取得できない場合には、nullを返す。</returns>
        private static Enum GetDataKey(object target, string name)
        {
            Type t = target.GetType();
            MethodInfo minfo = t.GetMethod(name);
            LogisticsPropertyChangedAttribute[] items = (LogisticsPropertyChangedAttribute[])minfo.GetCustomAttributes(typeof(LogisticsPropertyChangedAttribute), false);

            if (items.Length == 0)
            {
                return null;
            }
            LogisticsPropertyChangedAttribute my = (LogisticsPropertyChangedAttribute)items[0];

            return my.DataKey;
        }

        /// <summary>
        /// 無名イベント中継クラス<br/>
        /// </summary>
        private class AnonymousDispather
        {
            /// <summary>
            /// キー
            /// </summary>
            public Enum DataKey { get; set; }

            /// <summary>
            /// キーが合致した時に呼び出されるデリゲート
            /// </summary>
            public Delegate Method { get; set; }

            /// <summary>
            /// イベントハンドラ<br/>
            /// </summary>
            /// <param name="sender">イベント元</param>
            /// <param name="e">パラメーター</param>
            public void ChangeHandler(object sender, PropertyChangeEventArgs e)
            {
                // 同一のenum型かチェックする
                if (e.Key.GetType() == this.DataKey.GetType())
                {
                    if (this.DataKey.CompareTo(e.Key) == 0)
                    {
                        // Debug.WriteLine("同じenum = {0}", e.key);
                        this.Method.DynamicInvoke(e.Latest, e);
                    }
                    else
                    {
                        // Debug.WriteLine("違うenum {0} != {1}", DataKey, e.key);
                    }
                }
            }
        }
    }
}
