using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tincoff_Gate.Integration;
using Tincoff_Gate.Models;
using Tincoff_Gate.Models.ToBank;
using Tincoff_Gate.Models.ToXfer;

namespace Tincoff_Gate.Controllers
{
    //отправка из ХБК
    [ApiController]
    [Route("transfer")]
    public class HbkController : ControllerBase
    {
        private AppSettings _config;

        //private readonly ILogger<HomeController> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IOptions<ConnectionStrings> _connectionString;

        public HbkController(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionString)
        {
            _appSettings = appSettings;
            _connectionString = connectionString;
        }

        [HttpPost]
        [Route("check")]
        public CheckRespBank Check()
        {
            Logger logger = new Logger(_connectionString, _appSettings);
            CheckRespBank resp = new CheckRespBank();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            string id = "";
            CheckReqBank req = new CheckReqBank();
            try
            {
                req = JsonConvert.DeserializeObject<CheckReqBank>(Convert.ToString(reqBody));

                Log logDb = logger.GetLog(req.platformReferenceNumber);
                if (logDb != null && logDb.idPlatform != null && logDb.idPlatform != "")
                {
                    return JsonConvert.DeserializeObject<CheckRespBank>(logDb.checkResponse);
                }

                id = req.platformReferenceNumber;
                string ammount = req.paymentAmount.amount;
                string SettlAmmount = req.settlementAmount.amount?.ToString().Replace(",", ".");
                string recAmmount = req.receivingAmount.amount?.ToString().Replace(",", ".");
                //if (!(req.paymentAmount.amount.ToString().Contains(',') || req.paymentAmount.amount.ToString().Contains('.')))
                //{
                //    ammount = ammount + ".0";
                //}

                //if (!(req.settlementAmount.amount.ToString().Contains(',') || req.settlementAmount.amount.ToString().Contains('.')))
                //{
                //    SettlAmmount = SettlAmmount + ".0";
                //}

                //if (!(recAmmount.Contains(',') || recAmmount.Contains('.')))
                //{
                //    recAmmount = recAmmount + ".0";
                //}
                string ConcatStr =
                    req.platformReferenceNumber +
                    req.receiver.identification.value +
                    ammount +
                    req.paymentAmount.currency +
                    SettlAmmount +
                    req.settlementAmount.currency +
                    recAmmount +
                    req.receivingAmount.currency;
                Signature signature = new Signature();

                bool signVerificed = signature.VerifySignature(ConcatStr, req.platformSignature);

                //test
                //signVerificed = true;
                if (!signVerificed)
                {
                    resp.transferState = new Models.CommonModels.TransferState();
                    resp.transferState.errorCode = 206;
                    resp.transferState.errorMessage = "Ошибка проверки подписи платформы";
                    resp.transferState.state = "INVALID";

                    descript = "Ошибка проверки подписи платформы";
                    //return resp;
                }
                else
                {
                    Integration.Integration integr = new Integration.Integration(_appSettings, _connectionString);
                    resp = integr.CheckBank(req);
                }
               
            }
            catch (Exception ex)
            {
                id = "";
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 103;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при проверке";
                resp.transferState.state = "INVALID";
                resp.checkDate = DateTime.Now;
                descript = ex.Message;
                status = "error";
            }

            logger.checkLog(new Log
            {
                id = Guid.NewGuid().ToString(),
                idPlatform = resp.platformReferenceNumber,
                comment = resp.transferState.errorMessage,
                state = resp.transferState.state,
                checkDate = DateTime.Now,
                checkRequest = reqBody,
                checkResponse = JsonConvert.SerializeObject(resp),
                idCheck = resp.platformReferenceNumber,               
                originatorid = "",
                originatorBank = _appSettings.Value.participantId,
                originatorSumm = Convert.ToDouble(req.paymentAmount.amount.Replace('.', ','))
            });
            return resp;
        }


        [HttpPost]
        [Route("confirm")]
        public ConfirmRespBank Confirm()
        {

            ConfirmRespBank resp = new ConfirmRespBank();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            string id = "";
            Logger logger = new Logger(_connectionString, _appSettings);
            ConfirmReqBank req = new ConfirmReqBank();

            AmlResponseMethod amlResult = new AmlResponseMethod();
            AmlResponseMethod amlResult2 = new AmlResponseMethod();
            amlResult.log = new Log();
                amlResult2.log = new Log();
            try
            {
                req = JsonConvert.DeserializeObject<ConfirmReqBank>(Convert.ToString(reqBody));
                Log logDb = logger.GetLog(req.platformReferenceNumber);
                if (logDb != null && logDb.idPlatform!=null && logDb.idPlatform!="")
                {
                    return JsonConvert.DeserializeObject<ConfirmRespBank>(logDb.payResponse);
                }
                Integration.Integration integration = new Integration.Integration(_appSettings, _connectionString);

                amlResult = integration.CheckAml(req.originator.fullName, req.platformReferenceNumber);
                if (amlResult.code == "0" || amlResult.code == "3")
                {
                    amlResult2 = integration.CheckAml2(req.receiver.displayName, req.platformReferenceNumber);
                    if (amlResult2.code == "0" || amlResult2.code == "3")
                    {


                        id = req.platformReferenceNumber;
                        string ammount = req.paymentAmount.amount;
                        string SettlAmmount = req.settlementAmount.amount;
                        string recAmmount = req.receivingAmount.amount;
                        //if (!(req.paymentAmount.amount.ToString().Contains(',') || req.paymentAmount.amount.ToString().Contains('.')))
                        //{
                        //    ammount = ammount + ".0";
                        //}

                        //if (!(req.settlementAmount.amount.ToString().Contains(',') || req.settlementAmount.amount.ToString().Contains('.')))
                        //{
                        //    SettlAmmount = SettlAmmount + ".0";
                        //}

                        //if (!(recAmmount.Contains(',') || recAmmount.Contains('.')))
                        //{
                        //    recAmmount = recAmmount + ".0";
                        //}


                        string ConcatStr =
                            req.platformReferenceNumber +
                            req.originator.identification.value +
                            req.receiver.identification.value +
                            ammount +
                            req.paymentAmount.currency +
                            SettlAmmount +
                            req.settlementAmount.currency +
                            recAmmount +
                            req.receivingAmount.currency;
                        Signature signature = new Signature();

                        bool signVerificed = signature.VerifySignature(ConcatStr, req.platformSignature);
                        //test
                        //signVerificed = true;
                        if (!signVerificed)
                        {
                            resp.transferState = new Models.CommonModels.TransferState();
                            resp.transferState.errorCode = 206;
                            resp.transferState.errorMessage = "Ошибка проверки подписи платформы";
                            resp.transferState.state = "INVALID";
                            descript = "Ошибка проверки подписи платформы";
                            //status = "error";
                            //return resp;
                        }
                        else
                        {
                            Integration.Integration integr = new Integration.Integration(_appSettings, _connectionString);
                            resp = integr.ConfirmBank(req);
                            resp.platformReferenceNumber = req.platformReferenceNumber;

                        }
                    }
                    else
                    {
                        resp.transferState = new Models.CommonModels.TransferState();
                        resp.transferState.errorCode = 205;
                        resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                        resp.transferState.state = "INVALID";
                    }
                }
                else
                {
                    resp.transferState = new Models.CommonModels.TransferState();
                    resp.transferState.errorCode = 205;
                    resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                    resp.transferState.state = "INVALID";
                }
            }
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 205;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                resp.transferState.state = "INVALID";
                status = "error";
                descript = ex.Message;
            }

            logger.payLog(new Log { idPlatform = req.platformReferenceNumber, state = resp.transferState.state, comment = resp.transferState.errorMessage, 
                payRequest = reqBody, payResponse = JsonConvert.SerializeObject(resp), payDate = DateTime.Now,
                amlCheckDate = amlResult.log.amlCheckDate,
                amlRequest = amlResult.log.amlRequest,
                amlResponse = amlResult.log.amlResponse,

                amlCheckDateRec = amlResult2.log.amlCheckDate,
                amlRequestRec = amlResult2.log.amlRequest,
                amlResponseRec = amlResult2.log.amlResponse



            });

           
            return resp;
        }

        [HttpPost]
        [Route("state")]
        public StatusRespBank Status()
        {

            StatusRespBank resp = new StatusRespBank();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            string id = "";
            Logger logger = new Logger(_connectionString, _appSettings);
            try
            {

                StatusReqBank req = JsonConvert.DeserializeObject<StatusReqBank>(Convert.ToString(reqBody));
                
                string ConcatStr =
                    req.platformReferenceNumber;
                id = req.platformReferenceNumber;
                //Signature signature = new Signature();

                //bool signVerificed = signature.VerifySignature(reqBody, req.platformSignature);

                //if (!signVerificed)
                //{
                //    resp.transferState = new Models.CommonModels.TransferState();
                //    resp.transferState.errorCode = 206;
                //    resp.transferState.errorMessage = "Ошибка проверки подписи платформы";
                //    resp.transferState.state = "INVALID";

                //    return resp;
                //}
                Integration.Integration integr = new Integration.Integration(_appSettings, _connectionString);
                resp = integr.StatusBank(req);
                
                resp.receivedDate =  DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
                resp.platformReferenceNumber = req.platformReferenceNumber;
            }
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 205;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                resp.transferState.state = "INVALID";
                resp.receivedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
                //resp.platformReferenceNumber = req.platformReferenceNumber;
                descript = ex.Message;
            }
            logger.StateLog(new Log
            {
                idPlatform = resp.platformReferenceNumber,

                stateDate = DateTime.Now,
                stateRequest = reqBody,
                stateResponse = JsonConvert.SerializeObject(resp)
            });

            return resp;
        }
    }
}
