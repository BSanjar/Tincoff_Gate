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
        public async void log(Log log)
        {
            await Task.Run(() => InsertLog(log));
        }
        public void InsertLog(Log log)
        {
            try
            {
                if (_appSettings.Value.LogAll == "1" || log.Status == "error") { 
                if (log.Descript.Count() > 1000)
                    log.Descript = log.Descript.Substring(0, 1000);
                
                    using (OracleConnection con = new OracleConnection(_connectionString.Value.DefaultConnection))
                    {
                        con.Open();
                        OracleCommand com = new OracleCommand("Insert_Log", con);
                        com.CommandType = System.Data.CommandType.StoredProcedure;

                        com.Parameters.Add("id_", OracleDbType.Varchar2, log.id, System.Data.ParameterDirection.Input);
                        com.Parameters.Add("status_", OracleDbType.Varchar2, log.Status, System.Data.ParameterDirection.Input);
                        com.Parameters.Add("event_", OracleDbType.Varchar2, log.EventName, System.Data.ParameterDirection.Input);
                        com.Parameters.Add("descript_", OracleDbType.Varchar2, log.Descript, System.Data.ParameterDirection.Input);
                        com.Parameters.Add("request_", OracleDbType.Clob, log.request, System.Data.ParameterDirection.Input);
                        com.Parameters.Add("response_", OracleDbType.Clob, log.response, System.Data.ParameterDirection.Input);
                        com.Parameters.Add("curs_", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                        OracleDataReader dr;

                        dr = com.ExecuteReader();
                        dr.Read();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
