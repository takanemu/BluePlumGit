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

namespace Gordias.Library.Headquarters
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Input;
    using Livet.Commands;

    /// <summary>
    /// ViewModelコマンドクラス
    /// </summary>
    public class TacticsCommand : Command, ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// 実行アクション
        /// </summary>
        private Action execute;

        /// <summary>
        /// 実行可能フラグ
        /// </summary>
        private bool isExecute;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">コマンドが実行するAction</param>
        public TacticsCommand(Action execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.IsExecute = true;
        }

        /// <summary>
        /// コマンドが実行可能かどうかを取得します。
        /// </summary>
        public bool CanExecute
        {
            get
            {
                return this.IsExecute;
            }
        }

        public bool IsExecute
        {
            get
            {
                return this.isExecute;
            }

            set
            {
                if (this.isExecute == value)
                {
                    return;
                }
                this.isExecute = value;
                this.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute()
        {
            this.execute();
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute;
        }

        /// <summary>
        /// コマンドが実行可能かどうかが変化した時に発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged()
        {
            var handler = Interlocked.CompareExchange(ref this.PropertyChanged, null, null);
            
            if (handler != null)
            {
                handler(this, EventArgsFactory.GetPropertyChangedEventArgs("CanExecute"));
            }
        }

        /// <summary>
        /// コマンドが実行可能かどうかが変化したことを通知します。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            this.OnPropertyChanged();
            this.OnCanExecuteChanged();
        }
    }
}
