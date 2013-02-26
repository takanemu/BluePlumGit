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

namespace Gordias.Library.Executors
{
    using Gordias.Library.Events;

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
