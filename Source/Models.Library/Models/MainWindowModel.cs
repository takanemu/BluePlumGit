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
    using System.Linq;
    using BluePlumGit.Entitys;
    using BluePlumGit.RepositorysDataSetTableAdapters;
    using GordiasClassLibrary.Collections;
    using Livet;

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


        public MainWindowModel()
        {
        }

        ~MainWindowModel()
        {
        }


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

        public int GetRepositoryCount()
        {
            var ta = new RepositorysTableAdapter();
            var repositorys = ta.GetData();

            return repositorys.Rows.Count;
        }
    }
}
