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

namespace GitlabTool
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                        brush = new SolidColorBrush(Color.FromRgb(104, 33, 122));
                        break;
                    case AccentEnum.Blue:
                        brush = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                        break;
                    case AccentEnum.Orange:
                        brush = new SolidColorBrush(Color.FromRgb(202, 81, 0));
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
        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            // ロギングクラスの初期化
            this.Log4netSetting();

            // 未補足の例外を補足する
            System.AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// UIスレッド集約エラーハンドラ
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal("DispatcherUnhandledExceptionを補足しました。アプリケーションを強制終了します。");

            System.Exception ex = (System.Exception)e.Exception;

            logger.Fatal("例外種別:" + ex.GetType().ToString());
            logger.Fatal("メッセージ:" + ex.Message);
            logger.Fatal("スタックトレース:" + ex.StackTrace);

            MessageBox.Show("アプリケーションを強制終了します。");

            // 例外を補足済みにし、アプリケーションを終了させる
            e.Handled = true;
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 集約エラーハンドラ
        /// </summary>
        /// <param name="sender">イベント元</param>
        /// <param name="e">パラメーター</param>
        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            logger.Fatal("UnhandledExceptionを補足しました。アプリケーションを強制終了します。");

            System.Exception ex = (System.Exception)e.ExceptionObject;
            
            logger.Fatal("例外種別:" + ex.GetType().ToString());
            logger.Fatal("メッセージ:" + ex.Message);
            logger.Fatal("スタックトレース:" + ex.StackTrace);

            MessageBox.Show("アプリケーションを強制終了します。");

            // アプリケーションを終了させる
            System.Environment.Exit(1);
        }

        /// <summary>
        /// ロギングクラスの初期化
        /// </summary>
        private void Log4netSetting()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.setting.xml"));

            string roaming = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string appname = "GitlabTool";

            // ログ出力先の変更
            System.Reflection.Assembly entryAsm = System.Reflection.Assembly.GetEntryAssembly();

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
                                    string dir = roaming + @"\2FC\" + appname + @"\logs";

                                    System.IO.Directory.CreateDirectory(dir);

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
