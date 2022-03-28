using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tincoff_Gate.Models;
using Tincoff_Gate.Models.SL;
using Tincoff_Gate.Models.ToBank;
using Tincoff_Gate.Models.ToXfer;

namespace Tincoff_Gate.Integration
{
    public class Integration
    {
        private AppSettings _config;

        //private readonly ILogger<HomeController> _logger;

        private readonly IOptions<AppSettings> _appSettings;
        private readonly IOptions<ConnectionStrings> _connectionString;

        public Integration(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionString)
        {
            _appSettings = appSettings;
            _connectionString = connectionString;
        }
        public string GateWay(string body)
        {
            
            string addr = _appSettings.Value.hostEsb ;
            string response = SendRequestEsb(body, addr);
            return response;
        }


        private string SendRequestEsb(string request, string addr)
        {
            Logger logger = new Logger(_connectionString, _appSettings);
            string descript = "";
            string status = "info";
            string resp = "";
            try
            {
              
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                addr = addr.Trim('/').Trim('\\'); // в конце адреса удалить слэш, если он имеется
                ServicePointManager.ServerCertificateValidationCallback = ((senderr, certificate, chain, sslPolicyErrors) => true);
                WebRequest _request = HttpWebRequest.Create(addr);

                //метод POST/GET и.т.д
                _request.Method = "POST";
                _request.ContentType = "application/json";
                //_request.ContentLength = body.Length;

                //добавляю загаловки сервиса

                var TextBytes = Encoding.UTF8.GetBytes(_appSettings.Value.EsbLogin + ":" + _appSettings.Value.EsbPassw);
                string auth = "Basic " + Convert.ToBase64String(TextBytes);
                
                //_request.Headers.Add("Content-Type", "application/xml");
                _request.Headers.Add("Authorization", auth);


                // пишем тело
                StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
                _streamWriter.Write(request);
                _streamWriter.Close();
                // читаем тело
                WebResponse _response = _request.GetResponse();

                StreamReader _streamReader = new StreamReader(_response.GetResponseStream());
                //string _result = _streamReader.ReadToEnd(); // переменная в которую пишется результат (ответ) сервиса
                resp = _streamReader.ReadToEnd();

            }
            catch (Exception ex)
            {
                descript = ex.Message;
                status = "error";
                throw new Exception("Не удалось выполнить запрос к ESB, ошибка: " + ex.Message);
            }
            finally
            {
                logger.InsertLog(new Log { id = "", Descript = descript, EventName = "HbkGate->Esb", Status = status, request = request, response = resp });
            }
            return resp;
        }


        public CheckRespXfer CheckXfer(CheckReqXfer checkReqXfer)
        {
                      string body = JsonConvert.SerializeObject(checkReqXfer);
                string addr = _appSettings.Value.hostXref + "/transfer/check";
                string response = QureyToXfer(body,addr,"Check");
                CheckRespXfer resp = new CheckRespXfer();
                resp = JsonConvert.DeserializeObject<CheckRespXfer>(response);
                return resp;
        }
        public ConfirmRespXfer ConfirmXfer(ConfirmReqXfer confirmReqXfer, string addr)
        {
            string body = JsonConvert.SerializeObject(confirmReqXfer);
            addr = addr + "/transfer/confirm";
            string response = QureyToXfer(body, addr, "Confirm");
            ConfirmRespXfer resp = new ConfirmRespXfer();
            resp = JsonConvert.DeserializeObject<ConfirmRespXfer>(response);
            return resp;
        }
        public StatusRespXfer StatusXfer(StatusReqXfer ReqXfer, string addr)
        {
            string body = JsonConvert.SerializeObject(ReqXfer);
            addr = addr + "/transfer/state";
            string response = QureyToXfer(body, addr, "State");
            StatusRespXfer resp = new StatusRespXfer();
            resp = JsonConvert.DeserializeObject<StatusRespXfer>(response);
            return resp;
        }

        public RateRespXfer RateXfer(RateReqXfer ReqXfer, string addr)
        {
            string body = JsonConvert.SerializeObject(ReqXfer);
            addr = addr + "/exchangerates";
            string response = QureyToXfer(body, addr, "Rate");
            RateRespXfer resp = new RateRespXfer();
            resp = JsonConvert.DeserializeObject<RateRespXfer>(response);
            return resp;
        }
        private string QureyToXfer(string body, string addr, string func)
        {
            Logger logger = new Logger(_connectionString, _appSettings);
            string descript = "";
            string status = "info";
            string res = "";
            try
            {
                var data = Encoding.UTF8.GetBytes(body);
                //string addr = "https://146.120.245.16:7050/aggregator/v1/transactions/check";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |  SecurityProtocolType.Tls;
                //ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(addr);

                WebProxy myProxy = new WebProxy();
                //string ProxyAddr = ConfigurationManager.AppSettings["ProxyAddr"];
                //string ProxyLogin = ConfigurationManager.AppSettings["ProxyLogin"];
                //string ProxyPass = ConfigurationManager.AppSettings["ProxyPassword"];

                Uri newUri = new Uri(_appSettings.Value.proxyAddr) ;
                // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                myProxy.Address = newUri;
                // Create a NetworkCredential object and associate it with the 
                // Proxy property of request object.
                myProxy.Credentials = new NetworkCredential(_appSettings.Value.proxyLogin, _appSettings.Value.proxyPassw);
                req.Proxy = myProxy;


                //req.AllowAutoRedirect = true;
                //req.ClientCertificates = certificates;
                req.Method = "POST";

                //req.Proxy = new WebProxy() { UseDefaultCredentials = true };
                //req.Credentials = new NetworkCredential("000763", "Qwerty87", "hbk.dom");
                req.ContentType = "application/json";
                req.ContentLength = data.Length;

                //req.Headers.Add("X-Key", Xkey);
                //req.Headers.Add("X-App-UUID", XAppUuid);
                //req.Headers.Add("X-Request-Signature", sign);

                
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                using (var stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)req.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                res = responseString.ToString();
                descript = "Получен ответ";
                
               
                return res;
            }
            catch (Exception ex)
            {
                descript = ex.Message;
                status = "error";
                
                throw new Exception("Не удалось выполнить запрос к "+addr+", ошибка: " + ex.Message);
            }
            finally
            {
                logger.InsertLog(new Log { id = "", Descript = descript, EventName = "HbkGate->Xfer/"+func, Status = status, request = body, response = res });
            }
        }

        public string QureyToSL(string body, string func)
        {
            string certName = Directory.GetCurrentDirectory();
            certName = certName + "\\CertSL\\"+_appSettings.Value.certNameSL;
            Logger logger = new Logger(_connectionString, _appSettings);
            string descript = "";
            string status = "info";
            string res = "";
            try
            {
                var data = Encoding.UTF8.GetBytes(body);

                X509Certificate2Collection certificates = new X509Certificate2Collection();
                certificates.Import(certName, _appSettings.Value.certPasswordSL, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_appSettings.Value.hostSL);
                req.AllowAutoRedirect = true;
                req.ClientCertificates = certificates;
                req.Method = "POST";

                req.Proxy = new WebProxy() { UseDefaultCredentials = true };
                req.ContentType = "text/xml";
                req.ContentLength = data.Length;
              
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                using (var stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)req.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                res = responseString.ToString();
               
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(res);;
                descript = "Получен ответ";
               
                return xdoc.InnerXml;
            }
            catch (Exception ex)
            {
                descript = ex.Message;
                status = "error";
                throw new Exception("Не удалось выполнить запрос к " + _appSettings.Value.hostSL + ", ошибка: " + ex.Message);
            }
            finally
            {
                logger.InsertLog(new Log { id = "", Descript = descript, EventName = "HbkGate->PayLogic/"+func, Status = status, request = body, response = res });
            }
        }

        public CheckRespBank CheckBank (CheckReqBank req)
        {
            string body = CreateCheckPaymentQuery(req.receiver.identification.value); 
            string response = QureyToSL(body,"Check");

            CheckRespBank resp = new CheckRespBank();
            resp = CreateCheckPaymentResponse(response, req);

            return resp;
        }
        public ConfirmRespBank ConfirmBank(ConfirmReqBank req)
        {

            string checkBody = CreateCheckPaymentQuery(req.receiver.identification.value);
            string responseCheck = QureyToSL(checkBody,"Check");


            CheckResp cr = SLCheckResponce(responseCheck);

            string body = CreatePayPaymentQuery(req,cr);
            string response = QureyToSL(body,"Pay");
            ConfirmRespBank resp = new ConfirmRespBank();

            resp = CreateConfirmPaymentResponse(response);
            return resp;
        }
        public StatusRespBank StatusBank(StatusReqBank req)
        {
            string body = CreateStatusPaymentQuery(req);
            string response = QureyToSL(body,"State");
            StatusRespBank resp = new StatusRespBank();
            resp = CreateStatusPaymentResponse(response);
            return resp;
        }


        public string CreateCheckPaymentQuery(string acc)
        {
            string resp = "";
            try
            {
                resp = "<request point=\""+_appSettings.Value.pointSL+ "\">\n    <advanced service=\"" + _appSettings.Value.ServiceIdSL + "\" function=\"CheckAcc\" >\n        <attribute name=\"id1\" value=\"" + acc  + "\"/>\n    </advanced>\n</request>";
            }
            catch(Exception ex)
            {
                throw new Exception("Не удалось сформировать запрос в check-SL:"+ex.Message);
            }
            return resp;
        }
        public string CreatePayPaymentQuery(ConfirmReqBank req, CheckResp cr)
        {
            string resp = "";
            try
            {
                //platformReferenceNumber = guid sender + guid platform
                string guid = req.platformReferenceNumber.Substring(req.platformReferenceNumber.Count()/2, (req.platformReferenceNumber.Count()/2));
                double summWithComiss = Convert.ToDouble(req.receivingAmount.amount.Replace('.',','))* 100; //сумма с комиссией
                double summ = Convert.ToDouble(req.receivingAmount.amount.Replace('.', ',')) * 100;           //сумма без комиссии
                string date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+0300";
                string servId = _appSettings.Value.EsbPayAcc;
                string origAddr = "";
                if (req.originator.additionalIdentification!=null)
                foreach (var i in req.originator.additionalIdentification)
                {
                    origAddr+= "<attribute name=\"" + i.type + "\" value=\"" + i.value + "\" />\n        ";
                }

                if (cr.cardFl == "1")
                {
                    servId = _appSettings.Value.EsbPayCard;
                }
                resp = "<request point=\"" + _appSettings.Value.pointSL + "\">\n    "+
                    "<opayment id=\"" + guid + "\"\n sum=\"" + summ + "\"\n check=\"0\"\n service=\"" + _appSettings.Value.ServiceIdSL + "\"\n " +
                    "account=\"" + req.receiver.identification.value + "\"\n date=\"" + date + "\">\n        " +
                    "<attribute name=\"servId\" value=\"" + servId + "\" />\n        " +
                    "<attribute name=\"source\" value=\"" + _appSettings.Value.source + "\" />\n        " +
                    "<attribute name=\"pointAccDep\" value=\"" + _appSettings.Value.pointAccDep + "\" />\n        " +
                    "<attribute name=\"pointAccNum\" value=\"" + _appSettings.Value.pointAccNum + "\" />\n        " +
                    "<attribute name=\"pointAccCur\" value=\"" + _appSettings.Value.pointAccCur + "\" />\n        " +
                    "<attribute name=\"pointAccProc\" value=\"" + _appSettings.Value.pointAccProc + "\" />\n        " +
                    "<attribute name=\"pointAccInn\" value=\"" + _appSettings.Value.pointAccInn + "\" />\n        " +
                    "<attribute name=\"pointAccName\" value=\"" + _appSettings.Value.pointAccName + "\" />\n " +
                    "<attribute name=\"cliAccNum\" value=\"" + cr.number + "\" />\n        " +
                    "<attribute name=\"cliAccDep\" value=\"" + cr.department + "\" />\n        " +
                    "<attribute name=\"cliAccCurr\" value=\"" + cr.currency + "\" />\n        " +
                    "<attribute name=\"cliAccProc\" value=\"" + cr.processing + "\" />\n        " +
                    "<attribute name=\"cliAccName\" value=\"" + cr.name + "\" />\n        " +
                    "<attribute name=\"cliAccCardFl\" value=\"" + cr.cardFl + "\" />\n        " +
                    "<attribute name=\"description\" value=\"Пополнение счета от "+req.originator.identification.value+"\" />\n        " +
                    "<attribute name=\"origNum\" value=\"" + req.originator.identification.value + "\" />\n        " +
                    "<attribute name=\"origName\" value=\"" + req.originator.fullName + "\" />\n        " +
                    "<attribute name=\"origBank\" value=\"" + req.originator.participant.participantId + "\" />\n        " +
                        origAddr +
                    "<attribute name=\"payAmmount\" value=\"" + req.paymentAmount.amount + "\" />\n        " +
                    "<attribute name=\"payAmmountCurr\" value=\"" + req.paymentAmount.currency + "\" />\n        " +
                    "<attribute name=\"displayFeeAmount\" value=\"" + req.displayFeeAmount.amount + "\" />\n        " +
                    "<attribute name=\"displayFeeAmountCurr\" value=\"" + req.displayFeeAmount.currency + "\" />\n        " +
                    "<attribute name=\"feeAmount\" value=\"" + req.feeAmount.amount + "\" />\n        " +
                    "<attribute name=\"feeAmountCurr\" value=\"" + req.feeAmount.currency + "\" />\n        " +
                    "<attribute name=\"settlementAmount\" value=\"" + req.settlementAmount.amount + "\" />\n        " +
                    "<attribute name=\"settlementAmountCurr\" value=\"" + req.settlementAmount.currency + "\" />\n        " +
                    "<attribute name=\"rate\" value=\"" + req.conversionRateSell.rate + "\" />\n        " +
                    "<attribute name=\"baseRate\" value=\"" + req.conversionRateSell.baseRate + "\" />\n        " +
                    "<attribute name=\"cliAccInn\" value=\"\" />   " +
                    "</opayment>\n" +
                    "</request>";

            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сформировать запрос в pay-SL:" + ex.Message);
            }
            return resp;
        }
        public string CreateStatusPaymentQuery(StatusReqBank req)
        {
            string resp = "";
            try
            {
                string guid = req.platformReferenceNumber.Substring(req.platformReferenceNumber.Count() / 2, (req.platformReferenceNumber.Count() / 2));
                resp = "<request point=\""+_appSettings.Value.pointSL+ "\">\n<status id=\"" + guid + "\"/>\n</request>";
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сформировать запрос в status-SL:" + ex.Message);
            }
            return resp;
        }


        public CheckRespBank CreateCheckPaymentResponse(string respSL, CheckReqBank req)
        {
            try
            {
                CheckRespBank resp = new CheckRespBank();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(respSL);

                if (xDoc == null || xDoc.InnerXml == "")
                {
                    throw new Exception("Не удалось сформировать ответ: тело ответа от платежной системы пуст");
                }

                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 0;
                resp.transferState.errorMessage = "";
                resp.transferState.state = "";
                resp.platformReferenceNumber = "";
                resp.checkDate = DateTime.Now;
                resp.receiver = new Models.CommonModels.Receiver();
                resp.receiver.currencies = new List<string>();
                resp.receiver.displayName = "";
                resp.receiver.identification = new Models.CommonModels.Identification();
                resp.receiver.identification.type = "";
                resp.receiver.identification.value = "";
                resp.receiver.participant = new Models.CommonModels.Participant();
                resp.receiver.participant.participantId = "";
                resp.receivingAmount = new Models.CommonModels.Ammount();
                resp.receivingAmount.amount = "";
                resp.receivingAmount.currency = "";

                CheckResp re = SLCheckResponce(respSL);
                                        
                    if (re.identifierStatus == "OK" && re.blackListFl == "0" && re.allowedCreditFl == "1")
                    {
                        resp.transferState.errorCode = 100;
                        resp.transferState.errorMessage = "Запрос проверки перевода выполнен без ошибок";
                        resp.transferState.state = "CHECKED";
                        resp.platformReferenceNumber = req.platformReferenceNumber;
                        resp.checkDate = DateTime.Now;
                        resp.receiver.currencies.Add(re.currency);
                        resp.receiver.displayName = re.maskedName;
                        resp.receiver.identification.type = "PHONE";
                        resp.receiver.identification.value = req.receiver.identification.value;
                        resp.receiver.participant.participantId = req.receiver.participant?.participantId;
                        resp.receivingAmount.amount = req.receivingAmount.amount;
                        resp.receivingAmount.currency = req.receivingAmount.currency;
                    }
                    else
                    {
                        resp.transferState.errorCode = 100;
                        resp.transferState.errorMessage = "Запрос проверки перевода выполнен без ошибок";
                        resp.transferState.state = "INVALID";
                        resp.platformReferenceNumber = req.platformReferenceNumber;
                        resp.checkDate = DateTime.Now;
                        //resp.receiver.currencies.Add(currency);
                        //resp.receiver.displayName = maskedName;
                        //resp.receiver.identification.type = "PHONE";
                        //resp.receiver.identification.value = req.receiver.identification.value;
                        //resp.receiver.particiant.participant = req.receiver.particiant.participant;
                        resp.receivingAmount.amount = req.receivingAmount.amount;
                        resp.receivingAmount.currency = req.receivingAmount.currency;
                    }
                
                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сформировать ответ для XFer-Check:" + ex.Message);
            }
            
        }

        public ConfirmRespBank CreateConfirmPaymentResponse(string respSL)
        {
            try
            {
                ConfirmRespBank resp = new ConfirmRespBank();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(respSL);

                if (xDoc == null || xDoc.InnerXml == "")
                {
                    throw new Exception("Не удалось сформировать ответ: тело ответа от платежной системы пуст");
                }

                XmlElement xRoot = xDoc.DocumentElement;
                XmlNode xnode = xRoot.FirstChild;
                XmlAttributeCollection resultAttributes = xnode.Attributes;
                string state = resultAttributes.GetNamedItem("state").InnerText;
                string substate = resultAttributes.GetNamedItem("substate").InnerText;
               
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 200;
                resp.transferState.errorMessage = "Запрос подтверждения перевода выполнен без ошибок";
                resp.transferState.state = "0";
                if (state == "60"||  state == "20" || state == "40")
                {
                    resp.transferState.errorCode = 200;
                    //resp.transferState.errorMessage = "Запрос подтверждения перевода выполнен без ошибок";
                    resp.transferState.state = "CONFIRM_PENDING";
                }
                else
                {
                    if(substate == "1" || substate == "2" || substate == "3")
                    resp.transferState.errorCode = 207;
                    else
                        resp.transferState.errorCode = 205;
                    //resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                    resp.transferState.state = "INVALID";
                }
                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сформировать ответ для XFer-Confirm:" + ex.Message);
            }
        }

        public StatusRespBank CreateStatusPaymentResponse(string respSL)
        {
            try
            {
                StatusRespBank resp = new StatusRespBank();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(respSL);

                if (xDoc == null || xDoc.InnerXml == "")
                {
                    throw new Exception("Не удалось сформировать ответ: тело ответа от платежной системы пуст");
                }

                XmlElement xRoot = xDoc.DocumentElement;
                XmlNode xnode = xRoot.FirstChild;
                XmlAttributeCollection resultAttributes = xnode.Attributes;
                string state = resultAttributes.GetNamedItem("state").InnerText;
                string substate = resultAttributes.GetNamedItem("substate").InnerText;
                string code = resultAttributes.GetNamedItem("code").InnerText;
                string final = resultAttributes.GetNamedItem("final").InnerText;
                //string trans = resultAttributes.GetNamedItem("trans").InnerText;

                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 200;
                resp.transferState.errorMessage = "Перевод в процессе подтверждения, ожидается ответ банка Получателя";
                resp.transferState.state = "0";


                if (state == "60")
                {
                    resp.transferState.state = "CONFIRMED";
                    resp.transferState.errorMessage = "Перевод подтвержден и успешно завершен";
                }
              
                else
                {
                    if (final == "1")
                    {
                        if (substate == "1" || substate == "2" || substate == "3")
                            resp.transferState.errorCode = 207;
                        else
                            resp.transferState.errorCode = 205;
                        resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                        resp.transferState.state = "INVALID";
                       
                        //resp.transferState.errorMessage = "Перевод не проведен по причине ошибки";
                    }
                    else
                    {
                        resp.transferState.state = "CONFIRM_PENDING";
                        //resp.transferState.errorMessage = "Перевод в процессе подтверждения, ожидается ответ банка Получателя";
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сформировать ответ для XFer-Status::" + ex.Message);
            }
        }


        public CheckResp SLCheckResponce(string respSL)
        {
            try
            {
                CheckResp resp = new CheckResp();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(respSL);

                if (xDoc == null || xDoc.InnerXml == "")
                {
                    return resp;
                }

                XmlElement xRoot = xDoc.DocumentElement;
                XmlNode xnode = xRoot.FirstChild;
                XmlAttributeCollection resultAttributes = xnode.Attributes;
                string service = resultAttributes.GetNamedItem("service").InnerText;


                if (service == "0")
                {
                    XmlNodeList xnodeList = xRoot.FirstChild.FirstChild.ChildNodes;

                    for (int i = 0; i < xnodeList.Count; i++)
                    {
                        resultAttributes = xnodeList[i].Attributes;
                        switch (resultAttributes.GetNamedItem("key").InnerText)
                        {
                            case "processing":
                                resp.processing = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "maskedName":
                                resp.maskedName = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "allowedCreditFl":
                                resp.allowedCreditFl = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "cardFl":
                                resp.cardFl = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "identifierStatus":
                                resp.identifierStatus = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "department":
                                resp.department = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "blackListFl":
                                resp.blackListFl = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "number":
                                resp.number = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "name":
                                resp.name = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                            case "currency":
                                resp.currency = resultAttributes.GetNamedItem("value").InnerText;
                                break;
                        }
                    }
                    
                }
                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось обработать ответ HBK-Check:" + ex.Message);
            }
        }
    }
}