﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("Hbk")]
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
            
            CheckRespBank resp = new CheckRespBank();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            try
            {
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

                    descript = "Ошибка проверки подписи платформы";
                    //return resp;
                }
                else
                {
                    Integration.Integration integr = new Integration.Integration(_appSettings);
                    resp = integr.CheckBank(req);
                }
               
            }
            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = 103;
                resp.transferState.errorMessage = "Ошибка внутренней системы получателя перевода при проверке";
                resp.transferState.state = "INVALID";
                descript = ex.Message;
                status = "error";
            }
            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = resp.platformReferenceNumber, Descript = descript, EventName = "Hbk/confirm", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

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
            try
            {

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
                    descript = "Ошибка проверки подписи платформы";
                    //status = "error";
                    //return resp;
                }
                else
                {
                    Integration.Integration integr = new Integration.Integration(_appSettings);
                    resp = integr.ConfirmBank(req);
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

            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = resp.platformReferenceNumber, Descript = descript, EventName = "Hbk/confirm", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });


            return resp;
        }

        [HttpPost]
        [Route("status")]
        public StatusRespBank Status()
        {

            StatusRespBank resp = new StatusRespBank();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            try
            {

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
                descript = ex.Message;
            }
            Logger logger = new Logger(_connectionString, _appSettings);
            logger.InsertLog(new Log { id = resp.platformReferenceNumber, Descript = descript, EventName = "Hbk/confirm", Status = status, request = reqBody, response = JsonConvert.SerializeObject(resp) });

            return resp;
        }

    }
}
