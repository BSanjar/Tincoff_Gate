using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Integration;
using Tincoff_Gate.Models;
using Tincoff_Gate.Models.ToXfer;

namespace Tincoff_Gate.Controllers
{
    [Route("Xfer")]
    [ApiController]
    public class XferController : ControllerBase
    {
        private AppSettings _config;

        //private readonly ILogger<HomeController> _logger;

        private readonly IOptions<AppSettings> _appSettings;
        private readonly IOptions<ConnectionStrings> _connectionString;

        public XferController(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionString)
        {
            _appSettings = appSettings;
            _connectionString = connectionString;
        }
        [HttpPost]
        [Route("check")]
        public CheckRespXfer Check()
        {
            CheckRespXfer resp = new CheckRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            try
            {
                CheckReqXfer req = JsonConvert.DeserializeObject<CheckReqXfer>(Convert.ToString(reqBody));
                
                //string ammount = req.paymentAmount.amount.ToString();
                //if (!(req.paymentAmount.amount.ToString().Contains(',')|| req.paymentAmount.amount.ToString().Contains('.')))
                //{
                //    ammount = ammount + ".0";
                //}
                

                string ConcatStr =
                    req.originatorReferenceNumber +
                    req.originator.identification.value +
                    req.receiver.identification.value +
                    req.paymentAmount.amount +
                    req.paymentAmount.currency +
                    req.receivingAmount.currency;
                Signature signature = new Signature();
                string sign = signature.SignData(ConcatStr);
                req.originatorSignature = sign;
                Integration.Integration integration = new Integration.Integration(_appSettings);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.CheckXfer(req);
                resp.platformReferenceNumber = req.originatorReferenceNumber;
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось выполнить проверку, ошибка: " + ex.Message;
                status = "error";
            }

            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = resp.platformReferenceNumber, Descript=resp.transferState.errorMessage, EventName= "Xfer/check",Status= status, request = reqBody, response = JsonConvert.SerializeObject(resp)});

            return resp;
        }

        [HttpPost]
        [Route("confirm")]
        public ConfirmRespXfer Confirm()
        {
            ConfirmRespXfer resp = new ConfirmRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            try
            {
               
                ConfirmReqXfer req = JsonConvert.DeserializeObject<ConfirmReqXfer>(Convert.ToString(reqBody));

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
                string sign = signature.SignData(ConcatStr);
                req.originatorSignature = sign;
                Integration.Integration integration = new Integration.Integration(_appSettings);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.ConfirmXfer(req, _appSettings.Value.hostXref);
                resp.platformReferenceNumber = req.platformReferenceNumber;
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось провести платеж, ошибка: " + ex.Message;
                status = "error";
            }
            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = resp.platformReferenceNumber, Descript = resp.transferState.errorMessage, EventName = "Xfer/confirm", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

            return resp;
        }

        [HttpPost]
        [Route("state")]
        public StatusRespXfer State()
        {
            StatusRespXfer resp = new StatusRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            try
            {
                StatusReqXfer req = JsonConvert.DeserializeObject<StatusReqXfer>(Convert.ToString(reqBody));
                resp.platformReferenceNumber = req.platformReferenceNumber;
                Integration.Integration integration = new Integration.Integration(_appSettings);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.StatusXfer(req, _appSettings.Value.hostXref);
                resp.platformReferenceNumber = req.platformReferenceNumber;
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось провести платеж, ошибка: " + ex.Message;
                status = "error";
            }
            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = resp.platformReferenceNumber, Descript = resp.transferState.errorMessage, EventName = "Xfer/state", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

            return resp;
        }

        [HttpPost]
        [Route("rate")]
        public RateRespXfer Rate()
        {
            RateRespXfer resp = new RateRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "Успех";
            try
            {
                RateReqXfer req = JsonConvert.DeserializeObject<RateReqXfer>(Convert.ToString(reqBody));

                Integration.Integration integration = new Integration.Integration(_appSettings);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.RateXfer(req, _appSettings.Value.hostXref);
                resp.errCode = "0";
                resp.errMessage = "успех";
            }

            catch (Exception ex)
            {
                status = "error";
                descript = ex.Message;
                resp.errCode = "-1";
                resp.errMessage = ex.Message;
            }
            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = "", Descript = descript, EventName = "Xfer/rate", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

            return resp;
        }
    }
}
