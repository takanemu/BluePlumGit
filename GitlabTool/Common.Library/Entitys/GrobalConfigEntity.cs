using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Common.Library.Entitys
{
    [DisplayName("グローバル設定")]
    public class GrobalConfigEntity : ICloneable 
    {
        [Category("Core")]
        [DisplayName("EMail")]
        [Description("電子メールアドレスを設定してください。")]
        public string EMail
        {
            get;
            set;
        }

        [Category("Core")]
        [DisplayName("Name")]
        [Description("ユーザー名称を設定してください。")]
        public string Name
        {
            get;
            set;
        }

        public virtual object Clone()
        {
            GrobalConfigEntity instance = (GrobalConfigEntity)Activator.CreateInstance(GetType());

            instance.EMail = this.EMail;
            instance.Name = this.Name;

            return instance;
        }
    }
}
