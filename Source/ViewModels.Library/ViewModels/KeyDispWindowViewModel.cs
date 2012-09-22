
namespace BluePlumGit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GordiasClassLibrary.Entitys;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interface;
    using BluePlumGit.Entitys;

    /// <summary>
    /// 鍵表示ウインドウ
    /// </summary>
    public class KeyDispWindowViewModel : TacticsViewModel<KeyDispWindowViewModelProperty, KeyDispWindowViewModelCommand>, IWindowParameter
    {
        public void Initialize()
        {
            this.Propertys.Text = ((RSAKeyEntity)this.Parameter).Text;
        }

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Parameter { get; set; }
    }

    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class KeyDispWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual string Text { get; set; }
    }

    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class KeyDispWindowViewModelCommand
    {
    }
}
