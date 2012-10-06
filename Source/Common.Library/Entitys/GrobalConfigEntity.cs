using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BluePlumGit.Entitys
{
    public class GrobalConfigEntity : ICloneable 
    {
        [Category("Core")]
        [DisplayName("EMail")]
        [Description("This property uses a TextBox as the default editor.")]
        public string EMail
        {
            get;
            set;
        }

        [Category("Core")]
        [DisplayName("Name")]
        [Description("This property uses a TextBox as the default editor.")]
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
