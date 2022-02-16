using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.CommonModels
{
    public class PartList
    {
        public int order { get; set; }
        public string localizedName { get; set; }
        public string logo { get; set; }
        public string country { get; set; }
        public int displayFee { get; set; }
        public int participantId { get; set; }
        public List<string> currencies { get; set; }
    }
}
