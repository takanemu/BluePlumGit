
namespace Common.Library.Entitys
{
    using Common.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
    public class ConfigEntity
    {
        public string ServerUrl { get; set; }
        public string Password { get; set; }
        public List<RepositoryEntity> Repository { get; set; }
        public AccentEnum Accent { get; set; }

        public ConfigEntity()
        {
            this.ServerUrl = string.Empty;
            this.Password = string.Empty;
            this.Repository = new List<RepositoryEntity>();
            this.Accent = AccentEnum.Blue;
        }
    }
}
