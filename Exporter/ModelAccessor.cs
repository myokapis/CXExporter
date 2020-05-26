//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Reflection;
//using ExportAttributes;

//// TODO: redo ModelAccessor and ModelAccessor<T>. All I need to do is use reflection to create a model 
////       containing an array of field objects that have the data accessor name, formatter function, etc.
////       there will be a single instance of the the model definition class. DataMapper can go away and be
////       replaced with a simple loop that grabs each field in the model definition and uses it to
////       obtain the database value and format it to a string

//namespace Exporter
//{

//    // TODO: add headers
//    public interface IModelAccessor
//    {

//        /// <summary>
//        /// Count of accessible property fields on the data object
//        /// </summary>
//        int Count { get; }

//        IEnumerable<string> FieldNames { get; }

//        /// <summary>
//        /// A collection of key-value pairs containing each field name and its corresponding value
//        /// </summary>
//        IEnumerable<KeyValuePair<string, string>> FieldValues { get; }

//        ///// <summary>
//        ///// The data object for which properties will be accessed
//        ///// </summary>
//        //T Model { get; }

//        /// <summary>
//        /// Returns a field value based on a field index
//        /// </summary>
//        /// <param name="fieldIndex">The index of the field to be accessed</param>
//        /// <returns>A string representing the value of the field</returns>
//        string this[int fieldIndex] { get; }

//        /// <summary>
//        /// Returns a field value based on a field name
//        /// </summary>
//        /// <param name="fieldName">The name of the field to be accessed</param>
//        /// <returns>A string representing the value of the field</returns>
//        string this[string fieldName] { get; }

//    }

//    /// <summary>
//    /// Allows a data object's properties to be accessed by index or name.
//    /// Also allows a data object's properties to be iterated.
//    /// </summary>
//    /// <typeparam name="T">Type of the data object</typeparam>
//    public class ModelAccessor<T> : IModelAccessor
//    {

//        private static List<PropertyInfo> _fieldProperties = null;
//        private static object[] _indexes = new object[] { };

//        /// <summary>
//        /// Creates a model accessor from a data object
//        /// </summary>
//        /// <param name="model">The data object for which properties will be accessed</param>
//        public ModelAccessor(T model)
//        {
//            this.Model = model;
//        }

//        /// <summary>
//        /// Count of accessible property fields on the data object
//        /// </summary>
//        public int Count
//        {
//            get
//            {
//                return FieldProperties.Count;
//            }
//        }

//        public IEnumerable<string> FieldNames
//        {
//            get
//            {
//                foreach (PropertyInfo pi in FieldProperties)
//                {
//                    yield return pi.Name;
//                }
//            }
//        }

//        /// <summary>
//        /// A collection of key-value pairs containing each field name and its corresponding value
//        /// </summary>
//        public IEnumerable<KeyValuePair<string, string>> FieldValues
//        {
//            get
//            {
//                foreach (PropertyInfo pi in FieldProperties)
//                {
//                    yield return new KeyValuePair<string, string>(pi.Name, FormatValue(pi));
//                }
//            }
//        }

//        /// <summary>
//        /// The data object for which properties will be accessed
//        /// </summary>
//        public T Model { get; }

//        /// <summary>
//        /// Returns a field value based on a field index
//        /// </summary>
//        /// <param name="fieldIndex">The index of the field to be accessed</param>
//        /// <returns>A string representing the value of the field</returns>
//        public string this[int fieldIndex]
//        {
//            get
//            {
//                var pi = FieldProperties[fieldIndex];
//                return FormatValue(pi);
//            }
//        }

//        /// <summary>
//        /// Returns a field value based on a field name
//        /// </summary>
//        /// <param name="fieldName">The name of the field to be accessed</param>
//        /// <returns>A string representing the value of the field</returns>
//        public string this[string fieldName]
//        {
//            get
//            {
//                PropertyInfo pi = FieldProperties.Find(p => p.Name == fieldName);
//                return FormatValue(pi);
//            }
//        }

//        #region "protected methods"

//        /// <summary>
//        /// A collection of <see cref="PropertyInfo"/> for public properties of this class
//        /// </summary>
//        protected static List<PropertyInfo> FieldProperties
//        {
//            get
//            {
//                if (_fieldProperties == null) GetFieldProperties();
//                return _fieldProperties;
//            }
//        }

//        /// <summary>
//        /// Formats a property's value using a custom format attribute if one exists. Otherwise
//        /// formatting uses the data type's default ToString() behavior.
//        /// </summary>
//        /// <param name="pi">The property whose value will be formatted</param>
//        /// <returns>A formatted string representing the property's value</returns>
//        protected string FormatValue(PropertyInfo pi)
//        {
//            object value = pi.GetValue(Model);
//            if (value == null) return "";

//            var formatter = (FormatAttribute)Attribute.GetCustomAttribute(pi, typeof(FormatAttribute));
//            return formatter == null ? value.ToString() : formatter.FormatData(value);
//        }

//        /// <summary>
//        /// A static method that finds public properties of the class. Any properties marked with
//        /// a [NotMapped] attribute are ignored.
//        /// </summary>
//        protected static void GetFieldProperties()
//        {
//            _fieldProperties = new List<PropertyInfo>();

//            foreach (PropertyInfo pi in typeof(T).GetProperties())
//            {
//                if (!Attribute.IsDefined(pi, typeof(NotMappedAttribute))) _fieldProperties.Add(pi);
//            }
//        }

//#endregion
//    }

//}
