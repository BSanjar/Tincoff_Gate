using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Models.ToBank;

namespace Tincoff_Gate.Models
{
    public class ServiceResult
    {
        public string code { get; set; }
        public string descript { get; set; }
        public string result { get; set; }

        public CheckRespBank checkRespBank { get; set; }
        public AmlResponseMethod amlResp { get; set; }
    }
}
