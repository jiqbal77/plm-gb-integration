using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace GPDataLayer.Data.ViewModels
{
    public class GenericResponse<T>
    {
        public T data { get; set; }
        public bool success { get; set; }
        public string message { get; set; }


        public GenericResponse(T _data,bool _success,string _message)
        {
            data = _data;
            success = _success;
            message = _message;
        }
    }
}
