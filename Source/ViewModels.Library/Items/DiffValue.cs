using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BluePlumGit.Items
{
    [DataContract]
    public class DiffValue
    {
        [DataMember]
        public bool Check { get; set; }

		[DataMember]
		public string Filename { get; set; }

        [DataMember]
        public string Oldname { get; set; }

        [DataMember]
        public string ChangeType { get; set; }

        public override string ToString()
		{
            return string.Format("{0}\n", this.Filename);
		}
        
    }
}
