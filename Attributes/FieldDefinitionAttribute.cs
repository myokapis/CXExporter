using System;
using System.Linq;

namespace ExportAttributes
{

    /// <summary>
    /// Abstract class for building format attributes that can be applied to properties to
    /// define the way the property data should be represented as text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class FieldDefinitionAttribute : Attribute
    {
        public int[] CaseItemIds { get; protected set; }
        public string Category { get; protected set; }
        public string ColumnName { get; protected set; }
        public string ExtraData { get; protected set; }
        public FieldType FieldType { get; protected set; }
        public object FilterValue { get; protected set; }
        public string FilterColumn { get; protected set; }

        /// <summary>
        /// Format string associated with the attribute
        /// </summary>
        public string FormatString { get; protected set; }

        public string Header { get; protected set; }
        public bool IsHidden { get; protected set; } = false;
        public bool IsVerticalData { get; protected set; } = false;
        public string MultiRowDelimiter { get; protected set; }
        public int[] QuestionIds { get; protected set; }
        public string[] QuestionTags { get; protected set; }
        public string RecordKey { get; protected set; }
        public string RecordType { get; protected set; }
        public int[] ScaleIds { get; protected set; }

        protected int[] ParseToIntArray(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            return input.Split(",")
                .Select(s => int.Parse(s.Trim()))
                .ToArray();
        }
    }

    public class CaseItemAttribute : FieldDefinitionAttribute
    {

        public CaseItemAttribute(string caseItemIds, string columnName = null, string header = null,
            string formatString = null, FieldType fieldType = FieldType.Default, string delimiter = null,
            object filterValue = null, string filterColumn = null, string extraData = null)
        {
            var ids = ParseToIntArray(caseItemIds);
            CaseItemIds = ids;
            ColumnName = columnName;
            ExtraData = extraData;
            FieldType = fieldType;
            FilterValue = filterValue;
            FilterColumn = filterColumn;
            FormatString = formatString;
            Header = header;
            IsVerticalData = true;
            MultiRowDelimiter = delimiter;
            RecordKey = string.Join("|", ids);
            RecordType = "CaseItem";
        }
    }

    public class FieldAttribute : FieldDefinitionAttribute
    {
        public FieldAttribute(string columnName = null, string header = null,
            string formatString = null, FieldType fieldType = FieldType.Default, string delimiter = null,
            object filterValue = null, string filterColumn = null, string extraData = null)
        {
            ColumnName = columnName;
            ExtraData = extraData;
            FieldType = fieldType;
            FilterValue = filterValue;
            FilterColumn = filterColumn;
            FormatString = formatString;
            Header = header;
            MultiRowDelimiter = delimiter;
        }
    }

    public class HiddenAttribute : FieldDefinitionAttribute
    {
        public HiddenAttribute()
        {
            IsHidden = true;
        }
    }

    public class QuestionAttribute : FieldDefinitionAttribute
    {
        public QuestionAttribute(string questionTags, string questionIds = null, string scaleIds = null,
            string columnName = null, string header = null, string formatString = null,
            FieldType fieldType = FieldType.Default, string delimiter = null, string filterValue = null,
            string filterColumn = null, string extraData = null)
        {
            var qIds = ParseToIntArray(questionIds);
            var sIds = ParseToIntArray(scaleIds);

            var qTags = questionTags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                ?.Select(s => s.Trim())
                ?.ToArray();

            var keys = new string[] 
            {
                qTags == null ? null : string.Join("~", qTags),
                qIds == null ? null : string.Join("~", qIds),
                sIds == null ? null : string.Join("~", sIds)
            };

            ColumnName = columnName;
            ExtraData = extraData;
            FieldType = fieldType;
            FilterValue = filterValue;
            FilterColumn = filterColumn;
            FormatString = formatString;
            Header = header;
            IsVerticalData = true;
            MultiRowDelimiter = delimiter;
            QuestionIds = qIds;
            QuestionTags = qTags;
            RecordKey = string.Join("|", keys.Where(k => !string.IsNullOrWhiteSpace(k)));
            RecordType = "Question";
            ScaleIds = sIds;
        }

        public class VerticalFieldAttribute : FieldDefinitionAttribute
        {
            public VerticalFieldAttribute(string recordType, string columnName = null, string header = null,
                string formatString = null, FieldType fieldType = FieldType.Default, string delimiter = null,
                string filterValue = null, string filterColumn = null, string extraData = null, string recordKey = null)
            {
                ColumnName = columnName;
                ExtraData = extraData;
                FieldType = fieldType;
                FilterValue = filterValue;
                FilterColumn = filterColumn;
                FormatString = formatString;
                Header = header;
                IsVerticalData = true;
                MultiRowDelimiter = delimiter;
                RecordKey = recordKey;
                RecordType = recordType;
            }
        }

    }

}
