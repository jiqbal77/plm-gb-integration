using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Enums
{
   public enum ResponseCode
    {
        OK=200,
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        NOT_FOUND =404,
        METHOD_NOT_ALLOWED = 405,
        INTERNAL_SERVER_ERROR = 500,
        BAD_GATEWAY = 502,
        SERVICE_UNAVAILABLE = 503,
    }
}
