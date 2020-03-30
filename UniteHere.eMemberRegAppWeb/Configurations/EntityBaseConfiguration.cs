using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegAppWeb.Configurations
{
    public class EntityBaseConfiguration<T> : EntityTypeConfiguration<T> where T:class, IEntityBase
    {
        public EntityBaseConfiguration()
        {
            HasKey(e => e.Id);
            Property(e => e.Id).HasMaxLength(128);
        }
    }
}
