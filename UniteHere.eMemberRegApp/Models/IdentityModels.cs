﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;

namespace UniteHere.eMemberRegApp.Models {
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LocalNumber { get; set; }
    public string Role { get; set; }

    public bool IsSuperAdmin { get { return Role.Equals("Super_Admin"); } }
    public bool IsLocalAdmin { get { return Role.Equals("Local_Admin"); } }
    public bool IsApi { get { return Role.Equals("Api"); } }
    public bool IsBasicUser { get { return Role.Equals("Basic_User"); } }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType) {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
      // Add custom user claims here
      userIdentity.AddClaim(new Claim(ClaimTypes.Sid, this.Id));
      userIdentity.AddClaim(new Claim(ClaimTypes.Name, this.UserName));
      userIdentity.AddClaim(new Claim("LocalNumber", this.LocalNumber));
      userIdentity.AddClaim(new Claim(ClaimTypes.Role, this.Role));

      return userIdentity;
    }

  }

}