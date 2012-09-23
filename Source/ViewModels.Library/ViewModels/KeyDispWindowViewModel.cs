﻿
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
    using Livet.Messaging.Windows;
    using System.Windows;

    /// <summary>
    /// 鍵表示ウインドウ
    /// </summary>
    public class KeyDispWindowViewModel : TacticsViewModel<KeyDispWindowViewModelProperty, KeyDispWindowViewModelCommand>, IWindowParameter
    {
        public void Initialize()
        {
            this.Propertys.Text = ((RSAKeyEntity)this.Parameter).Text;
        }

        #region Cancelボタン処理
        /// <summary>
        /// Cancelボタン処理
        /// </summary>
        [Command]
        public void CopyButton()
        {
            Clipboard.SetText(this.Propertys.Text);
        }
        #endregion

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
        /// <summary>
        /// OkButtonコマンド
        /// </summary>
        public TacticsCommand CopyButton { get; set; }
    }
}
