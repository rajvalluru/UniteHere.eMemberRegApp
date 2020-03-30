using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegApp.Infrastructure {
  public interface IDbFactory : IDisposable {
    ApplicationDbContext Init();
  }
}
