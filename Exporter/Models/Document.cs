using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Exporter.Models
{
    // TODO: create attributes to specify custom headers
    //       default to the field header values as long as they exist
    //       have a no header attribute that can be set?
    //       have a use field headers attribute that can be set?
    // TODO: create attributes to specify custom footers
    public class Document
    {
        public bool? ConditionalEscaping { get; set; } = null;
        public string DatePattern { get; set; } = "yyyyMMddHHmmss";
        public string Delimiter { get; set; } = ",";
        public EncodingSettings EncodingSettings { get; set;}
        public Regex EscapingRegex { get; set; }
        public Dictionary<Type, string> FieldFormats { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        public string FilePattern { get; set; }
        public Dictionary<string, Regex> JsonCleaner { get; set; }
        public Dictionary<char, string> JsonReplacements { get; set; }
        public string KeyColumnName { get; set; }
        public string ModelName { get; set; }
        public string NullValue { get; set; } = "";
        public Dictionary<string, Regex> TextCleaner { get; set; }
        public Dictionary<char, string> TextReplacements { get; set; }
    }

}
