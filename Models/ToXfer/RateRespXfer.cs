using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.ToXfer
{
    public class RateRespXfer
    {
        public string errCode { get; set; }
        public string errMessage { get; set; }
        public string effectiveDate { get; set; }
        public List<ExchangeRatesList> exchangeRatesList { get; set; }
    }


    public class ExchangeRatesList
    {
        public string currencyCode { get; set; }
        public double sellRate { get; set; }
        public double buyRate { get; set; }
        public int participantId { get; set; }
    }
}
