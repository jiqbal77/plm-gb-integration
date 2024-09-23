using GPAdapter.Filter;
using GPBusinessLayer.Business.GPTransaction;
using GPDataLayer.Data.ViewModels;
using GPUtils;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace GPAdapter.Controllers
{
    public class GpController : ApiController
    {
       // private CustomLogger logger;
        private AppLogger _logger = AppLogger.GetInstance();
        private GPTransactionBL GPBussinerlayer;
        [HttpPost]
        [BasicAuthenticationFilter]
        [ValidationFilter]
        public IHttpActionResult Transaction([FromBody] GpTransactionDAO gpTransactionDAO)
        {

            _logger.Debug("GPAdapter.GpController@Transaction",gpTransactionDAO.CODetail.ChangeId);
                try
                {
                    GPBussinerlayer = new GPTransactionBL();
                    var response = GPBussinerlayer.InsertDataToGp(gpTransactionDAO);

                    return Content(response.success ? HttpStatusCode.OK:HttpStatusCode.InternalServerError, new GenericResponseAPI<string>(response.success, response.message, "", response.success ? GPDataLayer.Data.Enums.ResponseCode.OK : GPDataLayer.Data.Enums.ResponseCode.INTERNAL_SERVER_ERROR));

                }
                catch (Exception ex)
                {
                   _logger.Error("GpAdapter.GpController@Transaction exception occured:" + ex.ToString(),gpTransactionDAO.CODetail.ChangeId);
                    return Content(HttpStatusCode.InternalServerError, new GenericResponseAPI<string>(false, ex.ToString(), "Exception", GPDataLayer.Data.Enums.ResponseCode.INTERNAL_SERVER_ERROR));
                }
        
        }

    }
}