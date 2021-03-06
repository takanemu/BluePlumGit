﻿#region License
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

namespace Common.Library.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NGit;

    /// <summary>
    /// Git Config Utility
    /// </summary>
    public class GitConfigUtility
    {
        /// <summary>
        /// GitConfigの検査
        /// </summary>
        /// <param name="config">コンフィグクラス</param>
        /// <param name="func">コールバック関数</param>
        public static void CheckConfig(StoredConfig config, Func<string, string, string, bool> func)
        {
            foreach (string section in config.GetSections())
            {
                foreach (string name in config.GetNames(section))
                {
                    if (func(section, string.Empty, name) == true)
                    {
                        return;
                    }
                }
                foreach (string subsection in config.GetSubsections(section))
                {
                    foreach (string name in config.GetNames(section, subsection))
                    {
                        if (func(section, string.Empty, name) == true)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
