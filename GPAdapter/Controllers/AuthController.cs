using GPAdapter.Filter;
using GPBusinessLayer.Business.Auth;
using GPDataLayer.Data.Enums;
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
using System.Text;
using System.Web.Http;

namespace GPAdapter.Controllers
{
 
    public class AuthController : ApiController
    {

        private CustomLogger logger;

        [HttpPost]
        [BasicAuthenticationFilter]
        public IHttpActionResult GetToken([FromBody] AuthDAO authPayload)
        {

            logger = new CustomLogger();
            logger.Debug("GPAdapter.ApiController@GetToken");
            try
            {
                AuthBL authBL = new AuthBL();
               
                if (authBL.verifyCredentials(authPayload))
                {
                    var jwt_token = authBL.generateJWT();

                    return Content(HttpStatusCode.OK, new GenericResponseAPI<string>(true, "Authorized successfully.",jwt_token,ResponseCode.OK));

                }
                else
                {
                    return Content(HttpStatusCode.Unauthorized, new GenericResponseAPI<string>(false, "Unauthorized.", "", ResponseCode.UNAUTHORIZED));
                   
                }
            }catch(Exception ex)
            {
                logger.Error("GPAdapter.ApiController@GetToken exception occured : "+ex.ToString());
                return Content(HttpStatusCode.BadRequest,new GenericResponseAPI<string>(false, ex.ToString(), "", ResponseCode.BAD_REQUEST));
            }
        }

    }
}