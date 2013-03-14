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

namespace Common.Library.Entitys
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// クローンエンティティ
    /// </summary>
    public class CloneEntity
    {
        /// <summary>
        /// プロジェクト名
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// アドレス
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// パス
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// 認証フラグ
        /// </summary>
        public bool IsCredential { get; set; }
        
        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// パスワード
        /// </summary>
        public string PassWord { get; set; }
    }
}
