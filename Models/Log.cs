using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models
{
    public class Log
    {
        public string id { get; set; } 
        public string idCheck { get; set; } 
        public string idPlatform { get; set; } //referenceIdOrQueryId
        public string state { get; set; } //INVALID, CHECKED, CONFIRMED,CONFIRM_PENDING,CHECK_PENDING
        public string comment { get; set; }
        public string originatorid { get; set; }
        public string originatorBank { get; set; } 
        public double originatorSumm { get; set; }  
        
        public string checkRequest { get; set; }
        public string checkResponse { get; set; }

        public string payRequest { get; set; }
        public string payResponse { get; set; }

        public string colvirPayRequest { get; set; }
        public string colvirPayResponse { get; set; }

        public string amlRequest { get; set; }
        public string amlResponse { get; set; }

        public string amlRequestRec { get; set; }
        public string amlResponseRec { get; set; }

        public string stateRequest { get; set; }
        public string stateResponse { get; set; }

        public DateTime checkDate { get; set; }
        public DateTime payDate { get; set; }
        public DateTime colvirPayDate { get; set; }
        public DateTime amlCheckDate { get; set; }
        public DateTime amlCheckDateRec { get; set; }
        public DateTime stateDate { get; set; }

    }
}
