using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models
{
    public class Log
    {
        public string id { get; set; } //referenceIdOrQueryId
        public string Status { get; set; } //info, warning, error
        public string EventName { get; set; } 
        public string Descript { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public DateTime LogDate { get; set; }
    }
}
