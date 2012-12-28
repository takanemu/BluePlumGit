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

namespace Gordias.Library.Headquarters
{
    using System;
    using System.Reflection;
    using Gordias.Library.Interfaces;

    /// <summary>
    /// Livetコマンド属性定義クラス
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : System.Attribute
    {
        /// <summary>
        /// 構築処理
        /// </summary>
        /// <typeparam name="CommandType">コマンド実装クラスタイプ</typeparam>
        /// <param name="target">構築ターゲットクラス</param>
        public static void Construction<CommandType>(ITacticsCommand<CommandType> target)
        {
            var commands = target.Commands;
            Type targetType = target.GetType();
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                if (method.IsDefined(typeof(CommandAttribute), false))
                {
                    var name = method.Name;

                    Type commandsType = commands.GetType();
                    PropertyInfo propertys = commandsType.GetProperty(name);
                    MethodInfo commandMethod = method;

                    propertys.SetValue(
                        commands,
                        new TacticsCommand(() =>
                        {
                            commandMethod.Invoke(target, null);
                        }),
                        null);
                }
            }
        }
    }
}
