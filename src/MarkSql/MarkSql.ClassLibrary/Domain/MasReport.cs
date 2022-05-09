using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkSql.ClassLibrary.Domain
{
    /// <summary>
    /// Parameters declared on SQL Server
    /// </summary>
    public class MasParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string MasType { get; set; }

        public string FormValue { get; set; }

    }
    public  class MasReport
    {
        public string Procname { get; set; } = string.Empty;
        public List<MasParameter> Parameters { get; set; } = new List<MasParameter>();


    }
}
