using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GordiasClassLibrary.Headquarters;
using GordiasClassLibrary.Interfaces;

namespace GordiasClassLibrary.Headquarters
{
    public class TacticsProperty : NotificationProvider, IRaisePropertyChanged
    {
        /// <summary>
        /// NotifyPropertyChangedイベントを発行する
        /// </summary>
        /// <param name="name">プロパティ名</param>
        public void OnRaisePropertyChanged(string name)
        {
            this.RaisePropertyChanged(name);
        }
    }
}
