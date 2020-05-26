using System.Collections.Generic;

namespace Exporter.Models
{

    public class Sql
    {
        public string CommandName { get; set; }
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; } = 60;
        public string ConnectionName { get; set; } = "Default";
        public Dictionary<string, object> SqlParams { get; set; }
    }

}
