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
    using Common.Library.Entitys;
    using Gordias.Library.Headquarters;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #region メインクラス
    /// <summary>
    /// 排他ファイル作成ウインドウビューモデル
    /// </summary>
    public class CreateIgnoreFileWindowViewModel : TacticsViewModel<CreateIgnoreFileWindowViewModelProperty, CreateIgnoreFileWindowViewModelCommand>
    {
        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
            this.Propertys.ProjectKindList = new List<ProjectKind>();

            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "ActionScript",
                Filename = "Actionscript.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "Android",
                Filename = "Android.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "C",
                Filename = "C.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "C++",
                Filename = "C++.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "C#",
                Filename = "CSharp.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "Java",
                Filename = "Java.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "Node.js",
                Filename = "Node.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "Objective-C",
                Filename = "Objective-C.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "Ruby on Rails",
                Filename = "Rails.gitignore",
            });
            this.Propertys.ProjectKindList.Add(new ProjectKind
            {
                Name = "Ruby",
                Filename = "Ruby.gitignore",
            });
            this.Propertys.SelectedProjectKind = this.Propertys.ProjectKindList[0];
        }
        #endregion

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            // 実行パス取得
            string baseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filepath = baseDir + @"\Assets\Gitignore\" + this.Propertys.SelectedProjectKind.Filename;

            File.Copy(filepath, this.Propertys.FolderPath + @"\.gitignore");

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

        /// <summary>
        /// フォルダーの選択
        /// </summary>
        /// <param name="message">フォルダー選択メッセージ</param>
        public void FolderSelected(FolderSelectionMessage message)
        {
            this.Propertys.FolderPath = message.Response;
        }
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class CreateIgnoreFileWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// ProjectKindList
        /// </summary>
        public virtual List<ProjectKind> ProjectKindList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ProjectKind SelectedProjectKind { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class CreateIgnoreFileWindowViewModelCommand
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
