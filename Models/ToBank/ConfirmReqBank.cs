using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Models.CommonModels;

namespace Tincoff_Gate.Models.ToBank
{
    public class ConfirmReqBank
    {
        public string platformReferenceNumber { get; set; }
        public string platformSignature { get; set; }
        public Originator originator { get; set; }
        public Receiver receiver { get; set; }
        public Ammount paymentAmount { get; set; }
        public Ammount displayFeeAmount { get; set; }
        public List<AmmountWithType> feeAmount { get; set; }
        public Ammount settlementAmount { get; set; }
        public Ammount receivingAmount { get; set; }
        public DateTime checkDate { get; set; }
        public DateTime transferDate { get; set; }
        public ConversionRateBuy conversionRateBuy { get; set; }
        public ConversionRateSell conversionRateSell { get; set; }
        public string comment { get; set; }
    }
}
