
namespace GitlabTool.ViewModels
{
    using Common.Library.Entitys;
    using Common.Library.Enums;
    using Common.Library.Messaging.Windows;
    using GitlabTool;
    using GitlabTool.Models;
    using Gordias.Library.Headquarters;
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

        #region Loadedメソッド
        /// <summary>
        /// Loadedメソッド
        /// </summary>
        protected override void LoadedHandlerOverride(object sender, RoutedEventArgs e)
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

        #region Closedメソッド
        /// <summary>
        /// Closedメソッド
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        protected override void ClosedHandlerOverride(object sender, EventArgs e)
        {
            this.model.SaveConfig(this.config);
        }
        #endregion

        #region 設定変更コマンド
        /// <summary>
        /// 設定変更コマンド
        /// </summary>
        [Command]
        private void Config()
        {
            /*
            WindowOpenMessage message = this.Messenger.GetResponse<WindowOpenMessage>(new WindowOpenMessage
            {
                MessageKey = "OpenWindow",
                WindowType = WindowTypeEnum.CONFIG,
                Parameter = this.globalConfig,
            });

            if (message.Response != null)
            {
                RepositoryEntity entity = (RepositoryEntity)message.Response.Result;
            }
            */
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

        [Command]
        private void ChangePurple()
		{
            this.config.Accent = AccentEnum.Purple;
            DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);
		}
        
        [Command]
        private void ChangeBlue()
		{
            this.config.Accent = AccentEnum.Blue;
            DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);
        }
        
        [Command]
        private void ChangeOrange()
		{
            this.config.Accent = AccentEnum.Orange;
            DataLogistics.Instance.SetValue(ApplicationEnum.Theme, this.config.Accent);
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
        /// 設定変更
        /// </summary>
        public TacticsCommand Config { get; private set; }

        /// <summary>
        /// 公開鍵作成
        /// </summary>
        public TacticsCommand KeypairGeneration { get; private set; }

        /// <summary>
        /// リポジトリの登録
        /// </summary>
        public TacticsCommand ChangePurple { get; private set; }
        
        /// <summary>
        /// リポジトリの登録
        /// </summary>
        public TacticsCommand ChangeBlue { get; private set; }

        /// <summary>
        /// リポジトリの登録
        /// </summary>
        public TacticsCommand ChangeOrange { get; private set; }
    }
    #endregion
}
