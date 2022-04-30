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
            Logger logger = new Logger(_connectionString, _appSettings);
            CheckRespXfer resp = new CheckRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "CHECKED";
            string descript = "успех";
            string colvirId = "";
            string id = "";
            string clientColvirId = "";
            CheckReqXfer req = new CheckReqXfer();

            AmlResponseMethod amlResult = new AmlResponseMethod();
            AmlResponseMethod amlResult2 = new AmlResponseMethod();
            amlResult.log = new Log();
            amlResult2.log = new Log();
            try
            {
                req = JsonConvert.DeserializeObject<CheckReqXfer>(Convert.ToString(reqBody));

                Integration.Integration integration = new Integration.Integration(_appSettings, _connectionString);
                colvirId = req.originatorSignature; //в этой поле возвращается код клиента в колвире, дальше в этой поле передаем подпись

                Limit limit = logger.GetLimit(colvirId);
                if (Convert.ToDouble(_appSettings.Value.sumTrnDay)==0|| Convert.ToDouble(_appSettings.Value.sumTrnDay) >= Convert.ToDouble(limit.sumTrnDay) + Convert.ToDouble(req.paymentAmount.amount.Replace('.', ',')))
                {
                    if (Convert.ToDouble(_appSettings.Value.sumTrnMonth) == 0 || Convert.ToDouble(_appSettings.Value.sumTrnMonth) >= Convert.ToDouble(limit.sumTrnMonth) + Convert.ToDouble(req.paymentAmount.amount.Replace('.', ',')))
                    {
                        if (Convert.ToDouble(_appSettings.Value.countTrnDay) == 0 || Convert.ToDouble(_appSettings.Value.countTrnDay) >= Convert.ToDouble(limit.countTrnDay) + 1)
                        {
                            amlResult = integration.CheckAml(req.originator.fullName, req.originatorReferenceNumber);
                            if (amlResult.code == "0" || amlResult.code == "3")
                            {

                                id = req.originatorReferenceNumber;
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

                                req.originator.nationality = GetNationISO(req.originator.nationality);



                                //CheckReqXfer req = new CheckReqXfer();
                                resp = integration.CheckXfer(req);

                                //замена точки на запятой
                                if (resp != null)
                                {
                                    amlResult2 = integration.CheckAml2(resp.receiver.displayName, req.originatorReferenceNumber);
                                    if (amlResult.code == "2" || amlResult.code == "-1")
                                    {
                                        resp.transferState = new Models.CommonModels.TransferState();
                                        resp.transferState.errorCode = -1;
                                        resp.transferState.errorMessage = "Получатель не прошел проверку по АМЛ";
                                        status = "CHECK_PENDING";
                                        descript = "Получатель не прошел проверку по АМЛ";
                                    }


                                    //if(resp.paymentAmount!=null && resp.paymentAmount.amount != null)
                                    //    resp.paymentAmount.amount = resp.paymentAmount.amount.Replace('.', ',');
                                    //if (resp.receivingAmount != null && resp.receivingAmount.amount != null)
                                    //    resp.receivingAmount.amount = resp.receivingAmount.amount.Replace('.', ',');
                                    //if (resp.conversionRateBuy != null && resp.conversionRateBuy.baseRate != null)
                                    //{
                                    //    resp.conversionRateBuy.baseRate = resp.conversionRateBuy.baseRate.Replace('.', ',');
                                    //}
                                    //if (resp.conversionRateBuy != null && resp.conversionRateBuy.rate != null)
                                    //{
                                    //    resp.conversionRateBuy.rate = resp.conversionRateBuy.rate.Replace('.', ',');
                                    //}
                                    //if (resp.feeAmount != null && resp.feeAmount.amount != null )
                                    //    resp.feeAmount.amount = resp.feeAmount.amount.Replace('.', ',');
                                    //if (resp.displayFeeAmount != null && resp.displayFeeAmount.amount != null )
                                    //    resp.displayFeeAmount.amount = resp.displayFeeAmount.amount.Replace('.', ',');


                                    resp.originatorReferenceNumber = req.originatorReferenceNumber;
                                }
                            }
                            else
                            {
                                resp.transferState = new Models.CommonModels.TransferState();
                                resp.transferState.errorCode = -1;
                                resp.transferState.errorMessage = "Не прошел проверку по АМЛ";
                                status = "CHECK_PENDING";
                                descript = "Не прошел проверку по АМЛ";
                            }


                        }
                        else
                        {
                            resp.transferState = new Models.CommonModels.TransferState();
                            resp.transferState.errorCode = -21;
                            resp.transferState.errorMessage = "Исчерпан лимит по количеству транзакций за день";
                            status = "CHECK_PENDING";
                            descript = "Исчерпан лимит по количеству транзакций за день";
                        }
                    }
                    else
                    {
                        resp.transferState = new Models.CommonModels.TransferState();
                        resp.transferState.errorCode = -22;
                        resp.transferState.errorMessage = "Исчерпан лимит по сумме транзакций за месяц";
                        status = "CHECK_PENDING";
                        descript = "Исчерпан лимит по сумме транзакций за месяц";
                    }
                }
                else
                {
                    resp.transferState = new Models.CommonModels.TransferState();
                    resp.transferState.errorCode = -23;
                    resp.transferState.errorMessage = "Исчерпан лимит по сумме транзакций за день";
                    status = "CHECK_PENDING";
                    descript = "Исчерпан лимит по сумме транзакций за день";
                }

            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось выполнить проверку, ошибка: " + ex.Message;
                status = "CHECK_PENDING";
                descript = ex.Message;
            }
            logger.checkLog(new Log { id = Guid.NewGuid().ToString(), 
                                      comment = descript, 
                                      state = resp.transferState.state, 
                                      checkDate = DateTime.Now,
                checkRequest = reqBody, 
                                      checkResponse = JsonConvert.SerializeObject(resp),
                                      idCheck = req.originatorReferenceNumber,
                                      idPlatform = resp.platformReferenceNumber,
                                      originatorid = colvirId,
                                      originatorBank = _appSettings.Value.participantId,
                                      originatorSumm = Convert.ToDouble(req.paymentAmount.amount.Replace('.',',')),

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
        [Route("confirm")]
        public ConfirmRespXfer Confirm()
        {
            ConfirmRespXfer resp = new ConfirmRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            Logger logger = new Logger(_connectionString, _appSettings);
            ConfirmReqXfer req = new ConfirmReqXfer();
            try
            {
               
                req = JsonConvert.DeserializeObject<ConfirmReqXfer>(Convert.ToString(reqBody));
               

                req.checkDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
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

                req.originator.nationality = GetNationISO(req.originator.nationality);

                Integration.Integration integration = new Integration.Integration(_appSettings, _connectionString);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.ConfirmXfer(req, _appSettings.Value.hostXref);
                //resp.platformReferenceNumber = req.platformReferenceNumber;
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось провести платеж, ошибка: " + ex.Message;
            }
            logger.payLog(new Log { idPlatform = req.platformReferenceNumber, state = resp.transferState.state, comment = resp.transferState.errorMessage, payRequest = reqBody, payResponse = JsonConvert.SerializeObject(resp), payDate = DateTime.Now });

            return resp;
        }




        [HttpPost]
        [Route("state")]
        public StatusRespXfer State()
        {
            StatusRespXfer resp = new StatusRespXfer();
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            string id = "";
            Logger logger = new Logger(_connectionString, _appSettings);
            StatusReqXfer req = new StatusReqXfer();
            try
            {
                req = JsonConvert.DeserializeObject<StatusReqXfer>(Convert.ToString(reqBody));
                id = req.originatorReferenceNumber;
                resp.platformReferenceNumber = req.originatorReferenceNumber;
                Integration.Integration integration = new Integration.Integration(_appSettings, _connectionString);

                //CheckReqXfer req = new CheckReqXfer();
                resp = integration.StatusXfer(req, _appSettings.Value.hostXref);
                resp.platformReferenceNumber = req.originatorReferenceNumber;
            }

            catch (Exception ex)
            {
                resp.transferState = new Models.CommonModels.TransferState();
                resp.transferState.errorCode = -1;
                resp.transferState.errorMessage = "Не удалось провести платеж, ошибка: " + ex.Message;
                status = "error";
                descript = ex.Message;
            }
            logger.StateLog(new Log { idPlatform = req.originatorReferenceNumber, 
                
                stateDate = DateTime.Now,
                stateRequest = reqBody,
                stateResponse = JsonConvert.SerializeObject(resp) });

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
            Logger logger = new Logger(_connectionString, _appSettings);
            try
            {
                RateReqXfer req = JsonConvert.DeserializeObject<RateReqXfer>(Convert.ToString(reqBody));
                

                req.effectiveDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")+"Z";
                Integration.Integration integration = new Integration.Integration(_appSettings, _connectionString);

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

            return resp;
        }

        public string GetNationISO(string nationality)
        {
            string natIso = "KGZ";
            try
            {
                string nat = nationality.Substring(0, 3).ToLower();
                
                switch (nat)
                {
                    case "рус":
                        natIso = "RUS";
                        break;
                    case "кыр":
                        natIso = "KGZ";
                        break;
                    case "каз":
                        natIso = "KAZ";
                        break;
                    case "укр":
                        natIso = "UKR";
                        break;
                    case "анг":
                        natIso = "AIA";
                        break;
                    default:
                        natIso = "KGZ";
                        break;

                }
                return natIso;
            }
            catch
            {
                return natIso;
            }
            
        }

      

    }
}
