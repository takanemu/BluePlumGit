#region License
// <copyright>
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
// </copyright>
#endregion

namespace GitlabTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Gitlab;
    using Gordias.Library.Entitys;
    using Gordias.Library.Headquarters;
    using Gordias.Library.Interface;
    using Livet.Messaging.IO;
    using Livet.Messaging.Windows;
    using log4net;

    #region メインクラス
    /// <summary>
    /// クローンウインドウビューモデル
    /// </summary>
    public class CloneRepositoryWindowViewModel : TacticsViewModel<CloneRepositoryWindowViewModelProperty, CloneRepositoryWindowViewModelCommand>, IWindowParameter, IWindowResult
    {
        #region Menber Variables
        /// <summary>
        /// ログ
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 通信ライブラリ
        /// </summary>
        private Gitlab gitlab;

        /// <summary>
        /// サーバーURL
        /// </summary>
        private string serverurl;

        /// <summary>
        /// パスワード
        /// </summary>
        private string password;

        /// <summary>
        /// メールアドレス
        /// </summary>
        private string email;

        /// <summary>
        /// api version
        /// </summary>
        private string apiVersion;

        /// <summary>
        /// プロジェクトリストの表示状態を設定または取得します
        /// </summary>
        private bool isEnabledProjectList;

        /// <summary>
        /// プロジェクトリストを設定または取得します
        /// </summary>
        private List<Project> projectlist;

        /// <summary>
        /// 選択されたプロジェクトを設定または取得します。
        /// </summary>
        private Project selectedProject = null;

        #endregion

        #region Properties
        /// <summary>
        /// プロジェクトリストの表示状態を設定または取得します
        /// </summary>
        public bool IsEnabledProjectList
        {
            get { return this.isEnabledProjectList; }
            set
            {
                this.isEnabledProjectList = value;
                this.RaisePropertyChanged(() => this.IsEnabledProjectList);
            }
        }

        /// <summary>
        /// プロジェクトリストを設定または取得します
        /// </summary>
        public List<Project> ProjectList
        {
            get { return this.projectlist; }
            set
            {
                if (this.projectlist != value)
                {
                    this.projectlist = value;
                    this.RaisePropertyChanged(() => this.ProjectList);
                }
            }
        }

        /// <summary>
        /// 選択されたプロジェクトを設定または取得します。
        /// </summary>
        public Project SelectedProject
        {
            get
            {
                return this.selectedProject;
            }

            set
            {
                if (this.selectedProject != value)
                {
                    this.Propertys.RepositoyName = value.Code;
                }
                if (this.apiVersion == "v2")
                {
                    this.Propertys.RemoteRepositoyUrl = string.Format(@"{0}{1}.git", this.serverurl, value.Code);
                }
                else
                {
                    this.Propertys.RemoteRepositoyUrl = string.Format(@"{0}{1}.git", this.serverurl, value.Path_with_namespace);
                }
            }
        }

        #endregion

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
        }
        #endregion

        #region Loadedメソッド
        /// <summary>
        /// Loadedメソッド
        /// </summary>
        public void Loaded()
        {
            if (this.Parameter != null)
            {
                // パラメータ設定
                this.serverurl = ((object[])this.Parameter)[0].ToString();
                this.password = ((object[])this.Parameter)[1].ToString();
                this.email = ((object[])this.Parameter)[2].ToString();
                this.apiVersion = ((object[])this.Parameter)[3].ToString();

                //プロジェクト取得処理
                if (!string.IsNullOrEmpty(this.serverurl) && !string.IsNullOrEmpty(this.password) && !string.IsNullOrEmpty(this.email))
                {
                    try
                    {
                        this.gitlab = new Gitlab(this.serverurl);
                        this.gitlab.ApiVersion = this.apiVersion == "v2" ? ApiVersionEnum.VERSION2 : ApiVersionEnum.VERSION3;

                        this.ProjectList = new List<Project>();

                        this.Propertys.IsCredential = true;
                        this.Propertys.PassWord = this.password;
                        this.Propertys.UserName = this.email;

                        this.getProjectList(this.email, this.password);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        this.IsEnabledProjectList = false;
                    }
                }
                else
                {
                    this.IsEnabledProjectList = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// プロジェクトリスト情報を取得します
        /// </summary>
        /// <param name="email">メールアドレス</param>
        /// <param name="password">パスワード</param>
        private async void getProjectList(string email, string password)
        {
            bool success = await this.gitlab.RequestSessionAsync(email, password);

            if (success)
            {
                this.ProjectList = await this.gitlab.RequestProjectsAsync();
                this.IsEnabledProjectList = true;
            }
            else 
            {
                this.IsEnabledProjectList = false;
            }
        }

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void OkButton()
        {
            logger.Info("操作：OKボタン");

            CloneEntity entity = new CloneEntity
            {
                Name = this.Propertys.RepositoyName,
                Path = this.Propertys.FolderPath,
                Url = this.Propertys.RemoteRepositoyUrl,
                IsCredential = this.Propertys.IsCredential,
                UserName = this.Propertys.UserName,
                PassWord = this.Propertys.PassWord,
            };
            WindowResultEntity windowResultEntity = new WindowResultEntity
            {
                Button = WindowButtonEnum.OK,
                Result = entity,
            };
            this.Responce = windowResultEntity;
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
            logger.Info("操作：Cancelボタン");

            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
            this.Responce = null;
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

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 戻り値
        /// </summary>
        public WindowResultEntity Responce { get; set; }
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class CloneRepositoryWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual string RepositoyName { get; set; }

        /// <summary>
        /// RemoteRepositoyUrl
        /// </summary>
        public virtual string RemoteRepositoyUrl { get; set; }

        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// IsCredential
        /// </summary>
        public virtual bool IsCredential { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// PassWord
        /// </summary>
        public virtual string PassWord { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class CloneRepositoryWindowViewModelCommand
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
