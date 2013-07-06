﻿#region Apache License
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

namespace Gitlab
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ユーザークラスファクトリー
    /// </summary>
    internal class UsersFactory
    {
        /// <summary>
        /// ユーザークラスの生成
        /// </summary>
        /// <param name="json">JSONデータ</param>
        /// <returns>ユーザークラス</returns>
        internal static User Create(string json)
        {
            return JsonConvert.DeserializeObject<User>(json);
        }

        /// <summary>
        /// ユーザークラスリストの生成
        /// </summary>
        /// <param name="json">JSONデータ</param>
        /// <returns>ユーザークラス</returns>
        internal static List<User> Creates(string json)
        {
            return JsonConvert.DeserializeObject<List<User>>(json);
        }
    }
}
