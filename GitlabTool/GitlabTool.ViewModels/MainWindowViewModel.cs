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

namespace GitlabTool.ViewModels
{
    using Commno.Library;
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Common.Library.Messaging.Windows;
    using GitlabTool;
    using GitlabTool.Models;
    using Gordias.Library.Headquarters;
    using NGit;
    using Sharpen;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    #region メインクラス
    /// <summary>
    /// メインウインドウビューモデル
    /// </summary>
    public class MainWindowViewModel : TacticsViewModel<MainWindowViewModelProperty, MainWindowViewModelCommand>
	{
        private static readonly string RSAKeyFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".ssh\gitlab_tool");

        /// <summary>
        /// モデル
        /// </summary>
        private MainWindowModel model;

        /// <summary>
        /// グローバルコンフィグ
        /// </summary>
        private GrobalConfigEntity globalConfig;

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private ConfigEntity config;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.model = new MainWindowModel();
        }


		#region WindowState 変更通知プロパティ

		private WindowState _WindowState;

		public WindowState WindowState
		{
			get { return this._WindowState; }
			set
			{
				if (this._WindowState != value)
				{
					this._WindowState = value;
                    this.IsMaximized = value == WindowState.Maximized;
                    this.CanNormalize = value == WindowState.Maximized;
                    this.CanMaximize = value == WindowState.Normal;
                    this.RaisePropertyChanged(() => WindowState);
                }
			}
		}
		#endregion
        
		#region IsMaximized 変更通知プロパティ

		private bool _IsMaximized;

		public bool IsMaximized
		{
			get { return this._IsMaximized; }
			set
			{
				if (this._IsMaximized != value)
				{
					this._IsMaximized = value;
                    //this.RaisePropertyChanged();
                    this.RaisePropertyChanged(() => IsMaximized);
				}
			}
		}

		#endregion

		#region CanMaximize 変更通知プロパティ

		private bool _CanMaximize = true;

		public bool CanMaximize
		{
			get { return this._CanMaximize; }
			set
			{
				if (this._CanMaximize != value)
				{
					this._CanMaximize = value;
                    //this.RaisePropertyChanged();
                    this.RaisePropertyChanged(() => CanMaximize);
                }
			}
		}

		#endregion

		#region CanMinimize 変更通知プロパティ

		private bool _CanMinimize = true;

		public bool CanMinimize
		{
			get { return this._CanMinimize; }
			set
			{
				if (this._CanMinimize != value)
				{
					this._CanMinimize = value;
                    //this.RaisePropertyChanged();
                    this.RaisePropertyChanged(() => CanMinimize);
                }
			}
		}

		#endregion

		#region CanNormalize 変更通知プロパティ

		private bool _CanNormalize = false;

		public bool CanNormalize
		{
			get { return this._CanNormalize; }
			set
			{
				if (this._CanNormalize != value)
				{
					this._CanNormalize = value;
                    //this.RaisePropertyChanged();
                    this.RaisePropertyChanged(() => CanNormalize);
                }
			}
		}

		#endregion

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
            this.config = this.model.OpenConfig();

            DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);


            this.globalConfig = this.model.LoadGrobalConfig();

            if (this.globalConfig == null)
            {
                this.Config();
            }

            if (this.globalConfig != null && this.globalConfig.EMail != null)
            {
                this.Propertys.GravatarId = this.globalConfig.EMail;
            }
        }
        #endregion

        #region 設定変更コマンド
        /// <summary>
        /// 設定変更コマンド
        /// </summary>
        [Command]
        private void Config()
        {
            ConfigDialogEntity param = new ConfigDialogEntity
            {
                ServerUrl = this.config.ServerUrl,
                Password = this.config.Password,
                Accent = this.config.Accent,
            };

            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CONFIG,
                Parameter = param,
            });

            if (message.Response != null)
            {
                ConfigDialogEntity entity = (ConfigDialogEntity)message.Response.Result;

                this.config.ServerUrl = entity.ServerUrl;
                this.config.Password = entity.Password;

                if (this.config.Accent != entity.Accent)
                {
                    this.config.Accent = entity.Accent;
                    DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);
                }
            }
        }
        #endregion

        #region 公開鍵の作成
        /// <summary>
        /// 公開鍵の作成
        /// </summary>
        [Command]
        private async void KeypairGeneration()
        {
            FilePath keyfile = new FilePath(MainWindowViewModel.RSAKeyFilePath);
            FilePath pubfile = new FilePath(MainWindowViewModel.RSAKeyFilePath + ".pub");

            if (keyfile.Exists())
            {
                // 旧ファイルの削除
                keyfile.Delete();
                pubfile.Delete();
            }
            // スクリプトで生成できないので、コマンドを呼び出して生成している。
            Process process = new Process();

            process.StartInfo.FileName = @".\ssh-keygen.exe";
            process.StartInfo.Arguments = @"-t rsa -N """" -C """ + this.globalConfig.EMail + @""" -f " + keyfile.GetAbsolutePath();
            process.StartInfo.WorkingDirectory = @".\";

            process.Start();

            // キーの登録
            if (pubfile.Exists())
            {
                // テキストファイル読み込み
                StreamReader sr = new StreamReader(pubfile.GetAbsolutePath(), Encoding.UTF8);

                string text = sr.ReadToEnd();

                sr.Close();

                // セッションの取得
                bool saccess = await this.model.OpenServerSession(this.config.ServerUrl, this.globalConfig.EMail, this.config.Password);

                if (saccess)
                {
                    // キーの追加
                    await this.model.AddSSHkeyAsync(this.globalConfig.EMail, text);
                }
            }

            /*
            FilePath keyfile = new FilePath(MainWindowViewModel.RSAKeyFilePath);

            if (keyfile.Exists())
            {
                return;
            }

            int type = Tamir.SharpSsh.jsch.KeyPair.RSA;

            // Output file name
            String filename = keyfile.GetAbsolutePath();
            // Signature comment
            String comment = "takanemu@gmail.com";

            try
            {
                // Create a new JSch instance
                Tamir.SharpSsh.jsch.JSch jsch = new Tamir.SharpSsh.jsch.JSch();

                // Prompt the user for a passphrase for the private key file
                String passphrase = "";

                // Generate the new key pair
                Tamir.SharpSsh.jsch.KeyPair kpair = Tamir.SharpSsh.jsch.KeyPair.genKeyPair(jsch, type);
                // Set a passphrase
                kpair.setPassphrase(passphrase);
                // Write the private key to "filename"
                kpair.writePrivateKey(filename);
                // Write the public key to "filename.pub"
                kpair.writePublicKey(filename + ".pub", comment);
                // Print the key fingerprint
                Console.WriteLine("Finger print: " + kpair.getFingerPrint());
                // Free resources
                kpair.dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            */
        }
        #endregion

        #region 複製コマンド
        /// <summary>
        /// 複製コマンド
        /// </summary>
        [Command]
        private void RepositoryClone()
        {
            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CLONE_REPOSITORY,
            });

            if (message.Response != null)
            {
                if ((WindowButtonEnum)message.Response.Button == WindowButtonEnum.OK)
                {
                    CloneEntity entity = (CloneEntity)message.Response.Result;
                    string gitdirName = Path.Combine(entity.Path, Constants.DOT_GIT);
                    FilePath gitdir = new FilePath(gitdirName);

                    if (!gitdir.Exists())
                    {
                        BusyIndicatorProgressMonitor monitor = new BusyIndicatorProgressMonitor();

                        monitor.StartAction = () =>
                        {
                            this.OpenBusyIndicator("リポジトリの複製中です。");
                        };
                        monitor.UpdateAction = (string taskName, int cmp, int totalWork, int pcnt) =>
                        {
                            this.UpdateBusyIndicator(taskName, cmp, totalWork, pcnt);
                        };
                        monitor.CompleteAction = () =>
                        {
                            this.CloseBusyIndicator();

                            RepositoryEntity repository = new RepositoryEntity
                            {
                                //ID = this.model.GetRepositoryNewId(),
                                Name = entity.Name,
                                Path = gitdir,
                            };

                            // dbの登録
                            //this.model.AddRepository(repository.ID, repository.Name, repository.Path);

                            // リスト追加
                            //this.repositorysCollection.Add(repository);
                            //this.RepositoryCollectionView.MoveCurrentTo(repository);
                        };

                        this.model.CloneRepository(entity, monitor);
                    }
                    else
                    {
                        MessageBox.Show(".gitディレクトリが、既に存在します。");
                    }
                }
            }
        }
        #endregion

        #region ウインドウクローズキャンセル処理
        /// <summary>
        /// ウインドウクローズキャンセル処理
        /// </summary>
        [Command]
        private void WindowCloseCancel()
        {
            this.model.SaveConfig(this.config);

            // TODO:終了時保存保護処理
            Environment.Exit(0);
        }
        #endregion

        /// <summary>
        /// インジケーターを表示する
        /// </summary>
        /// <param name="title">タイトル</param>
        private void OpenBusyIndicator(string title)
        {
            this.Propertys.BusyDialogMessageTitle = title;
            this.Propertys.IsBusyDialog = true;
        }

        /// <summary>
        /// インジケーターの更新
        /// </summary>
        /// <param name="complete">処理完了数</param>
        private void UpdateBusyIndicator(string taskName, int cmp, int totalWork, int pcnt)
        {
            this.Propertys.BusyDialogPcent = (double)pcnt;
            this.Propertys.BusyDialogProgressMessage = string.Format("{0} {1}/{2} {3}%", taskName, cmp, totalWork, pcnt);
        }

        /// <summary>
        /// インジケーターを閉じる
        /// </summary>
        private void CloseBusyIndicator()
        {
            this.Propertys.IsBusyDialog = false;
        }

	}
    #endregion

    #region プロパティクラス
    /// <summary>
    /// プロパティクラス
    /// </summary>
    public class MainWindowViewModelProperty : TacticsProperty
    {
        /// <summary>
        /// IsBusyDialog
        /// </summary>
        public virtual bool IsBusyDialog { get; set; }

        /// <summary>
        /// BusyDialogMessageTitle
        /// </summary>
        public virtual string BusyDialogMessageTitle { get; set; }

        /// <summary>
        /// BusyDialogProgressMessage
        /// </summary>
        public virtual string BusyDialogProgressMessage { get; set; }

        /// <summary>
        /// BusyDialogPcent
        /// </summary>
        public virtual double BusyDialogPcent { get; set; }

        /// <summary>
        /// Gravatarアイコン表示用ID
        /// </summary>
        public virtual string GravatarId { get; set; }
    }
    #endregion

    #region コマンドクラス
    /// <summary>
    /// コマンドクラス
    /// </summary>
    public class MainWindowViewModelCommand
    {
        /// <summary>
        /// ウインドウクローズキャンセルコマンド
        /// </summary>
        public TacticsCommand WindowCloseCancel { get; private set; }

        /// <summary>
        /// 設定変更
        /// </summary>
        public TacticsCommand Config { get; private set; }

        /// <summary>
        /// 公開鍵作成
        /// </summary>
        public TacticsCommand KeypairGeneration { get; private set; }

        /// <summary>
        /// リポジトリのクローン
        /// </summary>
        public TacticsCommand RepositoryClone { get; private set; }
    }
    #endregion
}
