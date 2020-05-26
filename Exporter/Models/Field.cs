using System;
using System.Collections.Generic;
using System.Text;
using ExportAttributes;
using Exporter.Services;

namespace Exporter.Models
{
    // TODO: consider making the arrays into lists
    public class Field
    {
        public int[] CaseItemIds { get; set; }
        public string Category { get; set; }
        public string ColumnName { get; set; }
        public int DisplayIndex { get; set; }
        public FieldType FieldType { get; set; } = FieldType.Default;
        public object FilterValue { get; set; }
        public string FilterColumn { get; set; }
        public string FormatString { get; set; }
        public bool HasFilter { get; set; }
        public string Header { get; set; }
        public int Index { get; set; }
        public bool IsHidden { get; set; } = false;
        public bool IsVerticalField { get; set; }
        public string MultiRowDelimiter { get; set; }
        public ProcessMethod ProcessMethod { get; set; }
        public int[] QuestionIds { get; set; }
        public string[] QuestionTags { get; set; }
        public string RecordType { get; set; }
        public string RecordTypeKey { get; set; }
        public int[] ScaleIds { get; set; }
    }

}
