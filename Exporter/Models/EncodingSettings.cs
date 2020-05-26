using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Models
{

    public class EncodingSettings
    {
        public int Bits { get; set; } = 8;
        public string Endian { get; set; } = "BE";
        public bool UseBOM { get; set; } = false;
    }

}
