using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.CommonModels
{
    public class TransferState
    {
        public string state { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
