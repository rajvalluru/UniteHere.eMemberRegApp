namespace UniteHere.eMemberRegAppWeb.Migrations {
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using Models;
  using System;
  using System.Data.Entity;
  using System.Data.Entity.Migrations;
  using System.Linq;
  using UniteHere.eMemberRegAppWeb;

  internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext> {
    public Configuration() {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(ApplicationDbContext context) {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data. E.g.
      //
      //Set up initial users
      CreateSuperUser(context);
    }
    private static void CreateSuperUser(ApplicationDbContext context) {
      var sadmin = new ApplicationUser {
        Email = "Admin@Unitehere.org",
        UserName = "eMbrAdmin",
        FirstName = "Super",
        LastName = "Admin",
        LocalNumber = "IU",
        Role = "Super_Admin"
      };
      //var passwordHash = new PasswordHasher();
      //string password = passwordHash.HashPassword("Password12!!");
      //context.Users.AddOrUpdate(u => u.UserName, sadmin);

      var mgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

      if (!(context.Users.Any(u => u.UserName == "eMbrAdmin"))) {
        mgr.Create(sadmin, "Password12!!");
      }
    }

  }
}
