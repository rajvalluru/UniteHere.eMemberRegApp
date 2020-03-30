using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
//using System.Web.Http;
using UniteHere.eMemberRegApp.Entities;
using System.Web.Mvc;
using UniteHere.eMemberRegApp.TimssDataAccess;
using NLog;
using NLog.Targets;
using UniteHere.eMemberRegAppWeb.Models;

namespace UniteHere.eMemberRegAppWeb.Controllers
{
    public class ReportController : Controller {
        // GET: Report
        public ActionResult Index()  {
            return View();
        }
    private string outputDir = "~/Content/Report_OutPut/";
    private string reportDir = "~/Reports/";

    public ActionResult GetDataReportParams() {
      return View();
    }
    [HttpPost]
    public ActionResult GetSummaryReport(string exportformat, DateTime? from_date=null, DateTime? to_date=null) {
      List<UserParamValues> paramList = new List<UserParamValues>();
      paramList.Add(new UserParamValues() { ParameterName = "FromDate", ParameterValue = from_date });
      paramList.Add(new UserParamValues() { ParameterName = "ToDate", ParameterValue = to_date });
      paramList.Add(new UserParamValues() { ParameterName = "LocalNo", ParameterValue = LocalSettingsConfig.LocalSettings.LocalNumber });

      if (string.IsNullOrEmpty(exportformat))
        exportformat = "pdf";

      var rep =  GetReport("eMbrshipSummary", exportformat, "Summary", paramList);
      if (rep.Length > 0) {
        var contentType = GetMediaType(exportformat);
        return File(rep, contentType, Path.GetFileName(rep));
      } else {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Report could not be produced.  Contact System Administrator!");
      }
    }
    public ActionResult GetSummaryReportParams() {
      return View();
    }
    [HttpPost]
    public ActionResult GetDataReport(string exportformat="pdf", DateTime? from_date = null, DateTime? to_date = null) {
      List<UserParamValues> paramList = new List<UserParamValues>();
      paramList.Add(new UserParamValues() { ParameterName = "FromDate", ParameterValue = from_date });
      paramList.Add(new UserParamValues() { ParameterName = "ToDate", ParameterValue = to_date });
      paramList.Add(new UserParamValues() { ParameterName = "LocalNo", ParameterValue = LocalSettingsConfig.LocalSettings.LocalNumber });

      if (string.IsNullOrEmpty(exportformat))
        exportformat = "pdf";

      var rep = GetReport("eMbrshipData", exportformat, "Detail", paramList);
      if (rep.Length > 0) {
        var contentType = GetMediaType(exportformat);
        return File(rep, contentType, Path.GetFileName(rep));
      } else {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Report could not be produced.  Contact System Administrator!");
      }
    }

    public ActionResult GetBirthDayReportParams() {
      return View();
    }
    [HttpPost]
    public ActionResult GetBirthdayReport(string exportformat = "pdf", DateTime? from_date = null, DateTime? to_date = null) {
      List<UserParamValues> paramList = new List<UserParamValues>();
      paramList.Add(new UserParamValues() { ParameterName = "FromDate", ParameterValue = from_date });
      paramList.Add(new UserParamValues() { ParameterName = "ToDate", ParameterValue = to_date });
      paramList.Add(new UserParamValues() { ParameterName = "LocalNo", ParameterValue = LocalSettingsConfig.LocalSettings.LocalNumber });

      if (string.IsNullOrEmpty(exportformat))
        exportformat = "pdf";

      var rep = GetReport("eMbrBirthday", exportformat, "BirthDate", paramList);
      if (rep.Length > 0) {
        var contentType = GetMediaType(exportformat);
        return File(rep, contentType, Path.GetFileName(rep));
      } else {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Report could not be produced.  Contact System Administrator!");
      }
    }

    public ActionResult GetDiscardDataReportParams() {
      return View();
    }
    [HttpPost]
    public ActionResult GetDiscardDataReport(string exportformat = "pdf", DateTime? from_date = null, DateTime? to_date = null) {
      List<UserParamValues> paramList = new List<UserParamValues>();
      paramList.Add(new UserParamValues() { ParameterName = "FromDate", ParameterValue = from_date });
      paramList.Add(new UserParamValues() { ParameterName = "ToDate", ParameterValue = to_date });
      paramList.Add(new UserParamValues() { ParameterName = "LocalNo", ParameterValue = LocalSettingsConfig.LocalSettings.LocalNumber });

      if (string.IsNullOrEmpty(exportformat))
        exportformat = "pdf";

      var rep = GetReport("eMbrshipDiscardData", exportformat, "DiscardData", paramList);
      if (rep.Length > 0) {
        var contentType = GetMediaType(exportformat);
        return File(rep, contentType, Path.GetFileName(rep));
      } else {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Report could not be produced.  Contact System Administrator!");
      }
    }

    private string GetReport(string report_name, string format, string reportHeader, List<UserParamValues> parameters) {
      if (string.IsNullOrEmpty(format))
        format = "pdf";
      var fName = ExportReport(report_name, parameters, reportHeader, format);
      return fName;
    }

    private string GetMediaType(string format) {
      string mediaType = "application/pdf"; ;
      switch (format.ToLower()) {
        case "pdf":
          mediaType ="application/pdf";
          break;
        case "excel":
          mediaType = "application/vnd.ms-excel";
          break;
        case "csv":
          mediaType = "text/csv";
          break;
        default:
          mediaType = "application/pdf";
          break;
      }
      return mediaType;
    }

    //[HttpGet]
    //[Route("api/list/reports")]
    //public IHttpActionResult Get(int offset = 0, int limit = 25) {
    //  IEnumerable<ReportDef> rpts = ReportDefRepository.GetAll();

    //  var rptsList = rpts.Skip(offset).Take(limit);
    //  var rptsCount = rpts;
    //  int totalCount = rptsCount.Count();
    //  return Ok(new { Items = rptsList.ToList(), TotalCount = totalCount });
    //}

    [HttpGet]
    public ActionResult GetErrorLog() {
      //  Find the correct target
      var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("errorfile");

      //  Using the target, get the full path to the log file
      var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
      string fileName = Path.GetFullPath(fileTarget.FileName.Render(logEventInfo));
      if (!System.IO.File.Exists(fileName))
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No Errors occurred!");
      return File(fileName, "text/plain", Path.GetFileName(fileName));
    }

    private HttpResponseMessage SendAsAttachment(string fileName) {
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

    private string ExportReport(string reportName, List<UserParamValues> paramList, string reportHeader, string reportformat = "pdf") {
      string repName = reportName + ".rpt";
      string fileName = "L"+LocalSettingsConfig.LocalSettings.LocalNumber+"_"+ reportHeader + DateTime.UtcNow.ToString("_yyyymmdd_HHmmssfff");

      ReportDocument cryRpt = LoadCrReport(repName);

      SetParameters(paramList, cryRpt);

      ExcelDataOnlyFormatOptions excelFormatOpts = ExportOptions.CreateDataOnlyExcelFormatOptions(); //new ExcelFormatOptions();
      CharacterSeparatedValuesFormatOptions csvOptions = new CharacterSeparatedValuesFormatOptions();

      ExportFormatType exportFormat = ExportFormatType.PortableDocFormat;
      switch (reportformat.ToLower()) {
        case "pdf":
          exportFormat = ExportFormatType.PortableDocFormat;
          fileName = fileName + ".pdf";
          break;
        case "excel":
          cryRpt.ReportDefinition.Sections[0].SectionFormat.EnableSuppress = true;
          exportFormat = ExportFormatType.ExcelRecord;
          excelFormatOpts.ExcelUseConstantColumnWidth = false;
          excelFormatOpts.SimplifyPageHeaders = true;
          excelFormatOpts.ExportPageHeaderAndPageFooter = false;
          excelFormatOpts.ShowGroupOutlines = false;
          fileName = fileName + ".xls";
          cryRpt.ExportOptions.FormatOptions = excelFormatOpts;

          break;
        case "csv":
          exportFormat = ExportFormatType.CharacterSeparatedValues;
          csvOptions.SeparatorText = ",";
          csvOptions.Delimiter = "\n";
          csvOptions.ReportSectionsOption = CsvExportSectionsOption.ExportIsolated;
          csvOptions.GroupSectionsOption = CsvExportSectionsOption.DoNotExport;
          csvOptions.ExportMode = CsvExportMode.Standard;

          fileName = fileName + ".csv";
          cryRpt.ExportOptions.FormatOptions = csvOptions;

          break;
        default:
          exportFormat = ExportFormatType.PortableDocFormat;
          fileName = fileName + ".pdf";
          break;
      }

      //Check if OutputDir exists
      var physicalPath = Server.MapPath(outputDir);
      if (!Directory.Exists(physicalPath))
        Directory.CreateDirectory(physicalPath);
      string filePath = Server.MapPath(outputDir + fileName);
      cryRpt.ExportToDisk(exportFormat, filePath);
      return filePath;
    }

    private static void SetParameters(List<UserParamValues> paramList, ReportDocument cryRpt) {
      ParameterFieldDefinitions crParameterFieldDefinitions = null;
      ParameterFieldDefinition crParameterFieldDefinition;
      ParameterValues crParameterValues = new ParameterValues();
      ParameterDiscreteValue crParameterDiscreteValue = new ParameterDiscreteValue();

      crParameterFieldDefinitions = cryRpt.DataDefinition.ParameterFields;

      foreach (var param in paramList) {
        crParameterDiscreteValue.Value = param.ParameterValue;
        crParameterFieldDefinition = crParameterFieldDefinitions[param.ParameterName];
        crParameterValues = crParameterFieldDefinition.CurrentValues;
        crParameterValues.Clear();
        if (!string.IsNullOrEmpty(Convert.ToString(param.ParameterValue))) {
          switch (crParameterFieldDefinition.ValueType) {
            case FieldValueType.NumberField:
              crParameterDiscreteValue.Value = Convert.ToDouble(param.ParameterValue);
              break;
            case FieldValueType.CurrencyField:
              crParameterDiscreteValue.Value = Convert.ToDecimal(param.ParameterValue);
              break;
            case FieldValueType.DateField:
              crParameterDiscreteValue.Value = Convert.ToDateTime(param.ParameterValue).Date;
              break;
            case FieldValueType.DateTimeField:
              crParameterDiscreteValue.Value = Convert.ToDateTime(param.ParameterValue);
              break;
            case FieldValueType.BooleanField:
              crParameterDiscreteValue.Value = Convert.ToBoolean(param.ParameterValue);
              break;
            default:
              crParameterDiscreteValue.Value = Convert.ToString(param.ParameterValue);
              break;
          }
        } else {
          switch (crParameterFieldDefinition.ValueType) {
            case FieldValueType.NumberField:
              crParameterDiscreteValue.Value = Convert.ToDouble(0);
              break;
            case FieldValueType.CurrencyField:
              crParameterDiscreteValue.Value = Convert.ToDecimal(0);
              break;
            case FieldValueType.DateField:
            case FieldValueType.DateTimeField:
              if (param.ParameterName.ToLower().Contains("date") &&
                  (param.ParameterName.ToLower().StartsWith("from") || param.ParameterName.ToLower().StartsWith("start") ||
                   param.ParameterName.ToLower().StartsWith("begin")))
                param.ParameterValue = DateTime.MinValue;
              if (param.ParameterName.ToLower().Contains("date") &&
                 (param.ParameterName.ToLower().StartsWith("to") || param.ParameterName.ToLower().StartsWith("end")))
                param.ParameterValue = DateTime.MaxValue;
              crParameterDiscreteValue.Value = Convert.ToDateTime(param.ParameterValue).Date;
              break;
            case FieldValueType.BooleanField:
              crParameterDiscreteValue.Value = true;
              break;
            default:
              crParameterDiscreteValue.Value = "";
              break;
          }
        }
        crParameterValues.Add(crParameterDiscreteValue);
        crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);
      }
    }

    private ReportDocument LoadCrReport(string repName) {
      ReportDocument cryRpt = new ReportDocument();
      try {
        string repPath = Server.MapPath(reportDir + repName);

        if (!System.IO.File.Exists(repPath))
          throw new FileNotFoundException("Report " + repName + " is missing or invalid.");
        cryRpt.Load(Server.MapPath(reportDir + repName));

        ConnectionInfo crConnectionInfo = new ConnectionInfo();

        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(connString);

        crConnectionInfo.DatabaseName = connStringBuilder.InitialCatalog;
        crConnectionInfo.UserID = connStringBuilder.UserID;
        crConnectionInfo.Password = connStringBuilder.Password;
        crConnectionInfo.ServerName = connStringBuilder.DataSource;
        crConnectionInfo.IntegratedSecurity = connStringBuilder.IntegratedSecurity;

        foreach (CrystalDecisions.CrystalReports.Engine.Table table in cryRpt.Database.Tables) {
          TableLogOnInfo tLogOnInfo = table.LogOnInfo;
          tLogOnInfo.ConnectionInfo = crConnectionInfo;
          table.ApplyLogOnInfo(tLogOnInfo);
        }

        return cryRpt;
      } catch (Exception ex) {
        string innerExceptionMessage = ex.InnerException == null ? "" : " - " + ex.InnerException.Message;
        throw (new Exception("Report Path : " + (reportDir + repName) + " -- Exception - " + ex.Message + innerExceptionMessage));
      }
    }
  }
}
