//-----------------------------------------------------------------------
// <copyright>
//     takanori shibuya.
// </copyright>
//-----------------------------------------------------------------------

namespace GordiasClassLibrary.Headquarters
{
    using System;
    using System.Xml.Serialization;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interfaces;
    using GordiasClassLibrary.Utility;
    using Livet;

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

        // TODO: OpenDialog
        protected void OpenDialog()
        {
        }
    }
}
