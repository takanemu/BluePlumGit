#region License
// <copyright>
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
// </copyright>
#endregion

namespace Common.Library.Entitys
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// リポジトリ登録種別
    /// </summary>
    public enum InitializeRepositoryEnum
    {
        /// <summary>
        /// 登録のみ
        /// </summary>
        EntryOnly,

        /// <summary>
        /// 初期化後に登録
        /// </summary>
        InitializeAndEntry,
    }

    /// <summary>
    /// リポジトリ登録エンティティ
    /// </summary>
    public class InitializeRepositoryEntity
    {
        /// <summary>
        /// 登録種別
        /// </summary>
        public InitializeRepositoryEnum Mode { get; set; }

        /// <summary>
        /// リポジトリ情報
        /// </summary>
        public RepositoryEntity Entity { get; set; }
    }
}
