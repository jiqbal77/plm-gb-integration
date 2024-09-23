using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUtils
{
    public  class CustomLogger
    {

        private readonly ILog _log;
        private string serviceName;
        private string msg;
        public CustomLogger()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configFileDirectory = Path.Combine(baseDirectory, "log4net.config");

            FileInfo configFileInfo = new FileInfo(configFileDirectory);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);

            _log = log4net.LogManager.GetLogger("log4netFileLogger");
             serviceName = ConfigurationManager.AppSettings["serviceName"].ToString();
            msg = "";

        }

        public  void Info(string value,string traceId="test123")
        {
            sendMessageToGSILogger(value, traceId,"info");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Info(msg);
           
        }

        public void Debug(string value, string traceId = "test123")
        {
            sendMessageToGSILogger(value, traceId, "debug");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Debug(msg);
        }
        public void Warn(string value, string traceId = "test123")
        {
            sendMessageToGSILogger(value, traceId, "warn");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Warn(msg);
        }
        public void Error(string value, string traceId = "test123")
        {
            sendMessageToGSILogger(value, traceId, "error");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Error(msg);
        }
        public void Fatal(string value, string traceId = "test123")
        {
            sendMessageToGSILogger(value, traceId, "fatal");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Fatal(msg);
        }

        public void sendMessageToGSILogger(string msg,string traceId="12345",string type="info")
        {
            try
            {
                string loggerUrl = ConfigurationManager.AppSettings["loggerGSIUrl"].ToString();
                var client = new RestClient(loggerUrl);
                client.Timeout = -1;
                var request = new RestRequest(type, Method.POST);
                request.AddHeader("TraceID", traceId);
                request.AddHeader("Content-Type", "application/json");
                  var body = new {
                             LogMessage = new { 
                          Service=serviceName,
                          Message=msg
                      }
                  };

                string jsonBody = JsonConvert.SerializeObject(body);
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
              //  IRestResponse response = client.Execute(request);
            }
            catch (Exception ex)
            {
                _log.Fatal("GPUtils.CustomLogger@sendMessageToGSILogger exception:" + ex.ToString());
            }
        }

    }
}
