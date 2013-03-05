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

namespace GitlabTool.ViewModels
{
    using Gordias.Library.Headquarters;
    using Livet.Messaging.Windows;
    using log4net;
    using NGit;
    using NGit.Api;
    using NGit.Storage.File;
    using Sharpen;
    using System.Collections.Generic;
    using System.Diagnostics;

    #region メインクラス
    /// <summary>
    /// ブランチ削除ウインドウビューモデル
    /// </summary>
    public class BranchRemoveWindowViewModel : TacticsViewModel<BranchRemoveWindowViewModelProperty, BranchRemoveWindowViewModelCommand>
    {
        /// <summary>
        /// ログ
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
            FilePath path = new FilePath(@"", @".git");
            FileRepository db = new FileRepository(path);
            Git git = new Git(db);
            IList<Ref> list = git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();

            foreach (Ref branch in list)
            {
                string name = branch.GetName();
                Debug.WriteLine("name = " + name);
            }
        }
        #endregion

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        #region Cancelボタン処理
        /// <summary>
        /// Cancelボタン処理
        /// </summary>
        [Command]
        private void CancelButton()
        {
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class BranchRemoveWindowViewModelProperty : TacticsProperty
    {
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class BranchRemoveWindowViewModelCommand
    {
        /// <summary>
        /// OkButtonコマンド
        /// </summary>
        public TacticsCommand OkButton { get; private set; }

        /// <summary>
        /// CancelButtonコマンド
        /// </summary>
        public TacticsCommand CancelButton { get; private set; }
    }
    #endregion

}
