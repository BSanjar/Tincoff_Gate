using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Models.CommonModels;

namespace Tincoff_Gate.Models.ToXfer
{
    public class CheckReqXfer
    {
        public string originatorReferenceNumber { get; set; }
        public string originatorSignature { get; set; }
        public Originator originator { get; set; }
        public Receiver receiver { get; set; }
        public Ammount paymentAmount { get; set; }
        public Ammount feeAmount { get; set; }
        public Ammount receivingAmount { get; set; }
        public ConversionRateSell conversionRateSell { get; set; }
    }

}
