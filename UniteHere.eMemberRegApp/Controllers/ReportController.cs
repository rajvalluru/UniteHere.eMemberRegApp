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
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegApp.Controllers {
  public class ReportController : BaseApiController {
    private string outputDir = "~/Content/Report_OutPut/";
    private string reportDir = "~/Reports/CrystalReports/";

    [HttpPost]
    [Route("api/reports/{report_name}")]
    public HttpResponseMessage GetReport(string report_name, [FromUri] string format, List<UserParamValues> parameters) {
      if (string.IsNullOrEmpty(report_name)) {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
          Content = new StringContent("ReportName cannot be blank!"),
          ReasonPhrase = "Please provide Report name."
        };
        return response;
      }
      if (string.IsNullOrEmpty(format))
        format = "pdf";
      var fName = ExportReport(report_name, parameters, format);
      return SendAsAttachment(fName);
    }

    [HttpGet]
    [Route("api/list/reports")]
    public IHttpActionResult Get(int offset = 0, int limit = 25) {
      IEnumerable<ReportDef> rpts = ReportDefRepository.GetAll();

      var rptsList = rpts.Skip(offset).Take(limit);
      var rptsCount = rpts;
      int totalCount = rptsCount.Count();
      return Ok(new { Items = rptsList.ToList(), TotalCount = totalCount });
    }


    private HttpResponseMessage SendAsAttachment(string fileName) {
      if (!File.Exists(fileName)) {
        var failedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
          Content = new StringContent("Report could not be generated!"),
          ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
        };
        return failedResponse;
      }

      HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
      Byte[] bytes = File.ReadAllBytes(fileName);
      response.Content = new ByteArrayContent(bytes);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
      response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(fileName);

      return response;
    }

    private string ExportReport(string reportName, List<UserParamValues> paramList, string reportformat = "pdf") {
      string repName = reportName + ".rpt";
      string fileName = reportName + DateTime.UtcNow.ToString("_yyyymmdd_HHmmssfff");

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
          exportFormat = ExportFormatType.ExcelRecord;
          excelFormatOpts.ExcelUseConstantColumnWidth = true;
          excelFormatOpts.SimplifyPageHeaders = false;
          excelFormatOpts.ExportPageHeaderAndPageFooter = true;
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
      string filePath = HttpContext.Current.Server.MapPath(outputDir + fileName);
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
        string repPath = HttpContext.Current.Server.MapPath(reportDir + repName);

        if (!File.Exists(repPath))
          throw new FileNotFoundException("Report " + repName + " is missing or invalid.");
        cryRpt.Load(HttpContext.Current.Server.MapPath(reportDir + repName));

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
      } catch (Exception ex)
      {
        string innerExceptionMessage = ex.InnerException==null ? "" : " - " + ex.InnerException.Message;
        throw (new Exception("Report Path : " + (reportDir + repName) + " -- Exception - " + ex.Message + innerExceptionMessage));
      }
    }
  }
}
