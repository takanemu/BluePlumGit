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

namespace Gordias.Library.Events
{
    using System;

    /// <summary>
    /// 完了イベント実装インターフェース
    /// </summary>
    /// <author>Takanori Shibuya.</author>
    public interface INotifyComplete
    {
        /// <summary>
        /// 完了イベント
        /// </summary>
        event CompleteEventHandler Complete;
    }

    /// <summary>
    /// 完了イベントデリゲート
    /// </summary>
    /// <param name="sender">イベント元</param>
    /// <param name="e">パラメーター</param>
    /// <author>Takanori Shibuya.</author>
    public delegate void CompleteEventHandler(object sender, CompleteEventArgs e);

    /// <summary>
    /// 完了イベントパラメーター
    /// </summary>
    /// <author>Takanori Shibuya.</author>
    public class CompleteEventArgs : EventArgs
    {
        /// <summary>
        /// アボートフラグ
        /// </summary>
        public bool IsAbort { get; set; }
    }
}
