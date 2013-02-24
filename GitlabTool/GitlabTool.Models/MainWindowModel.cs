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

namespace GitlabTool.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using Common.Library.Entitys;
    using Gordias.Library.Collections;
    using Livet;
    using NGit.Api;
    using Sharpen;
    using NGit.Api.Errors;
    using NGit.Transport;
    using NGit.Storage.File;
    using NGit.Util;
    using Commno.Library;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
using Gitlab;

    /// <summary>
    /// SSH接続用クラス(未使用)
    /// </summary>
    internal class DefaultSshSessionFactory : JschConfigSessionFactory
    {
        protected override void Configure(NGit.Transport.OpenSshConfig.Host hc, NSch.Session session)
        {
        }
    }

    /// <summary>
    /// メインウインドウモデル
    /// </summary>
    public class MainWindowModel : NotificationObject
    {
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

                return JsonConvert.DeserializeObject<ConfigEntity>(json); ;
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
        /// <param name="host"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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
                RaisePropertyChanged(() => Complete);
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
                RaisePropertyChanged(() => CloneCounter);
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

            if (result.EMail == null || result.Name == null)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// リモートリポジトリをローカルへ複製する
        /// </summary>
        /// <param name="entity">エンティティ</param>
        /// <param name="monitor">モニター</param>
        public void CloneRepository(CloneEntity entity, BusyIndicatorProgressMonitor monitor)
        {
            FilePath directory = entity.Path;

            CloneCommand clone = new CloneCommand();

            //clone.SetCloneAllBranches(true);
            clone.SetDirectory(directory);
            clone.SetURI(entity.Url);

            clone.SetProgressMonitor(monitor);

            if (entity.IsCredential)
            {
                UsernamePasswordCredentialsProvider user = new UsernamePasswordCredentialsProvider(entity.UserName, entity.PassWord);

                clone.SetCredentialsProvider(user);
            }
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (s, evt) =>
                {
                    monitor.StartAction();

                    try
                    {
                        clone.Call();
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

        public void Fetch(Git git, BusyIndicatorProgressMonitor monitor)
        {
            FetchCommand command = git.Fetch();

            RefSpec spec = new RefSpec("refs/heads/master:refs/heads/FETCH_HEAD");

            command.SetRefSpecs(spec);
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
