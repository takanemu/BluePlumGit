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
