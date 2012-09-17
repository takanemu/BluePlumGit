
namespace BluePlumGit.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using BluePlumGit.Entitys;
    using BluePlumGit.RepositorysDataSetTableAdapters;
    using GordiasClassLibrary.Collections;
    using Livet;

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

        public int GetRepositoryCount()
        {
            var ta = new RepositorysTableAdapter();
            var repositorys = ta.GetData();

            return repositorys.Rows.Count;
        }
    }
}
