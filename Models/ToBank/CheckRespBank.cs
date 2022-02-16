using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Models.CommonModels;

namespace Tincoff_Gate.Models.ToBank
{
    public class CheckRespBank
    {
        public string platformReferenceNumber { get; set; }
        public Receiver receiver { get; set; }
        public Ammount receivingAmount { get; set; }
        public DateTime checkDate { get; set; }
        public TransferState transferState { get; set; }
    }

}
