using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.TimssDataAccess;
using UniteHere.eMemberRegAppWeb.Models;

namespace UniteHere.eMemberRegAppWeb.Controllers {
  public class HelperFunctions {
    private static string outputDir = "~/Content/Report_OutPut/";
    private static string reportDir = "~/Reports/";

    public static string PrepareExcelFile(IEnumerable<EMemberRegistrationViewModel> data, string repName) {
      string fileName = repName + ".csv";
      string filePath = HttpContext.Current.Server.MapPath(outputDir + fileName);
      var dataList = data.ToList();
      using (StreamWriter file = new System.IO.StreamWriter(filePath)) {
        if (dataList.Count == 0)
          return filePath;
        var firstLine = dataList[0];
        file.WriteLine(firstLine.Header);
        foreach (var line in dataList) {
          file.WriteLine(line.Stringify);
        }
      }
      return filePath;
    }

    public static void FindTimssId(IEnumerable<EMemberRegistrationViewModel> mbrList) {
      foreach (var mbr in mbrList) {
        var timssId = TimssData.GetMemberId(mbr.Ssn);
        mbr.IsNewMember = timssId == string.Empty ? true : false;
      }
    }

    public static HttpResponseMessage SendAsAttachment(string fileName) {
      if (!System.IO.File.Exists(fileName)) {
        var failedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
          Content = new StringContent("Report could not be generated!"),
          ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
        };
        return failedResponse;
      }

      HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
      Byte[] bytes = System.IO.File.ReadAllBytes(fileName);
      response.Content = new ByteArrayContent(bytes);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
      response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(fileName);

      return response;
    }

  }
}