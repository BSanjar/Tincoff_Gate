using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models.CommonModels
{
    public class Ammount
    {
        public string? amount { get; set; }
        public string currency { get; set; }
      

        public void ReplaceSplitter(string splitter, string newSplitter)
        {
            amount = amount.Replace(splitter, newSplitter);
        }
    }
}
