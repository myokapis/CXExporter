using System;
using System.Collections.Generic;
using System.Text;

namespace ExportAttributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class ModelDefinitionAttribute : Attribute
    {
        // TODO: maybe make these protected set
        public string DatePattern { get; set; }
        public string Delimiter { get; set; }
        public string FilePattern { get; set; }
        public bool IsComposite { get; set; }
        public string KeyColumnName { get; set; }
        public string NullValue { get; set; }
    }

    public class SimpleModelAttribute : ModelDefinitionAttribute
    {
        public SimpleModelAttribute(string filePattern = null, string datePattern = "yyyyMMddHHmmss",
            string delimiter = ",", string nullValue = "")
        {
            DatePattern = datePattern;
            Delimiter = delimiter;
            FilePattern = filePattern;
            IsComposite = false;
            NullValue = nullValue;
        }
    }

    public class CompositeModelAttribute : ModelDefinitionAttribute
    {
        public CompositeModelAttribute(string keyColumnName, string filePattern = null,
            string datePattern = "yyyyMMddHHmmss", string delimiter = ",", string nullValue = "")
        {
            DatePattern = datePattern;
            Delimiter = delimiter;
            FilePattern = filePattern;
            IsComposite = true;
            KeyColumnName = keyColumnName;
            NullValue = nullValue;
        }
    }

}
