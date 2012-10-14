
namespace BluePlumGit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GordiasClassLibrary.Headquarters;
    using GordiasClassLibrary.Interface;
    using GordiasClassLibrary.Entitys;
    using System.Collections.ObjectModel;
    using BluePlumGit.Items;
    using NGit.Api;
    using NGit.Diff;
    using System.Windows.Forms;
    using Livet.Messaging.Windows;
    using Common.Library.Enums;

    #region メインクラス
    public class CommitWindowViewModel : TacticsViewModel<CommitWindowViewModelProperty, CommitWindowViewModelCommand>, IWindowParameter, IWindowResult
    {
        /// <summary>
        /// カレントリポジトリ
        /// </summary>
        private Git git;

        private Dictionary<DiffEntry.ChangeType, string> table;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommitWindowViewModel()
        {
            this.table = new Dictionary<DiffEntry.ChangeType, string>
            {
                {DiffEntry.ChangeType.ADD, "追加"},
                {DiffEntry.ChangeType.COPY, "複製"},
                {DiffEntry.ChangeType.DELETE, "削除"},
                {DiffEntry.ChangeType.MODIFY, "変更"},
                {DiffEntry.ChangeType.RENAME, "名称変更"},
            };
        }

        #region Initializeメソッド
        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize()
        {
        }
        #endregion

        #region Loadedメソッド
        /// <summary>
        /// Loadedメソッド
        /// </summary>
        public void Loaded()
        {
            this.git = (Git)this.Parameter;

            DiffCommand diff = this.git.Diff().SetShowNameAndStatusOnly(true);

            IList<DiffEntry> entries = diff.Call();

            this.Propertys.Diffs = new ObservableCollection<DiffValue>();

            foreach (DiffEntry item in entries)
            {
                DiffValue row = new DiffValue();

                row.ChangeType = ConvChangeType(item.GetChangeType());

                switch (item.GetChangeType())
                {
                    case DiffEntry.ChangeType.ADD:
                        row.Filename = item.GetPath(DiffEntry.Side.NEW);
                        row.Oldname = "";
                        break;
                    case DiffEntry.ChangeType.COPY:
                        row.Filename = item.GetPath(DiffEntry.Side.OLD);
                        row.Oldname = "";
                        break;
                    case DiffEntry.ChangeType.DELETE:
                        row.Filename = item.GetPath(DiffEntry.Side.OLD);
                        row.Oldname = "";
                        break;
                    case DiffEntry.ChangeType.MODIFY:
                        row.Check = true;
                        row.Filename = item.GetPath(DiffEntry.Side.OLD);
                        row.Oldname = "";
                        break;
                    case DiffEntry.ChangeType.RENAME:
                        row.Filename = item.GetPath(DiffEntry.Side.NEW);
                        row.Oldname = item.GetPath(DiffEntry.Side.OLD);
                        break;
                }
                this.Propertys.Diffs.Add(row);
            }
        }
        #endregion

        private string ConvChangeType(DiffEntry.ChangeType type)
        {
            return this.table[type];
        }

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void CommitButton()
        {
            WindowResultEntity windowResultEntity = new WindowResultEntity
            {
                Button = WindowButtonEnum.OK,
                Result = this.Propertys.Diffs,
            };
            this.Responce = windowResultEntity;
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        #region Cancelボタン処理
        /// <summary>
        /// Cancelボタン処理
        /// </summary>
        [Command]
        private void CancelButton()
        {
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
            this.Responce = null;
        }
        #endregion

        /// <summary>
        /// 全選択ボタン処理
        /// </summary>
        [Command]
        private void AllSelectButton()
        {
        }

        /// <summary>
        /// 全解除ボタン処理
        /// </summary>
        [Command]
        private void AllReleaseButton()
        {
        }

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 戻り値
        /// </summary>
        public WindowResultEntity Responce { get; set; }
    }
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class CommitWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual ObservableCollection<DiffValue> Diffs { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class CommitWindowViewModelCommand
    {
        /// <summary>
        /// CommitButtonコマンド
        /// </summary>
        public TacticsCommand CommitButton { get; private set; }

        /// <summary>
        /// CancelButtonコマンド
        /// </summary>
        public TacticsCommand CancelButton { get; private set; }

        /// <summary>
        /// CommitButtonコマンド
        /// </summary>
        public TacticsCommand AllSelectButton { get; private set; }

        /// <summary>
        /// CommitButtonコマンド
        /// </summary>
        public TacticsCommand AllReleaseButton { get; private set; }
    }
    #endregion
}
