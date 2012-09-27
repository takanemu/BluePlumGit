using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGit.Api;

namespace BluePlumGit.Utility
{
    public class RepositotyUtility
    {
        /// <summary>
        /// カレントブランチ名を取得する
        /// </summary>
        /// <param name="git">Gitクラス</param>
        /// <returns>ブランチ名</returns>
        public static string GetCurrentBranch(Git git)
        {
            return git.GetRepository().GetBranch();
        }
    }
}
