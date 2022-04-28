using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tincoff_Gate.Models
{
  
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Body
    {
        public string COMMENT { get; set; }
        public string STATUS { get; set; }
        public string UID { get; set; }
        public string BANKOPERATIONID { get; set; }
        public string ISSUEDBID { get; set; }
    }

    public class AmlResponse
    {
        public string id { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string source { get; set; }
        public string dateTime { get; set; }
        public List<Body> body { get; set; }
    }


}
