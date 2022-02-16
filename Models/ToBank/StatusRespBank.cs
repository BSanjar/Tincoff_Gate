using System;
using Tincoff_Gate.Models.CommonModels;

namespace Tincoff_Gate.Models.ToBank
{
    public class StatusRespBank
    {
        public string platformReferenceNumber { get; set; }
        public Receiver receiver { get; set; }
        public Ammount receivingAmount { get; set; }
        public DateTime receivedDate { get; set; }
        public TransferState transferState { get; set; }
    }
}
