using System.Collections.Generic;
using Exporter.Services;

namespace Exporter.Models
{

    public class FieldInfo
    {
        public int AllFieldCount { get; set; }
        public List<Field> AllFields { get; set; }
        public int HFieldCount { get; set; }
        public List<Field> HFields { get; set; }
        public string KeyColumnName { get; set; }
        public List<int> MultiIndices { get; set; }
        //public ProcessMethod[] ProcessMethods { get; set; }
        public List<Field> VFields { get; set; }
        public int VFieldCount { get; set; }
        public Dictionary<object, Field> VFieldsByType { get; set; }
    }

}
