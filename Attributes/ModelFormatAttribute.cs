using System;

namespace ExportAttributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class ModelFormatAttribute : Attribute
    {
        /// <summary>
        /// The data type to which this formatter applies
        /// </summary>
        public Type Type { get; protected set; }

        /// <summary>
        /// Format string associated with the attribute
        /// </summary>
        public string FormatString { get; protected set; }
    }

    public class DateFormatAttribute : ModelFormatAttribute
    {
        public DateFormatAttribute(string formatString)
        {
            Type = typeof(DateTime);
            FormatString = formatString;
        }
    }

    public class IntFormatAttribute : ModelFormatAttribute
    {
        public IntFormatAttribute(string formatString)
        {
            Type = typeof(int);
            FormatString = formatString;
        }
    }

    public class LongFormatAttribute : ModelFormatAttribute
    {
        public LongFormatAttribute(string formatString)
        {
            Type = typeof(long);
            FormatString = formatString;
        }
    }

    public class StringFormatAttribute : ModelFormatAttribute
    {
        public StringFormatAttribute(string formatString)
        {
            Type = typeof(string);
            FormatString = formatString;
        }
    }

}
