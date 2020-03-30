using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegAppWeb.Configurations;
using UniteHere.eMemberRegAppWeb.Models;

namespace UniteHere.eMemberRegAppWeb {
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false) {
    }

    public static ApplicationDbContext Create() {
      return new ApplicationDbContext();
    }

    public virtual void Commit() {
      try {
        base.SaveChanges();
      } catch (DbUpdateException ex) {
        throw ex;
      } catch (Exception e) {
        throw e;
      }
    }

    #region Entity Sets

    public DbSet<EMemberRegistration> EMemberRegistration { get; set; }
    public IDbSet<Error> Error { get; set; }
    public IDbSet<ReportDef> ReportDef { get; set; }
    public IDbSet<ReportSecurity> ReportSecurity { get; set; }
    public IDbSet<ReportParameter> ReportParameter { get; set; }
    #endregion

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      //disable initializer
      //Database.SetInitializer<ApplicationDbContext>(null);
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

      base.OnModelCreating(modelBuilder);
      modelBuilder.Configurations.Add(new EMemberRegistrationConfiguration());
      modelBuilder.Configurations.Add(new ReportDefConfiguration());
      modelBuilder.Configurations.Add(new ReportSecurityConfiguration());
      modelBuilder.Configurations.Add(new ReportParameterConfiguration());

      modelBuilder.Entity<ApplicationUser>().Property(t => t.FirstName).HasMaxLength(100);
      modelBuilder.Entity<ApplicationUser>().Property(t => t.LastName).HasMaxLength(100);
      modelBuilder.Entity<ApplicationUser>().Property(t => t.LocalNumber).HasMaxLength(20);
      modelBuilder.Entity<ApplicationUser>().Property(t => t.Role).HasMaxLength(20);
      modelBuilder.Entity<ApplicationUser>().Property(t => t.PhoneNumber).HasMaxLength(20);
    }

    public System.Data.Entity.DbSet<UniteHere.eMemberRegAppWeb.Models.EMemberRegistrationViewModel> EMemberRegistrationViewModels { get; set; }
  }
}