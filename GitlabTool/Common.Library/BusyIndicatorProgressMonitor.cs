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

namespace Commno.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NGit;
    using System.Threading;

    /// <summary>
    /// コマンドモニタークラス
    /// </summary>
    public class BusyIndicatorProgressMonitor : BatchingProgressMonitor
    {
        public BusyIndicatorProgressMonitor()
        {
        }

        protected override void OnUpdate(string taskName, int workCurr)
        {
        }

        protected override void OnUpdate(string taskName, int cmp, int totalWork, int pcnt)
        {
            this.UpdateAction(taskName, cmp, totalWork, pcnt);
        }

        protected override void OnEndTask(string taskName, int workCurr)
        {
        }

        protected override void OnEndTask(string taskName, int cmp, int totalWork, int pcnt)
        {
        }

        public Action StartAction { get; set; }
        
        public Action<string, int> BeginTaskAction { get; set; }
        
        public Action<string, int, int, int> UpdateAction { get; set; }
        
        public Action CompleteAction { get; set; }
    }
}
