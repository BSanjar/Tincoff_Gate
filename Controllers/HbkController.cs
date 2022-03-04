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
            
            try
            {
                CheckReqBank req = JsonConvert.DeserializeObject<CheckReqBank>(Convert.ToString(reqBody));
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
            
            logger.InsertLog(new Log { id = id, Descript = descript, EventName = "Xfer->HbKGate/Check", Status = status,request = reqBody, response = JsonConvert.SerializeObject(resp) });

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
            try
            {
                ConfirmReqBank req = JsonConvert.DeserializeObject<ConfirmReqBank>(Convert.ToString(reqBody));
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
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 205;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                resp.transferState.state = "INVALID";
                status = "error";
                descript = ex.Message;
            }

            
            logger.InsertLog(new Log { id = id, Descript = descript, EventName = "Xfer->HabkGate/Confirm", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

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
            logger.InsertLog(new Log { id = id, Descript = descript, EventName = "Xfer->HabkGate/State", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

            return resp;
        }
    }
}
