using System;
using Tincoff_Gate.Models.CommonModels;

namespace Tincoff_Gate.Models.ToXfer
{
    public class StatusRespXfer
    {
        public string platformReferenceNumber { get; set; }
        public Originator originator { get; set; }
        public Receiver receiver { get; set; }
        public Ammount paymentAmount { get; set; }
        public Ammount displayFeeAmount { get; set; }
        public Ammount feeAmount { get; set; }
        public Ammount settlementAmount { get; set; }
        public Ammount receivingAmount { get; set; }
        public DateTime checkDate { get; set; }
        public DateTime transferDate { get; set; }
        public DateTime receivedDate { get; set; }
        public ConversionRateSell conversionRateSell { get; set; }
        public ConversionRateBuy conversionRateBuy { get; set; }
        public TransferState transferState { get; set; }
    }
}
