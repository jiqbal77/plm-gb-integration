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
    public static class MyLogger
    {
        public static ILog _log;
        public static string serviceName;
        public static string msg;
        public static void init()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configFileDirectory = Path.Combine(baseDirectory, "log4net.config");

            FileInfo configFileInfo = new FileInfo(configFileDirectory);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);

            _log = log4net.LogManager.GetLogger("log4netFileLogger");
             serviceName = ConfigurationManager.AppSettings["serviceName"].ToString();
            msg = "";

        }

        public static void Info(string value,string traceId="test123")
        {
            init();
            sendMessageToGSILogger(value, traceId,"info");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Info(msg);
           
        }

        public static void Debug(string value, string traceId = "test123")
        {
            init();
            sendMessageToGSILogger(value, traceId, "debug");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Debug(msg);
        }
        public static void Warn(string value, string traceId = "test123")
        {
            init();
            sendMessageToGSILogger(value, traceId, "warn");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Warn(msg);
        }
        public static void Error(string value, string traceId = "test123")
        {
            init();
            sendMessageToGSILogger(value, traceId, "error");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Error(msg);
        }
        public static void Fatal(string value, string traceId = "test123")
        {
            init();
            sendMessageToGSILogger(value, traceId, "fatal");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Fatal(msg);
        }

        public  static void sendMessageToGSILogger(string msg,string traceId="12345",string type="info")
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
