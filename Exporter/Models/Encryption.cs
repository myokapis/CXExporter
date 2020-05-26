using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Models
{

    public class Encryption
    {
        public bool Enabled { get; set; } = false;
        public string FilePattern { get; set; }
        public string KeyFileName { get; set; }
    }

}
