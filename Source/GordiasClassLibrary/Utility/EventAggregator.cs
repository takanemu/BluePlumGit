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

namespace GordiasClassLibrary.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// イベント基底情報インターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    abstract internal class EventParam
    {
        /// <summary>
        /// イベントハンドラー
        /// </summary>
        public Delegate Handler { get; set; }

        /// <summary>
        /// イベント追加
        /// </summary>
        abstract public void AddEventHandler();

        /// <summary>
        /// イベント廃棄
        /// </summary>
        abstract public void RemoveHandler();
    }

    /// <summary>
    /// CLRイベント情報クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    internal class CLREventParam : EventParam
    {
        /// <summary>
        /// ターゲット
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// イベント種別
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// イベント追加
        /// </summary>
        public override void AddEventHandler()
        {
            EventInfo eventInfo = this.Target.GetType().GetEvent(this.Type);

            if (eventInfo != null)
            {
                eventInfo.AddEventHandler(this.Target, this.Handler);
            }
            else
            {
                Debug.WriteLine("警告:AddEventHandler()で、イベント種別の取得に失敗しました。type=" + this.Type);
            }
        }

        /// <summary>
        /// イベント廃棄
        /// </summary>
        /// <author>Takanori Shibuya</author>
        public override void RemoveHandler()
        {
            EventInfo eventInfo = this.Target.GetType().GetEvent(this.Type);

            eventInfo.RemoveEventHandler(this.Target, this.Handler);
        }
    }

    /// <summary>
    /// Routedイベント情報クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    internal class RoutedEventParam : EventParam
    {
        /// <summary>
        /// ターゲット
        /// </summary>
        public UIElement Target { get; set; }

        /// <summary>
        /// イベント種別
        /// </summary>
        public RoutedEvent Type { get; set; }

        /// <summary>
        /// ルーティングイベント処理済みマーク
        /// </summary>
        public bool HandledEventsToo { get; set; }

        /// <summary>
        /// イベント追加
        /// </summary>
        public override void AddEventHandler()
        {
            this.Target.AddHandler(this.Type, this.Handler, this.HandledEventsToo);
        }

        /// <summary>
        /// イベント廃棄
        /// </summary>
        public override void RemoveHandler()
        {
            this.Target.RemoveHandler(this.Type, this.Handler);
        }
    }

    /// <summary>
    /// イベント情報集約クラス
    /// イベントハンドラーを管理し簡単に解除を行えるようにする。
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public class EventAggregator
    {
        /// <summary>
        /// 登録イベントリスト
        /// </summary>
        private List<EventParam> eventList = new List<EventParam>();

        /// <summary>
        /// 廃棄済みフラグ
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EventAggregator()
        {
        }

        /// <summary>
        /// 全イベントハンドラを廃棄<br/>
        /// </summary>
        public void LeaveEventListener()
        {
            Debug.WriteLine("EventAggregator::LeaveEventListener()");

            foreach (EventParam it in this.eventList)
            {
                it.RemoveHandler();
            }
            this.eventList.Clear();
        }

        /// <summary>
        /// デストラクタ<br/>
        /// </summary>
        ~EventAggregator()
        {
            Dispose(false);
        }

        /// <summary>
        /// イベントの登録<br/>
        /// </summary>
        /// <param name="target">イベントターゲット</param><br/>
        /// <param name="type">イベント種別</param><br/>
        /// <param name="handler">イベントハンドラー</param><br/>
        public void EntryEventListener(object target, string type, Delegate handler)
        {
            CLREventParam param = new CLREventParam();

            // 同じ条件のイベントリスナーの重複登録回避を行なっている
            if (!this.IsEntry(param))
            {
                param.Target = target;
                param.Type = type;
                param.Handler = handler;
                param.AddEventHandler();

                this.eventList.Add(param);
            }
        }

        /// <summary>
        /// イベントの登録(ルーティングイベント版)<br/>
        /// </summary>
        /// <param name="target">イベントターゲット</param>
        /// <param name="type">処理するルーティング イベントの識別子</param>
        /// <param name="handler">ハンドラー実装</param>
        /// <param name="handledEventsToo">イベント データ内でルーティング イベントが処理済みとしてマークされている場合に呼び出されるようにハンドラーを登録するには true。ルーティング イベントが既に処理済みとしてマークされている場合はハンドラーを呼び出さないという既定の条件を使用してハンドラーを登録する場合は false。 既定値は、false です。 </param>
        public void EntryEventListener(UIElement target, RoutedEvent type, Delegate handler, bool handledEventsToo = false)
        {
            RoutedEventParam param = new RoutedEventParam();

            // 同じ条件のイベントリスナーの重複登録回避を行なっている
            if (!this.IsEntry(param))
            {
                param.Target = target;
                param.Type = type;
                param.Handler = handler;
                param.HandledEventsToo = handledEventsToo;
                param.AddEventHandler();

                this.eventList.Add(param);
            }
        }

        /// <summary>
        /// イベントが登録済みか判定<br/>
        /// </summary>
        /// <param name="param">イベント情報</param><br/>
        /// <returns>判定結果(true=登録済み / false=未登録)</returns><br/>
        private bool IsEntry(EventParam param)
        {
            bool result = false;

            foreach (EventParam it in this.eventList)
            {
                if (param is CLREventParam && it is CLREventParam)
                {
                    if (((CLREventParam)it).Target == ((CLREventParam)param).Target &&
                        ((CLREventParam)it).Type == ((CLREventParam)param).Type &&
                        ((CLREventParam)it).Handler == ((CLREventParam)param).Handler)
                    {
                        result = true;
                        break;
                    }
                }
                else if (param is RoutedEventParam && it is RoutedEventParam)
                {
                    if (((RoutedEventParam)it).Target == ((RoutedEventParam)param).Target &&
                        ((RoutedEventParam)it).Type == ((RoutedEventParam)param).Type &&
                        ((RoutedEventParam)it).Handler == ((RoutedEventParam)param).Handler &&
                        ((RoutedEventParam)it).HandledEventsToo == ((RoutedEventParam)param).HandledEventsToo)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ファイナラーザー<br/>
        /// メモリの廃棄処理を行います。<br/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                    // 自分がGCされたら管理しているリスナーを全て廃棄する
                    this.LeaveEventListener();
                }
            }
            this.disposed = true;
        }
    }
}
