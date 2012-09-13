using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluePlumGit.Interface
{
    public interface ICleanup
    {
        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 破棄処理を行います。
        /// </summary>
        void Uninitialize();
    }
}
