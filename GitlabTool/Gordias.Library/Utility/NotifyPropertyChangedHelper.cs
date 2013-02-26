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

namespace Gordias.Library.Utility
{
    using System.Collections.Generic;
    using System.Reflection;
    using Castle.DynamicProxy;
    using Gordias.Library.Interfaces;

    /// <summary>
    /// NotifyPropertyChangedを自動発行するプロパティクラスを生成する
    /// </summary>
    /// <typeparam name="Type">クラスタイプ</typeparam>
    public class NotifyPropertyChangedHelper<Type> where Type : class
    {
        /// <summary>
        /// プロクシジェネレーター
        /// </summary>
        private static ProxyGenerator generator;

        /// <summary>
        /// インターセプター
        /// </summary>
        private static PropertyInterceptor interceptor;

        /// <summary>
        /// クラスの生成
        /// </summary>
        /// <returns>生成されたクラスの参照</returns>
        public static Type Create()
        {
            if (generator == null)
            {
                generator = new ProxyGenerator();          // プロクシ生成クラスの生成
            }
            if (interceptor == null)
            {
                interceptor = new PropertyInterceptor();     // インターセプターの生成
            }
            Type result = generator.CreateClassProxy<Type>(interceptor);

            return result;
        }

        /// <summary>
        /// インターセプタークラス
        /// </summary>
        internal class PropertyInterceptor : IInterceptor
        {
            /// <summary>
            /// メソッドの実行の直前に呼び出される
            /// </summary>
            /// <param name="invocation">IInvocationクラス</param>
            public void Intercept(IInvocation invocation)
            {
                MethodBase method = invocation.Method;
                string propName = method.Name.Substring(4);

                if (method.Name.StartsWith("set_"))
                {
                    PropertyInfo info = invocation.InvocationTarget.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                    // アクセサ実行前の値を退避
                    object preValue = info.GetValue(invocation.InvocationTarget, null);
                    
                    // アクセサを実行
                    invocation.Proceed();
                    
                    // アクセサ実行後の値を退避
                    object postValue = info.GetValue(invocation.InvocationTarget, null);

                    if (EqualityComparer<object>.Default.Equals(preValue, postValue))
                    {
                        return;
                    }
                    ((IRaisePropertyChanged)invocation.InvocationTarget).OnRaisePropertyChanged(propName);
                }
                invocation.Proceed();
            }
        }
    }
}
