using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniteHere.eMemberRegAppWeb.Models {
  public class TokenIdentityUser {
    public string Id { get; set; }
    public string Name { get; set; }
    public string LocalNumber { get; set; }
    public string Role { get; set; }
    public bool IsSuperAdmin { get { return Role.Equals("Super_Admin"); } }
    public bool IsLocalAdmin { get { return Role.Equals("Local_Admin"); } }
    public bool IsApi { get { return Role.Equals("Api"); } }
    public bool IsBasicUser { get { return Role.Equals("Basic_User"); } }
  }
}