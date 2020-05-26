using System;
using ExportAttributes;

namespace ExportDefinitions.Models.LionKing
{

    [CompositeModel("Id")]
    [DateFormat("yyyy-MM-dd")]
    public class Symba
    {
        public DateTime? Birthdate { get; set; }
        public string Claws { get; set; }
        public string Eyes { get; set; }
        public int Nose { get; set; }

        [Question("Tag1,Tag2", filterValue: "", formatString: "{0, 14}")]
        public int? Teeth { get; set; }
    }

}
