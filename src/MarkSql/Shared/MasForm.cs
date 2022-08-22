using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

namespace MarkSql.Shared
{
    /// <summary>
    /// Parameters declared on SQL Server
    /// </summary>
    public class MasField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string MasType { get; set; }

        public string FormValue { get; set; }

    }
    public class MasForm
    {
        private string? name;
        public string Name {
            get
            {
                if (string.IsNullOrEmpty(this.name))
                {
                    var name = Procname.Substring(Procname.IndexOf('_') + 1);
                    
                    return name.Humanize();
                }
                return Name;
            }
            set { name = value; } 
        }

        public string Procname { get; set; } = String.Empty;
        public List<MasField> Fields { get; set; } = new List<MasField>();

    }
}
