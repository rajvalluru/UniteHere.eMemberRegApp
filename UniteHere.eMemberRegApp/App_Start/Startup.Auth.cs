using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using UniteHere.eMemberRegApp.Providers;
using UniteHere.eMemberRegApp.Models;

namespace UniteHere.eMemberRegApp {
  public partial class Startup {
    public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

    public static string PublicClientId { get; private set; }

    public void ConfigureAuth(IAppBuilder app) {
      // Configure the db context and user manager to use a single instance per request
      app.CreatePerOwinContext(ApplicationDbContext.Create);
      app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

      // Enable the application to use a cookie to store information for the signed in user
      app.UseCookieAuthentication(new CookieAuthenticationOptions());
 
      // Configure the application for OAuth based flow
      PublicClientId = "self";
      OAuthOptions = new OAuthAuthorizationServerOptions {
        TokenEndpointPath = new PathString("/Token"),
        Provider = new ApplicationOAuthProvider(PublicClientId),
        AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
        // In production mode set AllowInsecureHttp = false
        AllowInsecureHttp = true
      };

      // Enable the application to use bearer tokens to authenticate users
      app.UseOAuthBearerTokens(OAuthOptions);

    }
  }
}
