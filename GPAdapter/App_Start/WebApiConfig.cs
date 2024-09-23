using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using GPAdapter.Filter;
using Microsoft.Owin.Security.OAuth;
namespace GPAdapter
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services    
            // config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Filters.Add(new BasicAuthenticationFilter());

            // Add Custom validation filters  
            config.Filters.Add(new ValidationFilter());
//            FluentValidationModelValidatorProvider.Configure(config);

            // Web API routes    
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
             name: "DefaultApi",
             routeTemplate: "api/v1/{controller}/{action}/{id}",
             defaults: new
             {
                 id = RouteParameter.Optional
             }
            );
        }
    }
}
