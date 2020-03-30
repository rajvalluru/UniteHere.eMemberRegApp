using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegAppWeb.Infrastructure {
  public class DbFactory : Disposable, IDbFactory {
    ApplicationDbContext dbContext;

    public ApplicationDbContext Init() {
      return dbContext ?? (dbContext = ApplicationDbContext.Create());
    }

    protected override void DisposeCore() {
      if (dbContext != null)
        dbContext.Dispose();
    }
  }
}
