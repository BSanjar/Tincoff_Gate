using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public XferController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        [HttpPost]
        [Route("check")]
        public CheckRespXfer Check()
        {
            CheckRespXfer resp = new CheckRespXfer();
            try
            {
                string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                CheckReqXfer req = JsonConvert.DeserializeObject<CheckReqXfer>(Convert.ToString(reqBody));
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
                resp = integration.CheckXfer(req, _appSettings.Value.hostXref);
               
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось выполнить проверку, ошибка: " + ex.Message;
            }
            return resp;
        }

        [HttpPost]
        [Route("pay")]
        public ConfirmRespXfer Pay()
        {
            ConfirmRespXfer resp = new ConfirmRespXfer();
            try
            {
                string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                ConfirmReqXfer req = JsonConvert.DeserializeObject<ConfirmReqXfer>(Convert.ToString(reqBody));
                string ConcatStr =
                    req.platformReferenceNumber +
                    req.originator.identification.value +
                    req.receiver.identification.value +
                    req.paymentAmount.amount +
                    req.paymentAmount.currency +
                    req.settlementAmount.amount +
                    req.settlementAmount.currency +
                    req.receivingAmount.amount+
                    req.receivingAmount.currency;
                Signature signature = new Signature();
                string sign = signature.SignData(ConcatStr);
                req.originatorSignature = sign;
                Integration.Integration integration = new Integration.Integration(_appSettings);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.ConfirmXfer(req, _appSettings.Value.hostXref);
               
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось провести платеж, ошибка: " + ex.Message;
            }
            return resp;
        }

        [HttpPost]
        [Route("state")]
        public StatusRespXfer State()
        {
            StatusRespXfer resp = new StatusRespXfer();
            try
            {
                string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                StatusReqXfer req = JsonConvert.DeserializeObject<StatusReqXfer>(Convert.ToString(reqBody));
                //string ConcatStr =
                //    req.platformReferenceNumber +
                //    req.originator.identification.value +
                //    req.receiver.identification.value +
                //    req.paymentAmount.amount +
                //    req.paymentAmount.currency +
                //    req.settlementAmount.amount +
                //    req.settlementAmount.currency +
                //    req.receivingAmount.amount +
                //    req.receivingAmount.currency;
                //Signature signature = new Signature();
                //string sign = signature.GetSign(ConcatStr);
                //req.originatorSignature = sign;
                Integration.Integration integration = new Integration.Integration(_appSettings);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.StatusXfer(req, _appSettings.Value.hostXref);
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось провести платеж, ошибка: " + ex.Message;
            }
            return resp;
        }

    }
}
