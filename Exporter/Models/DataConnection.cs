using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Models
{

    public class DataConnection
    {
        public int ConnectTimeout { get; set; } = 30;
        public string Database { get; set; }
        public string Name { get; set; }
        public string SqlInstance { get; set; }
    }

}
