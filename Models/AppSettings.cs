namespace Tincoff_Gate.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string hostXref { get; set; }
        public string hostSL { get; set; }
        public string hostEsb { get; set; }
        public string EsbLogin { get; set; }
        public string EsbPassw { get; set; }
        public string pointSL { get; set; }
        public string certNameSL { get; set; }
        public string certPasswordSL { get; set; }
        public string ServiceIdSL { get; set; }
        public string EsbPayCard { get; set; }
        public string EsbPayAcc { get; set; }

        //ревизиты счета списания
        public string pointAccNum { get; set; }
        public string pointAccDep { get; set; }
        public string pointAccCur { get; set; }
        public string pointAccProc { get; set; }
        public string pointAccName { get; set; }
        public string pointAccInn { get; set; }
        public string source { get; set; }
        public string participantId { get; set; }

        public string LogAll { get; set; }
        public string countTrnDay { get; set; }
        public string sumTrnDay { get; set; }
        public string sumTrnMonth { get; set; }


        //
        public string proxyAddr { get; set; }
        public string proxyLogin { get; set; }
        public string proxyPassw { get; set; }

    }
}
