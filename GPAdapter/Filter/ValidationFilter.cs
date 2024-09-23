using GPDataLayer.Data.ViewModels;
using GPUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace GPAdapter.Filter
{
    public class ValidationFilter : ActionFilterAttribute
    {
        private AppLogger _logger = AppLogger.GetInstance();
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            _logger.Debug("ValidationFilter@OnActionExecuting");
            try
            {
               
                if (!actionContext.ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var modelStateVal in actionContext.ModelState.Values.Select(d => d.Errors))
                    {
                        errors.AddRange(modelStateVal.Select(error => error.ErrorMessage));
                    }

                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.BadRequest, new GenericResponseAPI<dynamic>(false, "Invalid Payload.Please see data for more details", errors, GPDataLayer.Data.Enums.ResponseCode.BAD_REQUEST)
                        );
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"GPAdapter.Filter.ValidationFilter@OnActionExecuting exception occured: {ex.ToString()}");
                actionContext.Response = actionContext.Request.CreateResponse(
                         HttpStatusCode.BadRequest, new GenericResponseAPI<dynamic>(false, "Invalid Payload. Exception Occured.Please see data for more details", ex.ToString(), GPDataLayer.Data.Enums.ResponseCode.BAD_REQUEST)
                         );

            }
        }
    }
}
