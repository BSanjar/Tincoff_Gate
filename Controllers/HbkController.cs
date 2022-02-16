using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tincoff_Gate.Models;
using Tincoff_Gate.Models.ToBank;
using Tincoff_Gate.Models.ToXfer;

namespace Tincoff_Gate.Controllers
{
    //отправка из ХБК
    [ApiController]
    [Route("Hbk")]
    public class HbkController : ControllerBase
    {
        private AppSettings _config;

        //private readonly ILogger<HomeController> _logger;

        private readonly IOptions<AppSettings> _appSettings;

        public HbkController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        [HttpPost]
        [Route("check")]
        public CheckRespBank Check()
        {
            
            CheckRespBank resp = new CheckRespBank();
            try
            {
                string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                CheckReqBank req = JsonConvert.DeserializeObject<CheckReqBank>(Convert.ToString(reqBody));
                string ConcatStr =
                    req.platformReferenceNumber +
                    req.receiver.identification.value +
                    req.paymentAmount.amount +
                    req.paymentAmount.currency +
                    req.settlementAmount.amount +
                    req.settlementAmount.currency +
                    req.receivingAmount.amount +
                    req.receivingAmount.currency;
                Signature signature = new Signature();

                bool signVerificed = signature.VerifySignature(reqBody, req.platformSignature);

                //test
                signVerificed = true;
                if (!signVerificed)
                {
                    resp.transferState = new Models.CommonModels.TransferState();
                    resp.transferState.errorCode = 206;
                    resp.transferState.errorMessage = "Ошибка проверки подписи платформы";
                    resp.transferState.state = "INVALID";

                    return resp;
                }
                Integration.Integration integr = new Integration.Integration(_appSettings);
                resp = integr.CheckBank(req);
               
            }
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 103;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при проверке";
                resp.transferState.state = "INVALID";
            }
           
            return resp;
        }


        [HttpPost]
        [Route("confirm")]
        public ConfirmRespBank Confirm()
        {

            ConfirmRespBank resp = new ConfirmRespBank();
            try
            {
                string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                ConfirmReqBank req = JsonConvert.DeserializeObject<ConfirmReqBank>(Convert.ToString(reqBody));
                string ConcatStr =
                    req.platformReferenceNumber +
                    req.originator.identification.value +
                    req.receiver.identification.value +
                    req.paymentAmount.amount +
                    req.paymentAmount.currency +
                    req.settlementAmount.amount +
                    req.settlementAmount.currency +
                    req.receivingAmount.amount +
                    req.receivingAmount.currency;
                Signature signature = new Signature();

                bool signVerificed = signature.VerifySignature(reqBody, req.platformSignature);
                //test
                signVerificed = true;
                if (!signVerificed)
                {
                    resp.transferState = new Models.CommonModels.TransferState();
                    resp.transferState.errorCode = 206;
                    resp.transferState.errorMessage = "Ошибка проверки подписи платформы";
                    resp.transferState.state = "INVALID";

                    return resp;
                }
                Integration.Integration integr = new Integration.Integration(_appSettings);
                resp = integr.ConfirmBank(req);

            }
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 205;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                resp.transferState.state = "INVALID";
            }

            return resp;
        }

        [HttpPost]
        [Route("status")]
        public StatusRespBank Status()
        {

            StatusRespBank resp = new StatusRespBank();
            try
            {
                string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                StatusReqBank req = JsonConvert.DeserializeObject<StatusReqBank>(Convert.ToString(reqBody));
                string ConcatStr =
                    req.platformReferenceNumber;
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
                Integration.Integration integr = new Integration.Integration(_appSettings);
                resp = integr.StatusBank(req);

            }
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 205;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при зачислении";
                resp.transferState.state = "INVALID";
            }

            return resp;
        }

    }
}
