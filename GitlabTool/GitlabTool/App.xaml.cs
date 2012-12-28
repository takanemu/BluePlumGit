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

namespace GitlabTool
{
    using Common.Library.Enums;
    using Gordias.Library.Headquarters;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 
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
    }
}
