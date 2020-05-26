using System;
using System.Collections.Generic;
using System.Text;

namespace Exporter.Models
{

    public class Process
    {
        public int ArchiveRetentionDays { get; set; }
        public string Code { get; set; }
        public Compression Compression { get; set; }
        public Document[] Documents { get; set; }
        public string Encoding { get; set; }
        public Encryption Encryption { get; set; }
        public Dictionary<Type, string> FieldFormats { get; set; }
        public int LogRetentionDays { get; set; }
        public Dictionary<string, object> Misc { get; set; }
        public string OutputDirectory { get; set; }
        public Sftp Sftp { get; set; }
        public Sql[] Sql { get; set; }
    }

}
