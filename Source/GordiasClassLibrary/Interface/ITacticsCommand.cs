using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GordiasClassLibrary.Interfaces
{
    /// <summary>
    /// コマンド定義実装クラスインターフェイス
    /// </summary>
    /// <typeparam name="CommandType">コマンド定義クラス</typeparam>
    public interface ITacticsCommand<CommandType>
    {
        /// <summary>
        /// コマンド定義クラス用プロパティ
        /// </summary>
        CommandType Commands { get; set; }
    }
}
