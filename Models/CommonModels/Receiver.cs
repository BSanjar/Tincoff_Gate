using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.CommonModels
{
    public class Receiver
    {
        public Identification identification { get; set; }
        public Participant participant { get; set; }
        public string displayName { get; set; }
        public List<string> currencies { get; set; }
    }
}
