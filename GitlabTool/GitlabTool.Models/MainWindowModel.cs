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

namespace GitlabTool.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Commno.Library;
    using Common.Library.Entitys;
    using Gitlab;
    using Gordias.Library.Collections;
    using Livet;
    using Newtonsoft.Json;
    using NGit;
    using NGit.Api;
    using NGit.Api.Errors;
    using NGit.Storage.File;
    using NGit.Transport;
    using NGit.Util;
    using Sharpen;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using log4net;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// SSH接続用クラス(未使用)
    /// </summary>
    internal class CustomConfigSessionFactory : JschConfigSessionFactory
    {
        /// <summary>
        /// 秘密鍵
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 公開鍵
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="hc">ホスト</param>
        /// <param name="session">セッション</param>
        protected override void Configure(NGit.Transport.OpenSshConfig.Host hc, NSch.Session session)
        {
            var config = new Properties();

            config["StrictHostKeyChecking"] = "no";
            config["PreferredAuthentications"] = "publickey";
            session.SetConfig(config);

            var jsch = this.GetJSch(hc, FS.DETECTED);
            jsch.AddIdentity("KeyPair", Encoding.UTF8.GetBytes(PrivateKey), Encoding.UTF8.GetBytes(PublicKey), null);
        }
    }

    /// <summary>
    /// メインウインドウモデル
    /// </summary>
    public class MainWindowModel : NotificationObject
    {
        /// <summary>
        /// ログ
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        /*
         * ModelからViewModelへのイベントを発行する場合はNotificatorを使用してください。
         *
         * Notificatorはイベント代替手段です。コードスニペット lnev でCLRイベントと同時に定義できます。
         *
         * ViewModelへNotificatorを使用した通知を行う場合はViewModel側でViewModelHelperを使用して受信側の登録をしてください。
         */

        private static readonly string ApplicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"2ndfactory\GitlabTool");

        private static readonly string ConfigFile = Path.Combine(MainWindowModel.ApplicationDataPath, "Config.json");

        private Gitlab gitlab;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowModel()
        {
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~MainWindowModel()
        {
        }

        /// <summary>
        /// 設定ファイルの読み込み
        /// </summary>
        /// <returns>設定</returns>
        public ConfigEntity OpenConfig()
        {
            FilePath file = new FilePath(MainWindowModel.ConfigFile);

            if (file.Exists())
            {
                StreamReader sr = new StreamReader(MainWindowModel.ConfigFile, Encoding.UTF8);

                string json = sr.ReadToEnd();

                sr.Close();

                return JsonConvert.DeserializeObject<ConfigEntity>(json);
            }
            return new ConfigEntity();
        }

        /// <summary>
        /// 設定のファイル保存
        /// </summary>
        /// <param name="config">設定</param>
        public void SaveConfig(ConfigEntity config)
        {
            FilePath path = new FilePath(MainWindowModel.ApplicationDataPath);

            if (!path.Exists())
            {
                path.Mkdir();
            }
            string json = JsonConvert.SerializeObject(config, new Newtonsoft.Json.Converters.StringEnumConverter());

            StreamWriter sw = new StreamWriter(MainWindowModel.ConfigFile, false, Encoding.UTF8);

            sw.Write(json);

            sw.Close();
        }

        /// <summary>
        /// セッション取得
        /// </summary>
        /// <param name="host">ホストアドレス</param>
        /// <param name="email">電子メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>true成功</returns>
        public async Task<bool> OpenServerSession(string host, string email, string password, ApiVersionEnum version)
        {
            this.gitlab = new Gitlab(host);
            this.gitlab.ApiVersion = version;

            this.gitlab.ErrorAction = (Exception exception) =>
            {
                // TODO:例外発生時の処理
            };

            // セッションの取得
            bool saccess = await this.gitlab.RequestSessionAsync(email, password);

            if (saccess)
            {
                // TODO:成功
                return true;
            }
            return false;
        }

        public async Task<bool> AddSSHkeyAsync(string title, string text)
        {
            bool result = await this.gitlab.AddSSHkeyAsync(title, text);

            return result;
        }

        #region Complete変更通知プロパティ
        private bool complete;

        public bool Complete
        {
            get
            {
                return this.complete;
            }
            
            set
            {
                if (this.complete == value)
                {
                    return;
                }
                this.complete = value;
                this.RaisePropertyChanged(() => this.Complete);
            }
        }
        #endregion

        #region CloneCounter変更通知プロパティ
        private int cloneCounter;

        public int CloneCounter
        {
            get
            {
                return this.cloneCounter;
            }
            
            set
            {
                if (this.cloneCounter == value)
                {
                    return;
                }
                this.cloneCounter = value;
                this.RaisePropertyChanged(() => this.CloneCounter);
            }
        }
        #endregion

        /// <summary>
        /// グローバル設定ファイル読み込み
        /// </summary>
        public GrobalConfigEntity LoadGrobalConfig()
        {
            FilePath gitconfig = new FilePath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".gitconfig");

            if (!gitconfig.Exists())
            {
                logger.Error("ファイル読み込みエラー[" + gitconfig + "]");
                return null;
            }
            FileBasedConfig config = new FileBasedConfig(gitconfig, FS.Detect());

            config.Load();
/*
            string text = config.ToText();

            foreach (string section in config.GetSections())
            {
                Console.Out.WriteLine("section = {0}", section);

                if (config.GetSubsections(section).Count > 0)
                {
                    foreach (string subsection in config.GetSubsections(section))
                    {
                        Console.Out.WriteLine(" subsection = {0}", subsection);

                        foreach (string name in config.GetNames(section, subsection))
                        {
                            Console.Out.WriteLine("  name = {0} / value = {1}", name, config.GetString(section, subsection, name));
                        }
                    }
                }
                else
                {
                    foreach (string name in config.GetNames(section))
                    {
                        Console.Out.WriteLine("  name = {0} / value = {1}", name, config.GetString(section, null, name));
                    }
                }
            }
*/
            GrobalConfigEntity result = new GrobalConfigEntity();

            result.EMail = config.GetString("user", null, "email");
            result.Name = config.GetString("user", null, "name");

            if (result.Name == null)
            {
                logger.Error(".gitconfig ファイルに、ユーザー名が記載されていません。");
                return null;
            }
            if (result.EMail == null)
            {
                logger.Error(".gitconfig ファイルに、電子メールが記載されていません。");
                return null;
            }
            return result;
        }

        /// <summary>
        /// リモートリポジトリをローカルへ複製する
        /// </summary>
        /// <param name="entity">エンティティ</param>
        /// <param name="privateKeyData">秘密鍵</param>
        /// <param name="publicKeyData">公開鍵</param>
        /// <param name="monitor">モニター</param>
        public void CloneRepository(CloneEntity entity, string privateKeyData, string publicKeyData, BusyIndicatorProgressMonitor monitor)
        {
            var customConfigSessionFactory = new CustomConfigSessionFactory();

            customConfigSessionFactory.PrivateKey = privateKeyData;
            customConfigSessionFactory.PublicKey = publicKeyData;

            NGit.Transport.JschConfigSessionFactory.SetInstance(customConfigSessionFactory);

            UsernamePasswordCredentialsProvider creds = new UsernamePasswordCredentialsProvider(entity.UserName, entity.PassWord);

            FilePath directory = entity.Path;
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (s, evt) =>
            {
                monitor.StartAction();

                try
                {
                    var git = Git.CloneRepository()
                                  .SetProgressMonitor(monitor)
                                  .SetDirectory(directory)
                                  .SetURI(entity.Url)
                                  .SetBranchesToClone(new Collection<string>() { "master" })
                                  .SetCredentialsProvider(creds)
                                  .Call();
                }
                catch (JGitInternalException)
                {
                    // TODO:
                }
            };
            bw.RunWorkerCompleted += (s, evt) =>
            {
                monitor.CompleteAction();
                //this.SettingHttpBufferSize(entity.Path);
            };
            bw.RunWorkerAsync();
            
        }

        /// <summary>
        /// httpプロトコルバッファ値を設定。
        /// この設定がないと大きなファイルをpushできない。
        /// </summary>
        /// <param name="folderPath">リポジトリパス</param>
        private void SettingHttpBufferSize(string folderPath)
        {
            FilePath path = new FilePath(folderPath, @".git");

            FileRepository db = new FileRepository(path);

            Git git = new Git(db);

            var config = git.GetRepository().GetConfig();

            config.SetString("http", null, "postBuffer", "524288000");
            config.Save();
        }

        /// <summary>
        /// フェッチ
        /// </summary>
        /// <param name="git"></param>
        /// <param name="privateKeyData"></param>
        /// <param name="publicKeyData"></param>
        /// <param name="monitor"></param>
        public void Fetch(Git git, CloneEntity entity, string privateKeyData, string publicKeyData, BusyIndicatorProgressMonitor monitor)
        {
            var customConfigSessionFactory = new CustomConfigSessionFactory();

            customConfigSessionFactory.PrivateKey = privateKeyData;
            customConfigSessionFactory.PublicKey = publicKeyData;

            NGit.Transport.JschConfigSessionFactory.SetInstance(customConfigSessionFactory);

            UsernamePasswordCredentialsProvider creds = new UsernamePasswordCredentialsProvider(entity.UserName, entity.PassWord);

            FetchCommand command = git.Fetch();

            RefSpec spec = new RefSpec("refs/heads/master:refs/heads/FETCH_HEAD");

            command.SetRemoveDeletedRefs(true);
            command.SetRefSpecs(spec);
            command.SetProgressMonitor(monitor);
            command.SetCredentialsProvider(creds);

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (s, evt) =>
            {
                monitor.StartAction();

                try
                {
                    command.Call();
                }
                catch (JGitInternalException)
                {
                    // TODO:
                }
            };
            bw.RunWorkerCompleted += (s, evt) =>
            {
                monitor.CompleteAction();
            };
            bw.RunWorkerAsync();
        }

        public void Pull(Git git, BusyIndicatorProgressMonitor monitor)
        {
            PullCommand command = git.Pull();

            command.SetProgressMonitor(monitor);

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (s, evt) =>
            {
                monitor.StartAction();

                try
                {
                    command.Call();
                }
                catch (JGitInternalException)
                {
                    // TODO:
                }
            };
            bw.RunWorkerCompleted += (s, evt) =>
            {
                monitor.CompleteAction();
            };
            bw.RunWorkerAsync();
        }
    }
}
