using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GordiasClassLibrary.Interfaces
{
    /// <summary>
    /// RaisePropertyChangedインターフェイス
    /// </summary>
    public interface IRaisePropertyChanged
    {
        /// <summary>
        /// NotifyPropertyChangedイベントを発行する
        /// </summary>
        /// <param name="name">プロパティ名</param>
        void OnRaisePropertyChanged(string name);
    }
}
