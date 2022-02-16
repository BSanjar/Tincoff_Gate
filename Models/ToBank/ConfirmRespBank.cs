using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Models.CommonModels;

namespace Tincoff_Gate.Models.ToBank
{
    public class ConfirmRespBank
    {
        public string platformReferenceNumber { get; set; }
        public TransferState transferState { get; set; }
    }
}
