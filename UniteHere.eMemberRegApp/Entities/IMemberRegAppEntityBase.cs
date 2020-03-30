using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegApp.Entities {
  public interface IMemberRegAppEntityBase : IEntityBase {

    string CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    string ModifiedBy { get; set; }
    DateTime? ModifiedOn { get; set; }

    int RowVersion { get; set; }

  }
}
