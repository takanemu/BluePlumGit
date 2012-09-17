
namespace GordiasClassLibrary.Executors
{
    using System.Collections.Generic;
    using LegionMVVMLibrary.Events;

    /// <summary>
    /// 順次実行管理クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public class SerialExecutor : CommandExecutorArchetype, INotifyCompleteExecutor
    {
        /// <summary>
        /// 処理リスト
        /// </summary>
        private List<INotifyCompleteExecutor> list = new List<INotifyCompleteExecutor>();

        /// <summary>
        /// 処理の登録
        /// </summary>
        /// <param name="execute">処理クラス</param>
        /// <author>Takanori Shibuya</author>
        public void EntryAction(INotifyCompleteExecutor execute)
        {
            execute.Complete += new CompleteEventHandler(this.CompleteEventHandler);
            this.list.Add(execute);
        }

        /// <summary>
        /// 処理実行
        /// </summary>
        /// <author>Takanori Shibuya</author>
        public void Execution()
        {
            if (this.list.Count > 0)
            {
                this.list[0].Execution();
            }
        }

        /// <summary>
        /// 完了イベントハンドラー
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        /// <author>Takanori Shibuya</author>
        private void CompleteEventHandler(object sender, CompleteEventArgs e)
        {
            if (e.IsAbort)
            {
                // 中断処理
                foreach (INotifyComplete it in this.list)
                {
                    it.Complete -= this.CompleteEventHandler;
                }
                this.list.Clear();
                this.OnComplete(true);
            }
            else
            {
                // 処理完了
                ((INotifyComplete)sender).Complete -= this.CompleteEventHandler;
                this.list.Remove((INotifyCompleteExecutor)sender);

                if (this.list.Count == 0)
                {
                    // 全処理が完了したことを通知
                    this.OnComplete();
                }
                else
                {
                    // 次の処理を実行
                    this.list[0].Execution();
                }
            }
        }
    }
}
