using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using UniteHere.eMemberRegApp.Models;

namespace UniteHere.eMemberRegApp.Providers {
  public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider {
    private readonly string _publicClientId;

    public ApplicationOAuthProvider(string publicClientId) {
      if (publicClientId == null) {
        throw new ArgumentNullException("publicClientId");
      }

      _publicClientId = publicClientId;
    }

    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {
      var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

      ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

      if (user == null) {
        context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
        context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, Content-Type, Accept, Authorization" });
        context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH" });
        context.SetError("invalid_grant", "The user name or password is incorrect.");
        return;
      }

      ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
         OAuthDefaults.AuthenticationType);
      //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Sid, user.Id));
      //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
      //oAuthIdentity.AddClaim(new Claim("LocalNumber", user.LocalNumber));
      //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

      ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
        CookieAuthenticationDefaults.AuthenticationType);

      AuthenticationProperties properties = CreateProperties(user.UserName, user.Id, user.LastName, user.FirstName, user.Email);
      AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
      context.Validated(ticket);
      context.Request.Context.Authentication.SignIn(cookiesIdentity);
      context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
      context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, Content-Type, Accept, Authorization" });
      context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH" });

    }

    public override Task TokenEndpoint(OAuthTokenEndpointContext context) {
      foreach (KeyValuePair<string, string> property in context.Properties.Dictionary) {
        context.AdditionalResponseParameters.Add(property.Key, property.Value);
      }

      return Task.FromResult<object>(null);
    }

    public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
      // Resource owner password credentials does not provide a client ID.
      if (context.ClientId == null) {
        context.Validated();
      }

      return Task.FromResult<object>(null);
    }

    public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context) {
      if (context.ClientId == _publicClientId) {
        Uri expectedRootUri = new Uri(context.Request.Uri, "/");

        if (expectedRootUri.AbsoluteUri == context.RedirectUri) {
          context.Validated();
        }
      }

      return Task.FromResult<object>(null);
    }

    public static AuthenticationProperties CreateProperties(string userName, string id, string lastName, string firstName, string email) {
      IDictionary<string, string> data = new Dictionary<string, string>
      {
                { "userName", userName }
              , {"id", id }
              , {"lastname", lastName }
              , {"firstname", firstName }
              , {"email",  email}
            };
      return new AuthenticationProperties(data);
    }
  }
}