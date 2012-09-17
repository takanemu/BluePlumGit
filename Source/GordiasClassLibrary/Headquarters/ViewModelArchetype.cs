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

namespace GordiasClassLibrary.Headquarters
{
    using System;
    using System.Xml.Serialization;
    using GordiasClassLibrary.Interfaces;

    /// <summary>
    /// ViewModel基底クラス
    /// </summary>
    /// <typeparam name="PropertyType">プロパティクラス</typeparam>
    /// <typeparam name="CommandType">コマンドクラス</typeparam>
    /// <author>Takanori Shibuya</author>
    [Serializable]
    public abstract class ViewModelArchetype<PropertyType, CommandType> : NotificationProvider, IDisposable, ITacticsCommand<CommandType>
        where PropertyType : class
        where CommandType : new()
    {
        [NonSerialized]
        private bool disposed = false;

        /// <summary>
        /// コマンドプロパティ
        /// </summary>
        [XmlIgnore]
        public CommandType Commands { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ViewModelArchetype()
        {
        }

        /// <summary>
        /// ViewModelリソース開放処理
        /// </summary>
        private void DisposedOverride()
        {
        }

        /// <summary>
        /// このインスタンスによって使用されているすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }
            this.disposed = true;
            this.DisposedOverride();
        }
    }
}
