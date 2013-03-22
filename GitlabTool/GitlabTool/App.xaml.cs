#region License
// <copyright>
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
// </copyright>
#endregion

namespace GitlabTool
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Common.Library.Enums;
    using Gordias.Library.Headquarters;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Repository;

    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ログ
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public App()
        {
            DataLogistics.Instance.Change += (object sender, PropertyChangeEventArgs e) =>
            {
                AccentEnum accent = (AccentEnum)DataLogistics.Instance.GetValue(ApplicationEnum.Theme);

                SolidColorBrush brush = null;

                switch (accent)
                {
                    case AccentEnum.Purple:
                        brush = new SolidColorBrush(Colors.Purple);
                        break;
                    case AccentEnum.Blue:
                        brush = new SolidColorBrush(Colors.Blue);
                        break;
                    case AccentEnum.Orange:
                        brush = new SolidColorBrush(Colors.Orange);
                        break;
                    case AccentEnum.Yellow:
                        brush = new SolidColorBrush(Colors.Yellow);
                        break;
                    case AccentEnum.Green:
                        brush = new SolidColorBrush(Colors.Lime);
                        break;
                    case AccentEnum.White:
                        brush = new SolidColorBrush(Colors.White);
                        break;
                    case AccentEnum.SkyBlue:
                        brush = new SolidColorBrush(Colors.DeepSkyBlue);
                        break;
                    case AccentEnum.Red:
                        brush = new SolidColorBrush(Colors.Crimson);
                        break;
                    case AccentEnum.Pink:
                        brush = new SolidColorBrush(Colors.DeepPink);
                        break;
                    case AccentEnum.Gray:
                        brush = new SolidColorBrush(Colors.Silver);
                        break;
                }
                this.Resources["AccentBrushKey"] = brush;
            };
            DataLogistics.Instance.SetValue(ApplicationEnum.Theme, AccentEnum.Blue);
        }

        /// <summary>
        /// アプリケーション起動
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // ロギングクラスの初期化
            this.Log4netSetting();

            // 未補足の例外を補足する
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// UIスレッド集約エラーハンドラ
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal("DispatcherUnhandledExceptionを補足しました。アプリケーションを強制終了します。");

            Exception ex = (Exception)e.Exception;

            logger.Fatal("例外種別:" + ex.GetType().ToString());
            logger.Fatal("メッセージ:" + ex.Message);
            logger.Fatal("スタックトレース:" + ex.StackTrace);

            MessageBox.Show("アプリケーションを強制終了します。");

            // 例外を補足済みにし、アプリケーションを終了させる
            e.Handled = true;
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 集約エラーハンドラ
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal("UnhandledExceptionを補足しました。アプリケーションを強制終了します。");

            Exception ex = (Exception)e.ExceptionObject;
            
            logger.Fatal("例外種別:" + ex.GetType().ToString());
            logger.Fatal("メッセージ:" + ex.Message);
            logger.Fatal("スタックトレース:" + ex.StackTrace);

            MessageBox.Show("アプリケーションを強制終了します。");

            // アプリケーションを終了させる
            Environment.Exit(1);
        }

        /// <summary>
        /// ロギングクラスの初期化
        /// </summary>
        private void Log4netSetting()
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            XmlConfigurator.Configure(new FileInfo(baseDir + @"\log4net.setting.xml"));

            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appname = "GitlabTool";

            // ログ出力先の変更
            Assembly entryAsm = Assembly.GetEntryAssembly();

            if (entryAsm != null)
            {
                foreach (ILoggerRepository repository in LogManager.GetAllRepositories())
                {
                    foreach (IAppender appender in repository.GetAppenders())
                    {
                        if (appender.Name.Equals("AppRollingFileAppender"))
                        {
                            FileAppender fileAppender = appender as FileAppender;

                            if (fileAppender != null)
                            {
                                string file = fileAppender.File;
                                if (!string.IsNullOrEmpty(file))
                                {
                                    string dir = roaming + @"\2ndfactory\" + appname + @"\logs";

                                    Directory.CreateDirectory(dir);

                                    // ファイル名の変更
                                    fileAppender.File = dir + @"\gitlabtool.log";
                                    fileAppender.ActivateOptions();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
