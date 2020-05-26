using System;
using ExportAttributes;

namespace ExportDefinitions.Models.LionKing
{

    [SimpleModel]
    public class Nallah
    {
        public int LionId { get; set; }
        public string BestFriend { get; set; }
        public DateTime Anniversary { get; set; }
    }

}
