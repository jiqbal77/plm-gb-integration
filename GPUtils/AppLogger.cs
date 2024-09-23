using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GPUtils
{
    public sealed class AppLogger
    {
        #region private members
        private static ILog _log;
        private static string serviceName;
        private static string msg;
   

        private AppLogger() 
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configFileDirectory = Path.Combine(baseDirectory, "log4net.config");

            FileInfo configFileInfo = new FileInfo(configFileDirectory);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);

            _log = log4net.LogManager.GetLogger("log4netFileLogger");
            serviceName = ConfigurationManager.AppSettings["serviceName"].ToString();
            msg = "";
        }

        // The Singleton's instance is stored in a static field. There there are
        // multiple ways to initialize this field, all of them have various pros
        // and cons. In this example we'll show the simplest of these ways,
        // which, however, doesn't work really well in multithreaded program.
        private static AppLogger _instance;
        #endregion

        #region public methods
        // This is the static method that controls the access to the singleton
        // instance. On the first run, it creates a singleton object and places
        // it into the static field. On subsequent runs, it returns the client
        // existing object stored in the static field.
        public static AppLogger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AppLogger();
            }
            return _instance;
        }

        public  void Info(string value,string traceId="test123")
        {
          
            SendMessageToGSILogger(value, traceId,"info");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Info(msg);
           
        }
        public  void Debug(string value, string traceId = "test123")
        {
            SendMessageToGSILogger(value, traceId, "debug");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Debug(msg);
        }
        public  void Warn(string value, string traceId = "test123")
        {
            
            SendMessageToGSILogger(value, traceId, "warn");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Warn(msg);
        }
        public  void Error(string value, string traceId = "test123")
        {
           
            SendMessageToGSILogger(value, traceId, "error");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Error(msg);
        }
        public  void Fatal(string value, string traceId = "test123")
        {
           
            SendMessageToGSILogger(value, traceId, "fatal");
            msg = "{TraceId:" + traceId + " , Service: " + serviceName + " , message:" + value + " }";
            _log.Fatal(msg);
        }

        public async void SendMessageToGSILogger(string msg,string traceId="12345",string type="info")
        {
            try
            {    
                string loggerUrl = ConfigurationManager.AppSettings["loggerGSIUrl"].ToString();
                string gsiLoggerEnable = ConfigurationManager.AppSettings["loggerGSIEnable"].ToString();
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
             
                if (gsiLoggerEnable.Equals("TRUE"))
                  {
                    _log.Debug("GPUtils.CustomLogger@sendMessageToGSILogger");
                    // var response = client.ExecuteAsync(request);

                  var response =   client.ExecuteAsync(request);
                   /* if (response.StatusCode == HttpStatusCode.OK)
                    {
                        _log.Debug("GPUtils.CustomLogger@sendMessageToGSILogger Logs sent to GSI successfully.");
                    }
                    else
                    {
                        _log.Error($"GPUtils.CustomLogger@sendMessageToGSILogger Logs not sent to GSI Error Occured:. {response.ErrorMessage}");
                    }*/
                }
                 
            }
            catch (Exception ex)
            {
                _log.Fatal("GPUtils.CustomLogger@sendMessageToGSILogger exception:" + ex.ToString());
            }
        }
        #endregion
    }
}
