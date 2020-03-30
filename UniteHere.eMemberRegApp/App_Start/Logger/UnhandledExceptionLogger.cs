using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegApp {
  public class UnhandledExceptionLogger  : ExceptionLogger{
    public override void Log(ExceptionLoggerContext context) {
      
      Error error = new Error() {
        Id = Guid.NewGuid().ToString(),
        Message = context.Exception.ToString(),
        StackTrace = context.Exception.StackTrace,
        DateCreated = DateTime.Now
      };
      ApplicationDbContext dbContext =  ApplicationDbContext.Create();

      dbContext.Error.Add(error);
      dbContext.Commit();
    }
  }
}