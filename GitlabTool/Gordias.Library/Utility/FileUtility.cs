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

namespace Gordias.Library.Utility
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
