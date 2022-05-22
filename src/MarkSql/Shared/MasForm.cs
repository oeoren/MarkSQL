using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public  class MasForm
    {
        public string Procname { get; set; } = string.Empty;
        public List<MasField> Fields { get; set; } = new List<MasField>();


    }
}
