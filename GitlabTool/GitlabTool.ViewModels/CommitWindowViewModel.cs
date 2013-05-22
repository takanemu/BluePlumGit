using Common.Library.Entitys;
using Common.Library.Enums;
using GitlabTool.ViewModels.Items;
using Gordias.Library.Entitys;
using Gordias.Library.Headquarters;
using Gordias.Library.Interface;
using Livet.Messaging.Windows;
using log4net;
using NGit.Api;
using NGit.Diff;
using NGit.Storage.File;
using Sharpen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GitlabTool.ViewModels
{
    /// <summary>
    /// コミットウィンドウ VM
    /// </summary>
    public class CommitWindowViewModel : TacticsViewModel<CommitWindowViewModelProperty, CommitWindowViewModelCommand>, IWindowParameter
    {
        /// <summary>
        /// ログ
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Members

        /// <summary>
        /// テーブル
        /// </summary>
        private Dictionary<DiffEntry.ChangeType, string> table;

        /// <summary>
        /// Git
        /// </summary>
        private Git git;

        #endregion

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

        #region Methods

        /// <summary>
        /// Initializeメソッド
        /// </summary>
        public void Initialize() 
        {

            RepositoryEntity entity = (RepositoryEntity)this.Parameter;

            if (entity == null) return;
            this.Propertys.FolderPath = entity.Location;

            FilePath path = new FilePath(this.Propertys.FolderPath, @".git");
            FileRepository db = new FileRepository(path);
            this.git = new Git(db);

            DiffCommand diff = this.git.Diff().SetShowNameAndStatusOnly(true);

            IList<DiffEntry> entries = diff.Call();

            this.Propertys.Diffs = new ObservableCollection<DiffValue>();

            foreach (DiffEntry item in entries)
            {
                DiffValue row = new DiffValue();

                row.ChangeType = item.GetChangeType();
                row.ChangeTypeName = ConvChangeType(item.GetChangeType());

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

        #region OKボタン処理
        /// <summary>
        /// OKボタン処理
        /// </summary>
        [Command]
        private void CommitButton()
        {
            bool isCheckFlag = false;

            logger.Info("操作：Commitボタン");

            string commitMessage = this.Propertys.CommitMessage;

            if (commitMessage == null || commitMessage == string.Empty)
            {
                MessageBox.Show("コミットメッセージを入力してください。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!ItemCheck())
            {
                MessageBox.Show("何も選択されていません。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (DiffValue item in this.Propertys.Diffs)
            {
                if (item.Check)
                {
                    if (!isCheckFlag) isCheckFlag = true;

                    if (item.ChangeType == DiffEntry.ChangeType.ADD)
                    {
                        this.git.Add().AddFilepattern(item.Filename).Call();
                    }
                    else if (item.ChangeType == DiffEntry.ChangeType.MODIFY)
                    {
                        this.git.Add().AddFilepattern(item.Filename).Call();
                    }
                    else if (item.ChangeType == DiffEntry.ChangeType.DELETE)
                    {
                        this.git.Rm().AddFilepattern(item.Filename).Call();
                    }
                }
            }

            this.git.Commit().SetMessage(commitMessage).Call();
            this.Messenger.Raise(new WindowActionMessage("WindowControl", WindowAction.Close));
        }
        #endregion

        /// <summary>
        /// チェック状態を確認します。
        /// </summary>
        /// <returns>１つでもチェック状態であれば、trueを返し、それ以外はfalseを返します</returns>
        private bool ItemCheck() 
        {
            bool isCheck = false;

            foreach (DiffValue item in this.Propertys.Diffs)
            {
                if (item.Check == true) { isCheck = true; break; }
            }

            return isCheck;
        }

        #region Cancelボタン処理
        /// <summary>
        /// Cancelボタン処理
        /// </summary>
        [Command]
        private void CancelButton()
        {
            logger.Info("操作：Cancelボタン");

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
            foreach (DiffValue item in this.Propertys.Diffs)
            {
                item.Check = true;
            }
        }

        /// <summary>
        /// 全解除ボタン処理
        /// </summary>
        [Command]
        private void AllReleaseButton()
        {
            foreach (DiffValue item in this.Propertys.Diffs)
            {
                item.Check = false;
            }
        }

        /// <summary>
        /// ChangeTypeを文字列へ変換する
        /// </summary>
        /// <param name="type">対象となるタイプ</param>
        /// <returns>変換された文字列</returns>
        private string ConvChangeType(DiffEntry.ChangeType type)
        {
            return this.table[type];
        }

        #endregion

        #region Properties

        /// <summary>
        /// パラメーター
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual ObservableCollection<DiffValue> Diffs { get; set; }

        /// <summary>
        /// 戻り値
        /// </summary>
        public WindowResultEntity Responce { get; set; }

        #endregion
    }

    #region プロパティクラス

    /// <summary>
    /// プロパティクラスを表します
    /// </summary>
    public class CommitWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// FolderPath
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// RepositoyName
        /// </summary>
        public virtual ObservableCollection<DiffValue> Diffs { get; set; }

        /// <summary>
        /// コミットメッセージ
        /// </summary>
        public virtual string CommitMessage { get; set; }
    }

    #endregion

    #region コマンドクラス
    
    /// <summary>
    /// コマンドクラスを表します
    /// </summary>
    public class  CommitWindowViewModelCommand
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
        /// AllSelectButtonコマンド
        /// </summary>
        public TacticsCommand AllSelectButton { get; private set; }

        /// <summary>
        /// AllReleaseButtonコマンド
        /// </summary>
        public TacticsCommand AllReleaseButton { get; private set; }

    }
    #endregion
}
