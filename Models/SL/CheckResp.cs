using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.SL
{
    public class CheckResp
    {
        public string department { get; set; }
        public string number { get; set; }
        public string currency { get; set; }
        public string processing { get; set; }
        public string name { get; set; }
        public string allowedCreditFl { get; set; }
        public string cardFl { get; set; }
        public string identifierStatus { get; set; }
        public string maskedName { get; set; }
        public string blackListFl { get; set; }
        public string clicode { get; set; }
    }
}
