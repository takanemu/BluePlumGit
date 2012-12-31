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
    using System.Windows;
    using System.Xml.Serialization;
    using Gordias.Library.Headquarters;
    using Gordias.Library.Interfaces;
    using Gordias.Library.Utility;
    using Livet;
    using System.ComponentModel;

    /// <summary>
    /// ViewModel基底クラス
    /// </summary>
    /// <typeparam name="PropertyType">プロパティクラス定義</typeparam>
    /// <typeparam name="CommandType">コマンドクラス定義</typeparam>
    [Serializable]
    public class TacticsViewModel<PropertyType, CommandType> : ViewModel, ITacticsCommand<CommandType>
        where PropertyType : class
        where CommandType : new()
    {
        public FrameworkElement Parent { get; set; }

        /// <summary>
        /// コマンドプロパティ
        /// </summary>
        [XmlIgnore]
        public CommandType Commands { get; set; }

        /// <summary>
        /// CLRプロパティ
        /// </summary>
        [XmlIgnore]
        public PropertyType Propertys { get; set; }

        /// <summary>
        /// イベント管理
        /// </summary>
        [XmlIgnore]
        public EventAggregator EventSweeper { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TacticsViewModel()
            : base()
        {
            this.EventSweeper = new EventAggregator();
            this.Propertys = NotifyPropertyChangedHelper<PropertyType>.Create();
            this.Commands = new CommandType();

            CommandAttribute.Construction(this);
            MessageReceiveAttribute.Construction(this, EventSweeper);
            LogisticsPropertyChangedAttribute.Construction(this, EventSweeper);
        }

        /// <summary>
        /// グローバル値の設定
        /// </summary>
        /// <typeparam name="DataType">パラメーターデータタイプ</typeparam>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        protected void SetValue<DataType>(Enum key, DataType value)
        {
            DataLogistics.Instance.SetValue(key, value);
        }

        /// <summary>
        /// グローバル値の取得
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        protected object GetValue(Enum key)
        {
            return DataLogistics.Instance.GetValue(key);
        }

        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <typeparam name="DataType">パラメーターデータタイプ</typeparam>
        /// <param name="address">通信先</param>
        /// <param name="parameter">パラメーター</param>
        protected void RequestTo<DataType>(Enum address, DataType parameter)
        {
            Tweety.Instance.RequestTo(address, parameter);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="parent">親コンポーネント</param>
        public void Initialize(FrameworkElement parent)
        {
            this.Parent = parent;

            if (this.Parent is Window)
            {
                this.EventSweeper.EntryEventListener(this.Parent, "Closed", new EventHandler(ClosedHandlerOverride));
                this.EventSweeper.EntryEventListener(this.Parent, "Closing", new CancelEventHandler(ClosingHandlerOverride));
            }
            this.EventSweeper.EntryEventListener(this.Parent, "Loaded", new RoutedEventHandler(LoadedHandlerOverride));
            this.EventSweeper.EntryEventListener(this.Parent, "Unloaded", new RoutedEventHandler(UnloadedHandlerOverride));
        }

        /// <summary>
        /// Loadedイベント処理
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        protected virtual void LoadedHandlerOverride(object sender, RoutedEventArgs e)
        {
            // overrideして利用します。
        }

        /// <summary>
        /// Unloadedイベント処理
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        protected virtual void UnloadedHandlerOverride(object sender, RoutedEventArgs e)
        {
            this.EventSweeper.LeaveEventListener();
        }

        /// <summary>
        /// Closingイベント処理
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        protected virtual void ClosingHandlerOverride(object sender, CancelEventArgs e)
        {
            // overrideして利用します。
        }

        /// <summary>
        /// Closedイベント処理
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        protected virtual void ClosedHandlerOverride(object sender, EventArgs e)
        {
            // overrideして利用します。
        }



    }
}
