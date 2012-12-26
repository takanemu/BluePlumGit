using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VS2012LikeWindow.Views;

namespace MetroLikeWindow
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static new App Current
        {
            get { return (App)Application.Current; }
        }

        internal ThemeService ThemeService { get; private set; }

        public App()
        {
            this.ThemeService = new ThemeService(this);
        }
    }
}
