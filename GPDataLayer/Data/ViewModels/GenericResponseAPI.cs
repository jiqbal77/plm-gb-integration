using GPDataLayer.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.ViewModels
{
    public class GenericResponseAPI<T>
    {
       
        public T data { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public ResponseCode statusCode { get; set; }
    
        public GenericResponseAPI(bool _success,string _message,T _data,ResponseCode _statusCode)
        {
            success = _success;
            message = _message;
            statusCode = _statusCode;
            data = _data;

        }
       
    }
}
