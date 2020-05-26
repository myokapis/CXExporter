using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Models
{

    public class Compression
    {
        public bool Enabled { get; set; } = false;
        public string FilePattern { get; set; }
    }

}
