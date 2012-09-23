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

namespace BluePlumGit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GordiasClassLibrary.Interface;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Entitys;

    /// <summary>
    /// ブランチの削除
    /// </summary>
    public class RemoveBranchWindowViewModel : TacticsViewModel<RemoveBranchWindowViewModelProperty, RemoveBranchWindowViewModelCommand>, IWindowResult
    {
        /// <summary>
        /// 戻り値
        /// </summary>
        public WindowResultEntity Responce { get; set; }
    }

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class RemoveBranchWindowViewModelProperty : TacticsProperty
    {
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class RemoveBranchWindowViewModelCommand
    {
    }
    #endregion
}
