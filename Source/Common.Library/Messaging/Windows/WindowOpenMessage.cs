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

namespace BluePlumGit.Messaging.Windows
{
    using System.Windows;
    using BluePlumGit.Entitys;
    using BluePlumGit.Enums;
    using Livet.Messaging;

    /// <summary>
    /// ウインドウオープンメッセージ
    /// </summary>
    public class WindowOpenMessage : ResponsiveInteractionMessage<RepositoryEntity>
    {
        /// <summary>
        /// ウインドウ種別
        /// </summary>
        public static readonly DependencyProperty WindowTypeProperty =
            DependencyProperty.Register("WindowType", typeof(WindowTypeEnum), typeof(WindowOpenMessage), new UIPropertyMetadata(null));

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOpenMessage()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="messageKey"></param>
        public WindowOpenMessage(string messageKey)
            : base(messageKey)
        {
        }

        /// <summary>
        /// ウインドウ種別
        /// </summary>
        public WindowTypeEnum WindowType
        {
            get
            {
                return (WindowTypeEnum)GetValue(WindowTypeProperty);
            }
            
            set
            {
                SetValue(WindowTypeProperty, value);
            }
        }

        /// <summary>
        /// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
        /// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
        /// </summary>
        /// <returns>自身の新しいインスタンス</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new WindowOpenMessage(MessageKey);
        }
    }
}
