using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegAppWeb.Configurations {
  public class EMemberRegAppEntityBaseConfiguration<T> : EntityBaseConfiguration<T> where T : class, IMemberRegAppEntityBase {
    public EMemberRegAppEntityBaseConfiguration() : base() {
      Property(p => p.CreatedBy).IsRequired().HasMaxLength(20);
      Property(p => p.CreatedOn).IsRequired();
      Property(p => p.RowVersion).IsRequired();

      Property(p => p.ModifiedBy).HasMaxLength(20);
    }
  }
}
