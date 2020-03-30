using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

namespace UniteHere.eMemberRegApp {
  public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
      // Web API configuration and services
      config.EnableCors(new EnableCorsAttribute("*", "*", "GET, POST, PUT, DELETE, OPTIONS, PATCH"));
      config.MessageHandlers.Add(new PreflightRequestsHandler());

      // Configure Web API to use only bearer token authentication.
      config.SuppressDefaultHostAuthentication();
      config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
      config.Filters.Add(new EMemberRegAppExceptionFilter());

      // configure Services - ExceptionLogger
      config.Services.Replace(typeof(IExceptionLogger), new UnhandledExceptionLogger());

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{uuid}",
          defaults: new { uuid = RouteParameter.Optional }
      );
    }
  }
}
