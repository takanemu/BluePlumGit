using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GordiasClassLibrary.Executors
{
    /// <summary>
    /// コマンド実行クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    public class CommandExecutor : CommandParameterExecutorArchetype, ICommandParameter
    {
        /// <summary>
        /// 処理
        /// </summary>
        private Action<ICommandParameter> action;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="action">処理</param>
        /// <author>Takanori Shibuya.</author>
        public CommandExecutor(Action<ICommandParameter> action)
        {
            this.EntryAction(action);
        }

        /// <summary>
        /// 処理登録
        /// </summary>
        /// <param name="action">処理</param>
        /// <author>Takanori Shibuya.</author>
        public void EntryAction(Action<ICommandParameter> action)
        {
            this.action = action;
        }

        /// <summary>
        /// 処理実行
        /// </summary>
        /// <author>Takanori Shibuya.</author>
        public void Execution()
        {
            try
            {
                this.action(this);
            }
            catch (Exception e)
            {
                this.DoAbort();
            }
        }
    }
}
