using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tincoff_Gate.Models;

namespace Tincoff_Gate.Integration
{
    public class Logger
    {
        private IOptions<ConnectionStrings> _connectionString;
        private IOptions<AppSettings> _appSettings;

        public Logger(IOptions<ConnectionStrings> connectionString, IOptions<AppSettings> appSettings)
        {
            _connectionString = connectionString;
            _appSettings = appSettings;
        }

        //Логирование выполняется асинхронно
        public async void checkLog(Log log)
        {
            await Task.Run(() => CheckLog(log));
        }

        public async void payLog(Log log)
        {
            await Task.Run(() => PayLog(log));
        }
        public async void colvirPayLog(Log log)
        {
            await Task.Run(() => PayColvirLog(log));
        }

        public async void amlLog(Log log)
        {
            await Task.Run(() => AmlLog(log));
        }

        public async void amlLogRec(Log log)
        {
            await Task.Run(() => AmlLogRec(log));
        }

        public void CheckLog(Log log)
        {
            try
            {
               

                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("CheckLog", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("ID_", OracleDbType.Varchar2, log.id, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("IDCHECK_", OracleDbType.Varchar2, log.idCheck, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("IDPLATFORM_", OracleDbType.Varchar2, log.idPlatform, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("STATE_", OracleDbType.Varchar2, log.state, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("TRNCOMMENT_", OracleDbType.Varchar2, log.comment, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("ORIGINATORID_", OracleDbType.Varchar2, log.originatorid, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("ORIGINATORBANK_", OracleDbType.Varchar2, log.originatorBank, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("ORIGINATORSUMM_", OracleDbType.Double, log.originatorSumm, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKREQUEST_", OracleDbType.Clob, log.checkRequest, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKRESPONSE_", OracleDbType.Clob, log.checkResponse, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKDATE_", OracleDbType.Date, log.checkDate, System.Data.ParameterDirection.Input);

                    com.Parameters.Add("AMLREQUEST_", OracleDbType.Clob, log.amlRequest, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("AMLRESPONSE_", OracleDbType.Clob, log.amlResponse, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKAMLDATE_", OracleDbType.Date, log.amlCheckDate, System.Data.ParameterDirection.Input);

                    com.Parameters.Add("AMLReceiverREQUEST_", OracleDbType.Clob, log.amlRequestRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("AMLreceiverRESPONSE_", OracleDbType.Clob, log.amlResponseRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKAMLReceiverDATE_", OracleDbType.Date, log.amlCheckDateRec, System.Data.ParameterDirection.Input);

                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void PayLog(Log log)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("PayLog", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("IDPLATFORM_", OracleDbType.Varchar2, log.idPlatform, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("STATE_", OracleDbType.Varchar2, log.state, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("TRNCOMMENT_", OracleDbType.Varchar2, log.comment, System.Data.ParameterDirection.Input);                   
                    com.Parameters.Add("PAYREQUEST_", OracleDbType.Clob, log.payRequest, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("PAYRESPONSE_", OracleDbType.Clob, log.payResponse, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("PAYDATE_", OracleDbType.Date, log.payDate, System.Data.ParameterDirection.Input);                   
                    com.Parameters.Add("amlRequest_", OracleDbType.Clob, log.amlRequest, System.Data.ParameterDirection.Input);                   
                    com.Parameters.Add("amlResponse_", OracleDbType.Clob, log.amlResponse, System.Data.ParameterDirection.Input);                   
                    com.Parameters.Add("amlCheckDate_", OracleDbType.Date, log.amlCheckDate, System.Data.ParameterDirection.Input);

                    com.Parameters.Add("amlRecRequest_", OracleDbType.Clob, log.amlRequestRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("amlRecResponse_", OracleDbType.Clob, log.amlResponseRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("amlRecCheckDate_", OracleDbType.Date, log.amlCheckDateRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void PayColvirLog(Log log)
        {
            try
            {
                

                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("PayColvirLog", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("IDPLATFORM_", OracleDbType.Varchar2, log.idPlatform, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("STATE_", OracleDbType.Varchar2, log.state, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("TRNCOMMENT_", OracleDbType.Varchar2, log.comment, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("COLVIRPAYREQUEST_", OracleDbType.Clob, log.colvirPayRequest, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("COLVIRPAYRESPONSE_", OracleDbType.Clob, log.colvirPayResponse, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("COLVIRPAYDATE_", OracleDbType.Date, log.colvirPayDate, System.Data.ParameterDirection.Input);

                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void AmlLog(Log log)
        {
            try
            {
                

                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("AmlLog", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("IDPLATFORM_", OracleDbType.Varchar2, log.idPlatform, System.Data.ParameterDirection.Input);
                    
                    com.Parameters.Add("AMLREQUEST_", OracleDbType.Clob, log.amlRequest, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("AMLRESPONSE_", OracleDbType.Clob, log.amlResponse, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKAMLDATE_", OracleDbType.Date, log.amlCheckDate, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void AmlLogRec(Log log)
        {
            try
            {


                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("AmlLogReceiver", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("IDPLATFORM_", OracleDbType.Varchar2, log.idPlatform, System.Data.ParameterDirection.Input);

                    com.Parameters.Add("AMLReceiverREQUEST_", OracleDbType.Clob, log.amlRequestRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("AMLreceiverRESPONSE_", OracleDbType.Clob, log.amlResponseRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("CHECKAMLReceiverDATE_", OracleDbType.Date, log.amlCheckDateRec, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }
        

        public void StateLog(Log log)
        {
            try
            {
                

                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("StateLog", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("IDPLATFORM_", OracleDbType.Varchar2, log.idPlatform, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("STATEREQUEST_", OracleDbType.Clob, log.stateRequest, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("STATERESPONSE_", OracleDbType.Clob, log.stateResponse, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("STATEDATE_", OracleDbType.Date, log.stateDate, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("state_", OracleDbType.Varchar2, log.state, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("comment_", OracleDbType.Varchar2, log.comment, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }

        public Limit GetLimit(string origintorId)
        {
            try
            {
                Limit limit = new Limit();

                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("GetLimit", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("IDClient_", OracleDbType.Varchar2, origintorId, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();

                    limit.countTrnDay = dr["CountTrnDay"] != DBNull.Value ? Convert.ToDouble(dr["CountTrnDay"]) : 0;
                    limit.sumTrnDay = dr["SummTrnDay"] != DBNull.Value ? Convert.ToDouble(dr["SummTrnDay"]) : 0;
                    limit.sumTrnMonth = dr["SummTrnMonth"] != DBNull.Value ? Convert.ToDouble(dr["SummTrnMonth"]) : 0;
                    

                    con.Close();
                }
                return limit;

            }
            catch (Exception ex)
            {
                //throw (new Exception(ex.Message));
                return new Limit();
            }

        }

        public Log GetLog(string platformId)
        {
            try
            {
                Log log = new Log();

                using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                {
                    con.Open();
                    OracleCommand com = new OracleCommand("getLog", con);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("platformId_", OracleDbType.Varchar2, platformId, System.Data.ParameterDirection.Input);
                    com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    OracleDataReader dr;

                    dr = com.ExecuteReader();
                    dr.Read();
                    
                    log.id = dr["id"] != DBNull.Value ? Convert.ToString(dr["id"]) : "";
                    log.idCheck = dr["idCheck"] != DBNull.Value ? Convert.ToString(dr["idCheck"]) : "";
                    log.idPlatform = dr["idPlatform"] != DBNull.Value ? Convert.ToString(dr["idPlatform"]) : "";
                    log.comment = dr["trncomment"] != DBNull.Value ? Convert.ToString(dr["trncomment"]) : "";
                    log.originatorid = dr["originatorid"] != DBNull.Value ? Convert.ToString(dr["originatorid"]) : "";
                    log.originatorBank = dr["originatorBank"] != DBNull.Value ? Convert.ToString(dr["originatorBank"]) : "";
                    log.originatorSumm = dr["originatorSumm"] != DBNull.Value ? Convert.ToDouble(dr["originatorSumm"]) : 0;
                    log.checkRequest = dr["CHECKREQUEST"] != DBNull.Value ? Convert.ToString(dr["CHECKREQUEST"]) : "";
                    log.checkResponse = dr["CHECKRESPONSE"] != DBNull.Value ? Convert.ToString(dr["CHECKRESPONSE"]) : "";
                    log.payRequest = dr["PAYREQUEST"] != DBNull.Value ? Convert.ToString(dr["PAYREQUEST"]) : "";
                    log.payResponse = dr["PAYRESPONSE"] != DBNull.Value ? Convert.ToString(dr["PAYRESPONSE"]) : "";
                    log.colvirPayRequest = dr["COLVIRPAYREQUEST"] != DBNull.Value ? Convert.ToString(dr["COLVIRPAYREQUEST"]) : "";
                    log.colvirPayResponse = dr["COLVIRPAYRESPONSE"] != DBNull.Value ? Convert.ToString(dr["COLVIRPAYRESPONSE"]) : "";
                    log.amlRequest = dr["AMLREQUEST"] != DBNull.Value ? Convert.ToString(dr["AMLREQUEST"]) : "";
                    log.amlResponse = dr["AMLRESPONSE"] != DBNull.Value ? Convert.ToString(dr["AMLRESPONSE"]) : "";
                    log.checkDate = dr["CHECKDATE"] != DBNull.Value ? Convert.ToDateTime(dr["CHECKDATE"]) : DateTime.Now;
                    log.payDate = dr["PAYDATE"] != DBNull.Value ? Convert.ToDateTime(dr["PAYDATE"]) : DateTime.Now;
                    log.colvirPayDate = dr["COLVIRPAYDATE"] != DBNull.Value ? Convert.ToDateTime(dr["COLVIRPAYDATE"]) : DateTime.Now;
                    log.amlCheckDate = dr["CHECKAMLDATE"] != DBNull.Value ? Convert.ToDateTime(dr["CHECKAMLDATE"]) : DateTime.Now;
                    log.state = dr["STATE"] != DBNull.Value ? Convert.ToString(dr["STATE"]) : "";
                    log.stateRequest = dr["STATEREQUEST"] != DBNull.Value ? Convert.ToString(dr["STATEREQUEST"]) : "";
                    log.stateResponse = dr["STATERESPONSE"] != DBNull.Value ? Convert.ToString(dr["STATERESPONSE"]) : "";
                    log.stateDate = dr["STATEDATE"] != DBNull.Value ? Convert.ToDateTime(dr["STATEDATE"]) : DateTime.Now;

                    con.Close();
                }
                return log;

            }
            catch (Exception ex)
            {
                //throw (new Exception(ex.Message));
                return new Log();
            }
            
        }
    }
}
