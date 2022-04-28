using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Tincoff_Gate.Integration;
using Tincoff_Gate.Models;

namespace Tincoff_Gate.Controllers
{
    [Route("Esb")]
    [ApiController]
    public class EsbController : ControllerBase
    {
        private AppSettings _config;

        //private readonly ILogger<HomeController> _logger;

        private readonly IOptions<AppSettings> _appSettings;
        private readonly IOptions<ConnectionStrings> _connectionString;

        public EsbController(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionString)
        {
            _appSettings = appSettings;
            _connectionString = connectionString;
        }
        [HttpPost]
        [Route("GateWay")]
        public HttpResponseMessageResult GateWay()
        {
            HttpResponseMessage mess = new HttpResponseMessage();
            Logger logger = new Logger(_connectionString, _appSettings);
           
            string reqBody = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string status = "info";
            string descript = "успех";
            string idPlatform = "";
            string content = "";
            try
            {
                Integration.Integration integration = new Integration.Integration(_appSettings, _connectionString);

                //CheckReqXfer req = new CheckReqXfer();
                content = integration.GateWay(reqBody);
                if (content != null && content != "")
                {
                    dynamic data = JObject.Parse(content);
                    dynamic body;
                    if (data.body.Type == JTokenType.Array)
                    {
                        body = data.body[0];
                    }
                    else
                    {
                        body = data.body;
                    }

                    if (data.responseCode != "0")
                    {
                        data.responseCode = "-1";
                        body.responseCode = "-1";
                    }
                    data.body = body;
                    content = Convert.ToString(data);
                    idPlatform = data.idPlatform;
                }
                else
                {
                    content = "{\n  \"version\": \"1.0\",\n  \"type\": \"002\",\n  \"id\": \"\",\n  \"dateTime\": \""+DateTime.Now.ToString()+"\",\n  \"source\": \"SOFT_LOGIC\",\n  \"restartAllowed\": 0,\n  \"responseCode\": \"-1\",\n  \"responseMessage\": \"Ошибка в адаптере\",\n  \"responseErrBackTrace\": \"Ошибка в адаптере\",\n  \"body\": {\n    \"responseCode\": \"-1\",\n    \"responseMessage\": \"Ошибка в адаптере\",\n    \"responseErrBackTrace\": \"Ошибка в адаптере\"\n  }\n}";
                }
            }

            catch (Exception ex)
            {
                status = "error";
                descript = ex.Message;
                content = "{\n  \"version\": \"1.0\",\n  \"type\": \"002\",\n  \"id\": \"\",\n  \"dateTime\": \"" + DateTime.Now.ToString() + "\",\n  \"source\": \"SOFT_LOGIC\",\n  \"restartAllowed\": 0,\n  \"responseCode\": \"-1\",\n  \"responseMessage\": \"Ошибка в адаптере\",\n  \"responseErrBackTrace\": \"Ошибка в адаптере\",\n  \"body\": {\n    \"responseCode\": \"-1\",\n    \"responseMessage\": \"Ошибка в адаптере\",\n    \"responseErrBackTrace\": \"Ошибка в адаптере\"\n  }\n}";
            }
            //logger.InsertLog(new Log { id = id, Descript = descript, EventName = "PayLogic->HbkGate/Esb", Status = status, request = reqBody, response = JsonConvert.SerializeObject(content) });
            
            logger.StateLog(new Log
            {
                idPlatform = idPlatform,

                colvirPayDate = DateTime.Now,
                colvirPayRequest = reqBody,
                colvirPayResponse = JsonConvert.SerializeObject(content)
            });

            mess.Content = new StringContent(content);
            mess.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //object response = JsonConvert.SerializeObject(content);

            HttpResponseMessage respMess = mess;


            HttpResponseMessageResult result = new HttpResponseMessageResult(respMess);

            return result;
        }        
    }
}
