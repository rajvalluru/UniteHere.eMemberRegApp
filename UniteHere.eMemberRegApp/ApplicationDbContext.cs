using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Models;

namespace UniteHere.eMemberRegApp {
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext() : base("DefaultConnection") { }
    static ApplicationDbContext() {
    }

    public static ApplicationDbContext Create() {
      //Console.WriteLine("ApplicationDbContext Create()...");
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

    public IDbSet<EMemberRegistration> EMemberRegistration { get; set; }
    public IDbSet<Error> Error { get; set; }
    public IDbSet<ReportDef> ReportDef { get; set; }
    public IDbSet<ReportSecurity> ReportSecurity { get; set; }
    public IDbSet<ReportParameter> ReportParameter { get; set; }
    #endregion

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      //disable initializer
      Database.SetInitializer<ApplicationDbContext>(null);
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
      modelBuilder.Ignore<EMemberRegistrationModel>();
      base.OnModelCreating(modelBuilder);
    }

  }

}