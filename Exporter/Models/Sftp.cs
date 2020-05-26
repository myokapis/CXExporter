using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Models
{

    public class Sftp
    {
        public bool Enabled { get; set; } = false;
        public string Host { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public string User { get; set; }
    }

}
