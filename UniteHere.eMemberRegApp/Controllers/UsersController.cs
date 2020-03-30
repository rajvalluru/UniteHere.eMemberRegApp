using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Infrastructure;
using UniteHere.eMemberRegApp.Models;
using UniteHere.eMemberRegApp.Repositories;
using UniteHere.eMemberRegApp.ViewModels;

namespace UniteHere.eMemberRegApp.Controllers {
  [Authorize]
  public class UsersController : BaseApiController {
    private ApplicationUserManager _applicationUserManager = null;
    ApplicationUserManager ApplicationUserManager {
      get {
        if (_applicationUserManager == null)
          _applicationUserManager = Request.GetOwinContext().Get<ApplicationUserManager>();
        return (_applicationUserManager);
      }
    }

    //  GET api/Users 
    public IList<IdentityUserViewModel> Get(string local_number = null, int offset = 0, int limit = 25) {
      IQueryable<ApplicationUser> users = ApplicationUserManager.Users;
      if (this.TokenId.IsSuperAdmin && local_number != null)
        users = users.Where(u => u.LocalNumber == local_number);
      if (this.TokenId.IsLocalAdmin)
        users = users.Where(u => u.LocalNumber == this.TokenId.LocalNumber);

      users = users.OrderBy(u => u.UserName).Skip(offset).Take(limit);

      var resultedUsers = users.ToList();

      return resultedUsers.Select(u => new IdentityUserViewModel() {
        Id = u.Id, UserName = u.UserName, Email = u.Email, LocalNumber = u.LocalNumber, Role = u.Role, FirstName = u.FirstName, LastName = u.LastName, PhoneNumber = u.PhoneNumber
      }).ToList();
    }

    //  POST api/Users 
    public async Task<IHttpActionResult> Post(RegisterUserViewModel model) {
      if (string.IsNullOrEmpty(model.LocalNumber))
        return BadRequest("Local Number cannot be empty!!!");
      if (!(this.TokenId.IsSuperAdmin || this.TokenId.IsLocalAdmin))
        return BadRequest("Unauthorized to add new users!");
      if (this.TokenId.IsLocalAdmin && model.LocalNumber != this.TokenId.LocalNumber)
        return BadRequest("Unauthorized to add new users to other locals");
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }
      if(model.UserName.Length < 4)
        return BadRequest("Username cannot be less than 4 characters");

      var existingUser = await ApplicationUserManager.FindByNameAsync(model.UserName);
      if (existingUser != null)
        return BadRequest("User with same username exists! Chose a different username!!");
      //Sanitize the role
      string role = "Basic_User";
      switch (model.Role.ToLower()) {
        case "super_admin":
          role = "Super_Admin";
          break;
        case "local_admin":
          role = "Local_Admin";
          break;
        case "cashier":
          role = "Cashier";
          break;
        default: role = "Basic_User"; break;
      }
      var user = new ApplicationUser() {
        UserName = model.UserName, Email = model.Email, LocalNumber = model.LocalNumber, Role = role
        , FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber
      };

      IdentityResult result = await ApplicationUserManager.CreateAsync(user, model.Password);

      if (!result.Succeeded) {
        return GetErrorResult(result);
      }

      return Ok();
    }

    //  PUT api/Users/{uuid:guid} 
    public async Task<IHttpActionResult> Put(string uuid, IdentityUserViewModel userVM) {
      if (userVM == null)
        return BadRequest("Argument Null");

      if (string.IsNullOrEmpty(userVM.LocalNumber))
        return BadRequest("Local Number cannot be empty!!!");
      if (userVM.UserName.Length < 4)
        return BadRequest("Username cannot be less than 4 characters");

      var user = await ApplicationUserManager.FindByIdAsync(uuid);
      if (user == null)
        return NotFound();
      if (!(this.TokenId.IsSuperAdmin || this.TokenId.IsLocalAdmin))
        return BadRequest("Insufficient permissions to update. Must be Super Admin or Local Admin.");
      if (this.TokenId.IsLocalAdmin && user.LocalNumber != this.TokenId.LocalNumber)
        return BadRequest("Local Admin can only update the users belonging to the local !!");

      user.Email = userVM.Email;
      user.FirstName = userVM.FirstName;
      user.LastName = userVM.LastName;
      user.UserName = userVM.UserName;
      user.PhoneNumber = userVM.PhoneNumber;
      user.LocalNumber = userVM.LocalNumber;
      user.Role = userVM.Role;

      //Sanitize the role
      string role = "Basic_User";
      switch (user.Role.ToLower()) {
        case "super_admin":
          role = "Super_Admin";
          break;
        case "local_admin":
          role = "Local_Admin";
          break;
        case "cashier":
          role = "Cashier";
          break;
        default: role = "Basic_User"; break;
      }
      user.Role = role;

      IdentityResult result = await ApplicationUserManager.UpdateAsync(user);

      if (!result.Succeeded) {
        return GetErrorResult(result);
      }

      return Ok();
    }

    //  DELETE api/Users/{uuid:guid} 
    public async Task<IHttpActionResult> Delete(string uuid) {
      var user = await ApplicationUserManager.FindByIdAsync(uuid);
      if (user == null)
        return NotFound();
      if (!(this.TokenId.IsSuperAdmin || this.TokenId.IsLocalAdmin))
        return BadRequest("Insufficient permissions to update. Must be Super Admin or Local Admin.");
      if (this.TokenId.IsLocalAdmin && user.LocalNumber != this.TokenId.LocalNumber)
        return BadRequest("Local Admin can only delete the users belonging to the local !!");

      IdentityResult result = await ApplicationUserManager.DeleteAsync(user);

      if (!result.Succeeded) {
        return GetErrorResult(result);
      }

      return Ok();
    }

    [HttpGet]
    [Route("api/Me")]
    //  GET /Me 
    public async Task<IdentityUserViewModel> Me() {
      var u = await ApplicationUserManager.FindByIdAsync(this.TokenId.Id);

      var displayUser = new IdentityUserViewModel() {
        Id = u.Id,
        UserName = u.UserName,
        Email = u.Email,
        LocalNumber = u.LocalNumber,
        Role = u.Role,
        FirstName = u.FirstName,
        LastName = u.LastName,
        PhoneNumber = u.PhoneNumber
      };

      return displayUser;
    }

    [HttpPut]
    [Route("api/Me")]
    //  PUT /Me 
    public async Task<IHttpActionResult> Me(ChangeMeProfileModel model) {
      var me = await ApplicationUserManager.FindByIdAsync(this.TokenId.Id);
      IdentityResult result;
      if (await ApplicationUserManager.CheckPasswordAsync(me, model.CurrentPassword)) {
        me.Email = model.User.Email;
        me.FirstName = model.User.FirstName;
        me.LastName = model.User.LastName;
        me.PhoneNumber = model.User.PhoneNumber;
        result = await ApplicationUserManager.UpdateAsync(me);
        if (!result.Succeeded) {
          return GetErrorResult(result);
        }
       
        if (!string.IsNullOrEmpty(model.NewPassword)) {
          result = await ApplicationUserManager.ChangePasswordAsync(me.Id, model.CurrentPassword, model.NewPassword);

          if (!result.Succeeded) {
            return GetErrorResult(result);
          }
        }
      } else {
        result = IdentityResult.Failed("Wrong Password!");
        return GetErrorResult(result);
      }
      return Ok();
    }

    [HttpPost]
    [Route("api/Users/ResetPassword")]
    public async Task<IHttpActionResult> SetPassword(ResetPasswordModel model) {
      if (!(this.TokenId.IsSuperAdmin || this.TokenId.IsLocalAdmin))
        return BadRequest("Insufficient permissions to update. Must be Super Admin or Local Admin.");

      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      var user = await ApplicationUserManager.FindByIdAsync(model.Id);
      if (user == null)
        return NotFound();

      if (this.TokenId.IsLocalAdmin && user.LocalNumber != this.TokenId.LocalNumber)
        return BadRequest("Local Admin can only update the users belonging to the local !!");

      string token = await ApplicationUserManager.GeneratePasswordResetTokenAsync(user.Id);
      IdentityResult result = await ApplicationUserManager.ResetPasswordAsync(model.Id, token, model.NewPassword );

      if (!result.Succeeded) {
        return GetErrorResult(result);
      }

      return Ok();
    }

    [HttpPost]
    [Route("api/import_users/{local_number}")]
    public async Task<IHttpActionResult> ImportLocalUsers(string local_number) {

      // Check if the request contains multipart/form-data.
      if (!Request.Content.IsMimeMultipartContent()) {
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
      }

      //var uriPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
      //var path = new Uri(uriPath).LocalPath;
      string root = System.Web.HttpContext.Current.Server.MapPath("~/Uploads");//path + (@"\App_Data");
      var provider = new MultipartFormDataStreamProvider(root);

      try {
        // Read the form data.
        await Request.Content.ReadAsMultipartAsync(provider);

        if (provider.FileData.Count > 1)
          return BadRequest("Cannot provide more than one file!!!");

        foreach (MultipartFileData file in provider.FileData) {
          Trace.WriteLine(file.Headers.ContentDisposition.FileName);
          Trace.WriteLine("Server file path: " + file.LocalFileName);
          var result = await CreateLocalUsers(file.LocalFileName, file.Headers.ContentDisposition.FileName, local_number);
          return (result);
        }
        return Ok();
      } catch (System.Exception e) {
        return BadRequest(e.Message);
      }

    }

    private async Task<IHttpActionResult> CreateLocalUsers(string fileName, string originalFileName, string localNumber) {
      using (var rd = new StreamReader(File.OpenRead(fileName))) {
        if (!rd.EndOfStream)
          rd.ReadLine();    // Skip the header
        int totalLineCount = 0;
        int successCount = 0;
        var mgr = this.ApplicationUserManager;
        while (!rd.EndOfStream) {
          var ln = rd.ReadLine();
          totalLineCount++;
          var values = ln.Split(',');
          if (ln.Length > 6 && values[2] == localNumber) {
            var newUser = new RegisterUserViewModel() {
              UserName = values[0],
              Email = values[1],
              LocalNumber = values[2],
              FirstName = values[3],
              LastName = values[4],
              PhoneNumber = values[5],
              Password = values[6],
              Role = values[7]
            };

            var existingUser = await mgr.FindByNameAsync(newUser.UserName);
            if (existingUser == null) {
              var user = new ApplicationUser() {
                UserName = newUser.UserName,
                Email = newUser.Email,
                LocalNumber = newUser.LocalNumber,
                Role = newUser.Role,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                PhoneNumber = newUser.PhoneNumber
              };

              IdentityResult result = await mgr.CreateAsync(user, newUser.Password);

              if (result.Succeeded) {
                successCount++;
              }
            }
          }
        }
        return Ok(new ImportFileResult() { FileName = originalFileName, NoOfLines = totalLineCount, SuccessCount = successCount, FailureCount = totalLineCount - successCount });
      }
    }

    private IHttpActionResult GetErrorResult(IdentityResult result) {
      if (result == null) {
        return InternalServerError();
      }

      if (!result.Succeeded) {
        if (result.Errors != null) {
          foreach (string error in result.Errors) {
            ModelState.AddModelError("", error);
          }
        }

        if (ModelState.IsValid) {
          // No ModelState errors are available to send, so just return an empty BadRequest.
          return BadRequest();
        }

        return BadRequest(ModelState);
      }

      return null;
    }

  }
}
