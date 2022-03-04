using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.CommonModels
{
    public class Originator
    {
        public Identification identification { get; set; }
        public Participant participant { get; set; }
        public string fullName { get; set; }
        public List<Identification> additionalIdentification { get; set; }
    }
}
