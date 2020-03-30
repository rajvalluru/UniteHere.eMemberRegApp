using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Infrastructure;
using UniteHere.eMemberRegApp.Models;
using UniteHere.eMemberRegApp.Repositories;

namespace UniteHere.eMemberRegApp.Controllers {
  [Authorize]
  public abstract class BaseApiController : ApiController {
    protected IEntityBaseRepository<Error> _errorsRepository;
    protected readonly IUnitOfWork _unitOfWork;
    private IMemberRegAppEntityBaseRepository<EMemberRegistration> _eMemberRegistrationRepository;
    private IMemberRegAppEntityBaseRepository<ReportDef> _reportDefRepository = null;
    private IEntityBaseRepository<ReportSecurity> _reportSecurityRepository = null;
    private IEntityBaseRepository<ReportParameter> _reportParameterRepository = null;

    public IMemberRegAppEntityBaseRepository<EMemberRegistration> EMemberRegistrationRepository {
      get {
        _eMemberRegistrationRepository = (_eMemberRegistrationRepository == null) ? new EMemberRegAppEntityBaseRepository<EMemberRegistration>(DbContext) : _eMemberRegistrationRepository;
        return _eMemberRegistrationRepository;
      }
    }
    public IEntityBaseRepository<Error> ErrorsRepository {
      get {
        _errorsRepository = (_errorsRepository == null) ? new EntityBaseRepository<Error>(DbContext) : _errorsRepository;
        return _errorsRepository;
      }
    }
    public IMemberRegAppEntityBaseRepository<ReportDef> ReportDefRepository {
      get {
        _reportDefRepository = (_reportDefRepository == null) ? new EMemberRegAppEntityBaseRepository<ReportDef>(DbContext) : _reportDefRepository;
        return _reportDefRepository;
      }
    }
    public IEntityBaseRepository<ReportSecurity> ReportSecurityRepository {
      get {
        _reportSecurityRepository = (_reportSecurityRepository == null) ? new EntityBaseRepository<ReportSecurity>(DbContext) : _reportSecurityRepository;
        return _reportSecurityRepository;
      }
    }
    public IEntityBaseRepository<ReportParameter> ReportParameterRepository {
      get {
        _reportParameterRepository = (_reportParameterRepository == null) ? new EntityBaseRepository<ReportParameter>(DbContext) : _reportParameterRepository;
        return _reportParameterRepository;
      }
    }



    public ApplicationDbContext DbContext { get { return Request.GetOwinContext().Get<ApplicationDbContext>(); } }

    private TokenIdentityUser _tokenId = null;
    public TokenIdentityUser TokenId {
      get {
        if (_tokenId == null) {
          //Get the current claims principal
          var id = (ClaimsPrincipal)Thread.CurrentPrincipal;
          _tokenId = new TokenIdentityUser() {
            Id = id.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value
          , Name = id.Identity.Name
          , LocalNumber = id.Claims.FirstOrDefault(c => c.Type == "LocalNumber").Value
          , Role = id.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value
          };
        }
        return _tokenId;
      }
    }
  }
}

