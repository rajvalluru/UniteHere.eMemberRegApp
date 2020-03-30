using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegAppWeb.Infrastructure {
  public class UnitOfWork : IUnitOfWork {
    private readonly IDbFactory dbFactory;
    private ApplicationDbContext dbContext;

    public UnitOfWork(IDbFactory dbFactory) {
      this.dbFactory = dbFactory;
    }

    public ApplicationDbContext DbContext {
      get { return dbContext ?? (dbContext = dbFactory.Init()); }
    }

    public void Commit() {
      DbContext.Commit();
    }
  }
}
