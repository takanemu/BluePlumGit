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

namespace Gordias.Library.Executors
{
    using System;

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
            catch (Exception)
            {
                this.DoAbort();
            }
        }
    }
}
