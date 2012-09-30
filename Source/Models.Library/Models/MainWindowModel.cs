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

namespace BluePlumGit.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using BluePlumGit.Entitys;
    using BluePlumGit.Library;
    using BluePlumGit.RepositorysDataSetTableAdapters;
    using GordiasClassLibrary.Collections;
    using Livet;
    using NGit.Api;
    using Sharpen;

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
        /// 
        /// </summary>
        /// <returns></returns>
        public List<RepositoryEntity> OpenDataBase()
        {
            List<RepositoryEntity> result = new List<RepositoryEntity>();

            //var appData = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //var dataPath = appData + @"\OmeSystemPlan\BluePlumGit";

            //if (!Directory.Exists(dataPath))
            //{
            //    // ディレクトリの作成
            //    Directory.CreateDirectory(dataPath);
            //}

            //using (var conn = new SQLiteConnection("Data Source=Configuration.db"))
            //{
            //    conn.Open();
            //    conn.Close();
            //} 

            var ta = new RepositorysTableAdapter();
            var repositorys = ta.GetData();

            foreach (RepositorysDataSet.RepositorysRow row in repositorys.Rows)
            {
                Console.WriteLine("{0}, {1}", row.name, row.path);
            }

            LinqList<RepositorysDataSet.RepositorysRow> rows = new LinqList<RepositorysDataSet.RepositorysRow>(repositorys.Rows);

            result = rows.Select(
                (row) =>
                {
                    RepositoryEntity ret = new RepositoryEntity
                    {
                        ID = row.id,
                        Name = row.name,
                        Path = row.path,
                    };
                    return ret;
                }
                ).ToList<RepositoryEntity>();

            return result;
        }

        public void AddRepository(long id, string name, string path)
        {
            var ta = new RepositorysTableAdapter();
            var repositorys = ta.GetData();

            repositorys.AddRepositorysRow(id, name, path);

            ta.Update(repositorys);  
        }

        public void RemoveRepository(long id)
        {
            var ta = new RepositorysTableAdapter();
            var repositorys = ta.GetData();

            BluePlumGit.RepositorysDataSet.RepositorysRow row = repositorys.FindByid(id);

            if (row != null)
            {
                row.Delete();
                ta.Update(repositorys);
            }
        }

        /// <summary>
        /// リポジトリ数の取得
        /// </summary>
        /// <returns>リポジトリ数</returns>
        public int GetRepositoryCount()
        {
            var ta = new RepositorysTableAdapter();
            var repositorys = ta.GetData();

            return repositorys.Rows.Count;
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
        /// リモートリポジトリをローカルへ複製する
        /// </summary>
        /// <param name="entity">エンティティ</param>
        /// <param name="monitor">モニター</param>
        public void CloneRepository(CloneEntity entity, BusyIndicatorProgressMonitor monitor)
        {
            FilePath directory = entity.Path;

            CloneCommand clone = Git.CloneRepository();
            clone.SetBare(false);
            clone.SetCloneAllBranches(true);
            clone.SetDirectory(directory);
            clone.SetURI(entity.Url);
            //clone.SetRemote(entity.Url);

            clone.SetProgressMonitor(monitor);

            //UsernamePasswordCredentialsProvider user = new UsernamePasswordCredentialsProvider("", "");

            //clone.SetCredentialsProvider(user);

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += (s, evt) =>
                {
                    monitor.StartAction();
                    clone.Call();
                };
            bw.RunWorkerCompleted += (s, evt) =>
                {
                    monitor.CompleteAction();
                };
            bw.RunWorkerAsync();
        }
    }
}
