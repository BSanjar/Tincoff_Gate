using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.ToXfer
{
    public class RateReqXfer
    {
        public string rateType { get; set; }
        public string currencyCode { get; set; }
        public string effectiveDate { get; set; }
    }
}
