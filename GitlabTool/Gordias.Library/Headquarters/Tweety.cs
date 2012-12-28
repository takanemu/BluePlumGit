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

namespace Gordias.Library.Headquarters
{
    using System;
    using System.Reflection;
    using Gordias.Library.Utility;

    /// <summary>
    /// ビューモデル間通信クラス<br/>
    /// </summary>
    /// <author>Takanori Shibuya.</author>
    public class Tweety
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static readonly Tweety Instance = new Tweety();

        /// <summary>
        /// シングルトン化コード
        /// </summary>
        static Tweety()
        {
        }

        /// <summary>
        /// シングルトン化コード
        /// </summary>
        private Tweety()
        {
        }

        /// <summary>
        /// 更新イベントデリゲート
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        public delegate void MessageReceiveEventHandler(object sender, MessageReceiveEventArgs e);

        public delegate void MessageReceiveHandler(object parameter);

        /// <summary>
        /// 更新イベントハンドラ
        /// </summary>
        public event MessageReceiveEventHandler Receive;

        /// <summary>
        /// リクエスト送信<br/>
        /// </summary>
        /// <param name="address">宛先</param><br/>
        /// <param name="parameter">パラメーター</param><br/>
        /// <author>Takanori Shibuya.</author>
        public void RequestTo<RequestType>(Enum address, RequestType parameter)
        {
            MessageReceiveEventArgs e = new MessageReceiveEventArgs();

            e.Address = address;
            e.Data = parameter;

            this.OnMessageReceive(e);
        }

        /// <summary>
        /// イベントの発生<br/>
        /// </summary>
        /// <param name="e">メッセージ通信イベントパラメーター</param><br/>
        /// <author>Takanori Shibuya.</author>
        protected virtual void OnMessageReceive(MessageReceiveEventArgs e)
        {
            if (this.Receive != null)
            {
                this.Receive(this, e);
            }
        }
    }

    /// <summary>
    /// メッセージ通信イベントパラメータークラス<br/>
    /// </summary>
    /// <author>Takanori Shibuya.</author>
    public class MessageReceiveEventArgs : EventArgs
    {
        /// <summary>
        /// 宛先
        /// </summary>
        public Enum Address { get; set; }

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Data { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MessageReceiveAttribute : System.Attribute
    {
        /// <summary>
        /// キー
        /// </summary>
        private Enum dataKey;

        /// <summary>
        /// コンストラクタ<br/>
        /// </summary>
        /// <param name="value">変数識別キー</param><br/>
        /// <author>Takanori Shibuya.</author>
        public MessageReceiveAttribute(object value)
        {
            this.dataKey = value as Enum;
        }
        
        /// <summary>
        /// 属性パラメーター<br/>
        /// </summary>
        /// <author>Takanori Shibuya.</author>
        public Enum DataKey
        {
            get { return this.dataKey; }
        }

        /// <summary>
        /// CommunicationProxyのイベントリスナーを構築する。<br/>
        /// イベントリスナーの構築は、属性タグによって指定する。<br/>
        /// </summary>
        /// <param name="target">構築対象クラス</param><br/>
        /// <param name="sweep">イベント管理クラス</param><br/>
        /// <author>Takanori Shibuya.</author>
        public static void Construction(object target, EventAggregator sweep)
        {
            Type type = target.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.IsDefined(typeof(MessageReceiveAttribute), false))
                {
                    // 検査キーを取得する
                    Enum ekey = GetDataKey(target, method.Name);

                    // ekeyの更新があった時だけ更新イベントを返す
                    if (ekey != null)
                    {
                        AnonymousDispather ad = new AnonymousDispather();

                        ad.DataKey = ekey;
                        
                        Type handlerType = typeof(Tweety.MessageReceiveHandler);

                        // デリゲートを作成する
                        ad.Method = Delegate.CreateDelegate(handlerType, target, method);
 
                        sweep.EntryEventListener(Tweety.Instance, "Receive", new Tweety.MessageReceiveEventHandler(ad.ReceiveHandler));
                    }
                }
            }
        }

        /// <summary>
        /// メソッドの属性パラメーターを取得する。<br/>
        /// </summary>
        /// <param name="target">メソッドを持ったクラス</param><br/>
        /// <param name="name">メソッド名</param><br/>
        /// <returns>属性パラメーター(列挙型)、取得できない場合には、nullを返す。</returns><br/>
        /// <author>Takanori Shibuya.</author>
        private static Enum GetDataKey(object target, string name)
        {
            Type t = target.GetType();
            MethodInfo minfo = t.GetMethod(name);
            MessageReceiveAttribute[] items = (MessageReceiveAttribute[])minfo.GetCustomAttributes(typeof(MessageReceiveAttribute), false);

            if (items.Length == 0)
            {
                return null;
            }
            MessageReceiveAttribute my = (MessageReceiveAttribute)items[0];

            return my.DataKey;
        }

        /// <summary>
        /// 無名イベント中継クラス<br/>
        /// </summary>
        /// <author>Takanori Shibuya.</author>
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
            /// 受信ハンドラー<br/>
            /// </summary>
            /// <param name="sender">イベント元</param><br/>
            /// <param name="e">パラメーター</param><br/>
            /// <author>Takanori Shibuya.</author>
            public void ReceiveHandler(object sender, MessageReceiveEventArgs e)
            {
                Type t1 = this.DataKey.GetType();
                Type t2 = e.Address.GetType();

                // 同一の型のときのみ比較をする。
                if (t1 == t2)
                {
                    if (this.DataKey.CompareTo(e.Address) == 0)
                    {
                        this.Method.DynamicInvoke(e.Data);
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
