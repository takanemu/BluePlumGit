
namespace GordiasClassLibrary.Executors
{
    using LegionMVVMLibrary.Events;

    /// <summary>
    /// コマンド実行基底クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public abstract class CommandExecutorArchetype : INotifyComplete, ICommandComplete, ICommandAbort
    {
        /// <summary>
        /// 更新イベントハンドラ
        /// </summary>
        public event CompleteEventHandler Complete;

        /// <summary>
        /// アボート発火
        /// </summary>
        /// <author>Takanori Shibuya.</author>
        public void DoAbort()
        {
            this.OnComplete(true);
        }

        /// <summary>
        /// 完了イベント発火
        /// </summary>
        /// <author>Takanori Shibuya.</author>
        public void DoComplete()
        {
            this.OnComplete();
        }

        /// <summary>
        /// 完了イベント発火
        /// </summary>
        /// <param name="isAbort">trueならアボート</param>
        /// <author>Takanori Shibuya</author>
        protected void OnComplete(bool isAbort = false)
        {
            CompleteEventArgs ea = new CompleteEventArgs();

            ea.IsAbort = isAbort;

            if (this.Complete != null)
            {
                this.Complete(this, ea);
            }
        }
    }

    /// <summary>
    /// パラメーター付きコマンド実行基底クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public abstract class CommandParameterExecutorArchetype : CommandExecutorArchetype, ICommandParameter
    {
        /// <summary>
        /// オプションデータ
        /// </summary>
        private object data = null;

        /// <summary>
        /// オプションデータ設定
        /// </summary>
        /// <param name="data">データ</param>
        /// <returns>メソッドチェーン</returns>
        /// <author>Takanori Shibuya.</author>
        public ICommandParameter SetData(object data)
        {
            this.data = data;
            return (ICommandParameter)this;
        }

        /// <summary>
        /// オプションデータ取得
        /// </summary>
        /// <returns>データ</returns>
        public object GetData()
        {
            return this.data;
        }
    }
}
