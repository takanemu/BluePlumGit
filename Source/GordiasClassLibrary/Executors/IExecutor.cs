
namespace GordiasClassLibrary.Executors
{
    using LegionMVVMLibrary.Events;

    /// <summary>
    /// 処理実行インターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public interface IExecutor
    {
        /// <summary>
        /// 処理実行
        /// </summary>
        void Execution();
    }

    /// <summary>
    /// 処理実行＆完了イベントインターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public interface INotifyCompleteExecutor : INotifyComplete, IExecutor
    {
    }

    /// <summary>
    /// パラメーターインターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public interface ICommandParameter
    {
        /// <summary>
        /// オプションデータ設定
        /// </summary>
        /// <param name="data">データ</param>
        /// <returns>メソッドチェーン</returns>
        ICommandParameter SetData(object data);

        /// <summary>
        /// オプションデータ取得
        /// </summary>
        /// <returns>データ</returns>
        object GetData();
    }

    /// <summary>
    /// イベント完了インターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public interface ICommandComplete
    {
        /// <summary>
        /// 完了イベント発火
        /// </summary>
        void DoComplete();
    }

    /// <summary>
    /// アボートインターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public interface ICommandAbort
    {
        /// <summary>
        /// アボート発火
        /// </summary>
        void DoAbort();
    }

    /// <summary>
    /// コマンド実装インターフェース
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public interface ICommandExecutor : ICommandComplete, ICommandAbort, ICommandParameter, INotifyCompleteExecutor
    {
    }
}
