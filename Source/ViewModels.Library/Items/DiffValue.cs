using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NGit.Diff;
using Livet;

namespace BluePlumGit.Items
{
    [DataContract]
    public class DiffValue : ViewModel
    {
        private bool _Check;

        public bool Check
        {
            get
            {
                return _Check;
            }
            set
            {
                if (_Check == value)
                {
                    return;
                }
                _Check = value;
                RaisePropertyChanged("Check");
            }
        }

		[DataMember]
		public string Filename { get; set; }

        [DataMember]
        public string Oldname { get; set; }

        [DataMember]
        public string ChangeTypeName { get; set; }

        [DataMember]
        public DiffEntry.ChangeType ChangeType { get; set; }

        public override string ToString()
		{
            return string.Format("{0}\n", this.Filename);
		}
        
    }
}
