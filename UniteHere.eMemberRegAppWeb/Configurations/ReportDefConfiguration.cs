using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegAppWeb.Configurations {
  public class ReportDefConfiguration : EMemberRegAppEntityBaseConfiguration<ReportDef> {
    public ReportDefConfiguration() {
      Property(s => s.Name).IsRequired().HasMaxLength(256);
      Property(s => s.Description).HasMaxLength(512);
    }
  }

  public class ReportSecurityConfiguration : EntityBaseConfiguration<ReportSecurity> {
    public ReportSecurityConfiguration() {
      Property(s => s.ReportDefId).HasMaxLength(128);
      Property(s => s.Role).IsRequired().HasMaxLength(20);
    }
  }

  public class ReportParameterConfiguration : EntityBaseConfiguration<ReportParameter> {
    public ReportParameterConfiguration() {
      Property(s => s.ReportDefId).HasMaxLength(128);
      Property(s => s.ParameterName).IsRequired().HasMaxLength(20);
      Property(s => s.DataType).HasMaxLength(20);
      Property(s => s.DefaultValue).HasMaxLength(128);
      Property(s => s.Description).HasMaxLength(256);
    }
  }
}