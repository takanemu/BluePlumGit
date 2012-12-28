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
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading;

    /// <summary>
    /// Notification基底クラス
    /// </summary>
    /// <author>Takanori Shibuya</author>
    [Serializable]
    public class NotificationProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更イベント
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ変更イベントを発行する(ラムダ式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            if (!(propertyExpression.Body is MemberExpression))
            {
                throw new NotSupportedException("ラムダ式を使ってください");
            }
            var memberExpression = (MemberExpression)propertyExpression.Body;

            RaisePropertyChanged(memberExpression.Member.Name);
        }

        /// <summary>
        /// プロパティ変更イベントを発行する(名称)
        /// </summary>
        /// <param name="propertyName"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var threadSafeHandler = Interlocked.CompareExchange(ref PropertyChanged, null, null);

            if (threadSafeHandler != null)
            {
                var e = EventArgsFactory.GetPropertyChangedEventArgs(propertyName);
                threadSafeHandler(this, e);
            }
        }
    }
}
