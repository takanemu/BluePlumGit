
namespace GordiasClassLibrary.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FileUtility
    {
        /// <summary>
        /// インスタンス化禁止
        /// </summary>
        static FileUtility()
        {
        }
        
        /// <summary>
        /// インスタンス化禁止
        /// </summary>
        private FileUtility()
        {
        }

        /// <summary>
        /// アプリケーションデータパスの取得
        /// </summary>
        /// <returns>パス</returns>
        public static string GetApplicationData()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        /// カレントディレクトリの取得
        /// </summary>
        /// <returns>パス</returns>
        public static string GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// ユーザーホームパスの取得
        /// </summary>
        /// <returns>パス</returns>
        public static string GetUserProfile()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}
