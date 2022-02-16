using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.CommonModels
{
    public class ConversionRateSell
    {
        public string type { get; set; }
        public string settlementCurrency { get; set; }
        public string receivingCurrency { get; set; }
        public double rate { get; set; }
        public double baseRate { get; set; }
    }
}
