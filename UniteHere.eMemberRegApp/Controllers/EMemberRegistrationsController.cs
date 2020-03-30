using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using Antlr.Runtime;
using Newtonsoft.Json;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Infrastructure;
using UniteHere.eMemberRegApp.Models;
using UniteHere.eMemberRegApp.Repositories;
using UniteHere.eMemberRegApp.TimssDataAccess;

namespace UniteHere.eMemberRegApp.Controllers {
  [Authorize]
  //[AllowAnonymous]
  [Route("~/api/registrations")]
  public class EMemberRegistrationsController : BaseApiController {

    [HttpGet]
    public IHttpActionResult Get(string local = null, DateTime? min_date = null, DateTime? max_date = null, bool show_success_records = false, int? page = 0, int? pageSize = 25) {
      int currentPage = page.Value;
      int currentPageSize = pageSize.Value;
      IEnumerable<EMemberRegistration> mbrs = EMemberRegistrationRepository.GetAll();
      if (this.TokenId.IsSuperAdmin && local != null)
        mbrs = mbrs.Where(u => u.LocalNumber == local);
      if (!this.TokenId.IsSuperAdmin)
        mbrs = mbrs.Where(u => u.LocalNumber == this.TokenId.LocalNumber);

      if (min_date != null)
        mbrs = mbrs.Where(e => e.CreatedOn >= min_date.Value);
      if (max_date != null)
        mbrs = mbrs.Where(e => e.CreatedOn <= max_date.Value);
      if (min_date == null && max_date == null) {
        if (!show_success_records)
          mbrs = mbrs.Where(e => !e.Success_Flag);
      }

      var mbrList = mbrs.OrderByDescending(u => u.CreatedOn).Skip(currentPage * currentPageSize).Take(currentPageSize).ToList();
      var mbrsCount = mbrs;
      int totalCount = mbrsCount.Count();

      IEnumerable<EMemberRegistrationModel> mbrVMs = Mapper.Map<IEnumerable<EMemberRegistration>, IEnumerable<EMemberRegistrationModel>>(mbrList);

      PaginationSet<EMemberRegistrationModel> pagedSet = new PaginationSet<EMemberRegistrationModel>() {
        Page = currentPage,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling((decimal)totalCount / currentPageSize),
        Items = mbrVMs
      };
      return Ok(pagedSet);
    }

    [HttpGet]
    public IHttpActionResult Get(string uuid) {
      if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid));
      var id = uuid;
      var mbr = EMemberRegistrationRepository.GetSingle(id);
      if (mbr == null) {
        return BadRequest("Member Not Found");
      }
      if (!this.TokenId.IsSuperAdmin && this.TokenId.LocalNumber != mbr.LocalNumber)
        return BadRequest("Insufficient permissions to view eMember Registrations of other locals.");

      var model = Mapper.Map<EMemberRegistrationModel>(mbr);
      return Ok(model);
    }

    [HttpPost]
    public async Task<IHttpActionResult> Post() {
      EMemberRegistration model = null;
      // Check if the request contains multipart/form-data.
      if (!Request.Content.IsMimeMultipartContent()) {
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
      }

      string pdfFilePath = ConfigurationManager.AppSettings["DuesCardsPath"];//System.Web.HttpContext.Current.Server.MapPath("~/DuesCardFiles");
      string root = System.Web.HttpContext.Current.Server.MapPath("~/Uploads");
      DirectoryInfo dir = new DirectoryInfo(pdfFilePath);
      if (!dir.Exists) {
        dir.Create();
      }
      dir = new DirectoryInfo(root);
      if (!dir.Exists) {
        dir.Create();
      }
      var provider = new MultipartFormDataStreamProvider(root);

      try {
        // Read the form data.
        await Request.Content.ReadAsMultipartAsync(provider);

        if (provider.FileData.Count > 1)
          return BadRequest("Cannot provide more than one file!");

        foreach (var key in provider.FormData.AllKeys) {
          foreach (var val in provider.FormData.GetValues(key)) {
            model =  JsonConvert.DeserializeObject<EMemberRegistration>(val);
            if(!string.IsNullOrEmpty(model.Gender))
                model.Gender = model.Gender.Substring(0, 1);
            break;
            //Trace.WriteLine(string.Format("{0}: {1}", key, val));
          }
          break;
        }
        if (!ModelState.IsValid) {
          return BadRequest(ModelState);
        }

        foreach (MultipartFileData fileData in provider.FileData) {
          if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName)) {
            return BadRequest("This request is not properly formatted");
          }
          string fileName = fileData.Headers.ContentDisposition.FileName;
          if (fileName.StartsWith("\"") && fileName.EndsWith("\"")) {
            fileName = fileName.Trim('"');
          }
          if (fileName.Contains(@"/") || fileName.Contains(@"\")) {
            fileName = Path.GetFileName(fileName);
          }
          var duesCardFile = Path.Combine(pdfFilePath, fileName);
          File.Move(fileData.LocalFileName, duesCardFile);
          model.Dues_Card_File_Name = fileName;
          using (var fStream = File.OpenRead(duesCardFile)) {
            byte[] contents = new byte[fStream.Length];
            fStream.Read(contents, 0, (int)fStream.Length);
            fStream.Close();
             model.Dues_Card_Image = contents;
          }
        }
      } catch (System.Exception e) {
        return BadRequest(e.Message);
      }


      EMemberRegistrationRepository.Add(model);
      DbContext.Commit();

      var viewModel = Mapper.Map<EMemberRegistrationModel>(model);

      //return Ok();
      return CreatedAtRoute("", new { id = viewModel.Id }, viewModel);
    }

 
    [HttpGet]
    [Route("~/api/Employers")]
    public IHttpActionResult GetEmployers(string local = "") {
      if (string.IsNullOrEmpty(local))
        local = this.TokenId.LocalNumber;

      var employers = TimssData.GetHouseData(local);
      return Ok(employers);
    }

    [HttpGet]
    [Route("~/api/Ethnicities")]
    public IHttpActionResult GetEthnicities() {
      var data = TimssData.GetReferenceData("ETHNICITY");
      return Ok(data);
    }

    private List<MemberData> GetMembers(string ssn) {
      List<MemberData> mbrs = new List<MemberData>();
      if (string.IsNullOrEmpty(ssn))
        return mbrs;

      mbrs = TimssData.GetMemberData(ssn);
      return (mbrs);
    }


    [HttpGet]
    [Route("~/api/Genders")]
    public IHttpActionResult GetGenders() {
      var data = TimssData.GetReferenceData("GENDER");
      return Ok(data);
    }

    [HttpGet]
    [Route("~/api/Languages")]
    public IHttpActionResult GetLanguages() {
      var data = TimssData.GetReferenceData("LANGUAGE");
      return Ok(data);
    }

    [HttpGet]
    [Route("~/api/CountryCodes")]
    public IHttpActionResult GetCountryCodes() {
      var data = TimssData.GetCountryData();
      return Ok(data);
    }
  }
}
