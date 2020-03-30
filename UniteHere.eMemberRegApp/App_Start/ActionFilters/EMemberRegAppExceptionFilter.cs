using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace UniteHere.eMemberRegApp {
  public class EMemberRegAppExceptionFilter : ExceptionFilterAttribute {
    public override void OnException(HttpActionExecutedContext actionExecutedContext) {
      string message = string.Empty;
      if (actionExecutedContext.Exception.InnerException == null) {
        message = actionExecutedContext.Exception.Message;
      } else {
        message = actionExecutedContext.Exception.InnerException.Message;
      }
      //We can log this exception message to the file or database.  
      var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
        Content = new StringContent("An unhandled exception was thrown by service."),
        ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
      };
      actionExecutedContext.Response = response;
    }
  }
}