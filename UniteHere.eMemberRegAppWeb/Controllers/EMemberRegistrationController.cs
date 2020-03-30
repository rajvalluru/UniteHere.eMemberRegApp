using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegAppWeb;
using UniteHere.eMemberRegAppWeb.Models;
using UniteHere.eMemberRegApp.TimssDataAccess;
using System.IO;
using Microsoft.Ajax.Utilities;
using PagedList;
using System.Net.Http;

namespace UniteHere.eMemberRegAppWeb.Controllers {
  public class EMemberRegistrationController : BaseController {
    private ApplicationDbContext db = new ApplicationDbContext();

    public ActionResult GetPendingCardsReport() {
      IQueryable<EMemberRegistration> mbrs = EMemberRegistrationRepository.GetAll();
      if (!this.TokenId.IsSuperAdmin)
        mbrs = mbrs.Where(u => u.LocalNumber == this.TokenId.LocalNumber);

      mbrs = mbrs.Where(e => !e.Success_Flag);

      var mbrList =  mbrs.OrderByDescending(u => u.CreatedOn).ToList();

      IEnumerable<EMemberRegistrationViewModel> mbrVMs = Mapper.Map<IEnumerable<EMemberRegistration>, IEnumerable<EMemberRegistrationViewModel>>(mbrList);
      HelperFunctions.FindTimssId(mbrVMs);
      string fileName = "L" + LocalSettingsConfig.LocalSettings.LocalNumber + "_PendingCards" +  DateTime.UtcNow.ToString("_yyyymmdd_HHmmssfff");

      fileName = HelperFunctions.PrepareExcelFile(mbrVMs, fileName);
      return File(fileName, "text/csv", Path.GetFileName(fileName));
    }

    // GET: EMemberRegistrations
    public async Task<ActionResult> Index() {
      logger.Info("Registration List Called");
      IQueryable<EMemberRegistration> mbrs = EMemberRegistrationRepository.GetAll();
      if (!this.TokenId.IsSuperAdmin)
        mbrs = mbrs.Where(u => u.LocalNumber == this.TokenId.LocalNumber);

      mbrs = mbrs.Where(e => !e.Success_Flag);

      var mbrList = await mbrs.OrderByDescending(u => u.CreatedOn).ToListAsync();

      IEnumerable<EMemberRegistrationViewModel> mbrVMs = Mapper.Map<IEnumerable<EMemberRegistration>, IEnumerable<EMemberRegistrationViewModel>>(mbrList);
      HelperFunctions.FindTimssId(mbrVMs);

      ViewData["DuesCardsPath"] = LocalSettingsConfig.LocalSettings.DuesCardsPath;
      return View(mbrVMs);
    }

    //private void FindTimssId(IEnumerable<EMemberRegistrationViewModel> mbrList) {
    //  foreach(var mbr in mbrList) {
    //    var timssId = TimssData.GetMemberId(mbr.Ssn);
    //    mbr.IsNewMember = timssId == string.Empty ? true : false;
    //  }
    //}

    public async Task<ActionResult> History(string local = null, DateTime? min_date = null, DateTime? max_date = null, string searchFilter = null, int? page = 0, int? pageSize = 20, bool? includeDiscarded = true) {
      int currentPage = page.Value;
      int currentPageSize = pageSize.Value;
      if (currentPage == 0) {
       currentPage = 1;
      }
      ViewBag.CurrentFilter = searchFilter;
      ViewBag.MinDate = min_date;
      ViewBag.MaxDate = max_date;
      ViewBag.IncludeDiscarded = includeDiscarded;

      IQueryable<EMemberRegistration> mbrs = EMemberRegistrationRepository.GetAll();
      if (this.TokenId.IsSuperAdmin && local != null)
        mbrs = mbrs.Where(u => u.LocalNumber == local);
      if (!this.TokenId.IsSuperAdmin)
        mbrs = mbrs.Where(u => u.LocalNumber == this.TokenId.LocalNumber);

      if (min_date != null)
        mbrs = mbrs.Where(e => e.CreatedOn >= min_date.Value);
      if (max_date != null)
        mbrs = mbrs.Where(e => e.CreatedOn <= max_date.Value);
      if (searchFilter != null) {
        var values = searchFilter.Split(' ');
        if (values.Length > 1 && values[0].Trim().Length > 0 && values[1].Trim().Length > 0) {
          //Search is by Name 
          string firstPart = values[0].Trim();
          string secondPart = values[1].Trim();
          mbrs = mbrs.Where(m =>
                 (m.First_Name.StartsWith(firstPart) && m.Last_Name.StartsWith(secondPart)) ||
                 (m.Last_Name.StartsWith(firstPart) && m.First_Name.StartsWith(secondPart)));
        } else {
          mbrs = mbrs.Where(m => m.Last_Name.Contains(searchFilter) || m.First_Name.Contains(searchFilter)
                              || m.Ssn.Replace("-", "").Contains(searchFilter) || m.Member_Union_Id.Contains(searchFilter));
        }
      }

      mbrs = mbrs.Where(e => e.Success_Flag);
      if (includeDiscarded.HasValue && includeDiscarded.Value == false)
        mbrs = mbrs.Where(e => e.Processed_Flag);  // Skips the DISCARDED records

      var  mbrList = await mbrs.OrderByDescending(u => u.Processed_Date).ToListAsync();
      var  mbrVMs = Mapper.Map<IEnumerable<EMemberRegistration>, IEnumerable<EMemberRegistrationViewModel>>(mbrList);

      return View(mbrVMs.ToPagedList(currentPage, currentPageSize));
    }


    // GET: EMemberRegistrations/Details/5
    public async Task<ActionResult> Details(string id) {
      if (id == null) {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EMemberRegistration eMemberRegistration = await db.EMemberRegistration.FindAsync(id);
      if (eMemberRegistration == null) {
        return HttpNotFound();
      }
      return View(eMemberRegistration);
    }

    // GET: EMemberRegistrations/Create
    public ActionResult Create() {
      return View();
    }

    //// POST: EMemberRegistrations/Create
    // [HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> Create([Bind(Include = "Id,First_Name,Last_Name,Middle_Name,Dob,Address,Address_2,City,State,Postal_Code,Deduction_Opt_Out_Flag,Home_Phone,Mobile_Phone,Email,Employer_Union_Id,Position,Date_Of_Hire,Ssn,Sms_Opt_In_Flag,Race,Gender,Other_Gender,Ethnicity,Country,First_Language,Second_Language,Tip_Opt_In_Flag,Tip_Frequency,Tip_Contribution,LocalNumber,Validated_Flag,Validation_Status,Validation_Date,Processed_Flag,Processed_Status,Processed_Date,Success_Flag,Member_Union_Id,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,RowVersion,Dues_Card_File_Name,Dues_Card_Image")] EMemberRegistration eMemberRegistration) {
    //  if (ModelState.IsValid) {
    //    db.EMemberRegistration.Add(eMemberRegistration);
    //    await db.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //  }

    //  return View(eMemberRegistration);
    //}

    // GET: EMemberRegistrations/Edit/5
    public async Task<ActionResult> Edit(string id) {
      if (id == null) {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EMemberRegistration eMemberRegistration = await db.EMemberRegistration.FindAsync(id);
      if (eMemberRegistration == null) {
        return HttpNotFound();
      }
      logger.Info(eMemberRegistration.Last_Name+" "+ eMemberRegistration.First_Name+" EDIT clicked.");
      ValidateInTimss(eMemberRegistration);
      return View(eMemberRegistration);
    }

    private void ValidateInTimss(EMemberRegistration eMemberRegistration) {
      // Validate against TIMSS
      var validationMessage = new StringBuilder();
      bool validatedFlag = true;
      List<string> comparisonMessagesList = new List<string>();
      string comparisonMessage;
      ViewData["Show_UpdateName"] = false;
      ViewData["Show_UpdateAddress"] = false;
      ViewData["Show_UpdateHouse"] = false;
      ViewData["Show_ConfirmMessage"] = false;
      ViewData["Show_AddTip"] = false;
      if (eMemberRegistration.Last_Name.Length + (string.IsNullOrEmpty(eMemberRegistration.Middle_Name) ? 0 : eMemberRegistration.Middle_Name.Length) + eMemberRegistration.First_Name.Length > 40)
        ModelState.AddModelError("", "E303 - Name cannot be more than 40 letters!  Please contact Miki Foster to have the name shortened in the database.");
      if (eMemberRegistration.State.Length > 2)
        ModelState.AddModelError("", "E302 - State abbreviation cannot be more than 2 letters!  Please contact Miki Foster to have the State value changed in the database.");

      var mbrData = TimssData.GetMemberData(eMemberRegistration.Ssn, eMemberRegistration.Tip_Opt_In_Flag);
      if (mbrData.Count > 0) {
        if (mbrData.Count == 1) {
          //Exactly ONE match found for SSN
          var mbr = mbrData[0];
          eMemberRegistration.Member_Union_Id = mbr.MemberUnionId;
          comparisonMessage = "Member exists in Database : " + mbr.MemberUnionId + " (" + (mbr.SelfPayFlag == "Y" ? "Selfpay" : "Checkoff") + ") - Status: " + mbr.MbrStatusCode;
          comparisonMessagesList.Add(comparisonMessage);
          validationMessage.Append(comparisonMessage);
          //Store if the Member is already on Checkoff.  [Note: Deduction_Opt_Out_Flag  TRUE implies he chose "SelfPay" i.e opted out of CheckOff] 
          ViewData["MemberData"] = mbr;
          if (mbr.MbrStatusCode.Equals("MBR Missing"))
            ModelState.AddModelError("", "E301 - Member info is missing in TIMSS!  Please enter the missing member info first in TIMSS for member " + mbr.MemberUnionId);
          if (mbr.SelfPayFlag == "N" && eMemberRegistration.Deduction_Opt_Out_Flag
               && mbr.HouseUnionId == eMemberRegistration.Employer_Union_Id) {
            //Member is on CheckOff in the database, but now choses SelfPay
            ViewData["Show_ConfirmMessage"] = true;
          }

          if (!mbr.FirstName.Trim().ToLower().Equals(eMemberRegistration.First_Name.Trim().ToLower(), StringComparison.InvariantCulture)) {
            comparisonMessage = "First Name in Database is different : " + mbr.FirstName + "(" + eMemberRegistration.First_Name + ")";
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            ViewData["Show_UpdateName"] = true;
          }
          if (!mbr.LastName.Trim().ToLower().Equals(eMemberRegistration.Last_Name.Trim().ToLower(), StringComparison.InvariantCulture)) {
            comparisonMessage = "Last Name in Database is different : " + mbr.LastName + "(" + eMemberRegistration.Last_Name + ")";
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            ViewData["Show_UpdateName"] = true;
          }
          if (!mbr.HouseUnionId.Equals(eMemberRegistration.Employer_Union_Id)) {
            comparisonMessage = "Primary House in Database is different : " + mbr.HouseUnionId;
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            if (mbr.SelfPayFlag == "N" && eMemberRegistration.Deduction_Opt_Out_Flag)
              ViewData["Show_UpdateHouse"] = false;  //Already CheckOff on H1, but now he wants to be SelfPay on H2
            ViewData["Show_UpdateHouse"] = true;
          }
          //Compare Address 
          if (!mbr.Zip.StartsWith(eMemberRegistration.Postal_Code) && !eMemberRegistration.Postal_Code.StartsWith(mbr.Zip)) {
            comparisonMessage = "Postal Code in Database is different : " + mbr.Zip;
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            ViewData["Show_UpdateAddress"] = true;
          } else if (!mbr.State.ToUpper().Equals(eMemberRegistration.State.ToUpper())) {
            comparisonMessage = "State in Database is different : " + mbr.State;
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            ViewData["Show_UpdateAddress"] = true;
          } else if (!mbr.City.Trim().ToLower().Equals(eMemberRegistration.City.Trim().ToLower(), StringComparison.InvariantCulture)) {
            comparisonMessage = "City in Database is different : " + mbr.City;
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            ViewData["Show_UpdateAddress"] = true;
          } else if (!string.Concat(mbr.Address, mbr.Address_2).Trim().ToLower().Equals(string.Concat(eMemberRegistration.Address, eMemberRegistration.Address_2).Trim().ToLower(), StringComparison.InvariantCulture)) {
            comparisonMessage = "Address Lines in Database is different : " + string.Concat(mbr.Address, " ", mbr.Address_2);
            comparisonMessagesList.Add(comparisonMessage);
            validationMessage.Append("\n" + comparisonMessage);
            ViewData["Show_UpdateAddress"] = true;
          }
          // Take care of TIP signup
          if (eMemberRegistration.Tip_Opt_In_Flag) {
            if (mbr.TimssTipData == null || mbr.TimssTipData.Count == 0) {// No TIP data in TIMSS 
              //comparisonMessage = "No TIP data exists in TIMSS ";
              //comparisonMessagesList.Add(comparisonMessage);
              //validationMessage.Append("\n" + comparisonMessage);
              ViewData["Show_AddTip"] = true;
            } else {
              comparisonMessage = "Below TIP data exists in TIMSS: ";
              //comparisonMessagesList.Add(comparisonMessage);
              validationMessage.Append("Below TIP data exists in TIMSS: \n");
              ViewData["Show_AddTip"] = true;
              foreach (var tip in mbr.TimssTipData) {
                if (tip.Contains(eMemberRegistration.Employer_Union_Id))
                  ViewData["Show_AddTip"] = false; //Update Tip Amount in the same House

                comparisonMessage = tip;
                //comparisonMessagesList.Add(comparisonMessage);
                validationMessage.Append("\n" + comparisonMessage);
              }
            }
          }
        } else {
          //More than one member found in database
          validatedFlag = false;
          validationMessage.Append("More than one member already exists in Database with same SSN:\n ");
          foreach (var mbr in mbrData) {
            validationMessage.Append(" \n              " + mbr.MemberUnionId + " - " + mbr.FirstName + " " + mbr.LastName);
          }
        }
      } else {
        // No matching Member found in DB
        eMemberRegistration.Member_Union_Id = null;
      }
      eMemberRegistration.Validated_Flag = validatedFlag;
      eMemberRegistration.Validation_Status = validationMessage.ToString();
      // Populate Employer_Name
      if (eMemberRegistration.Employer_Name.IsNullOrWhiteSpace()) {
        var cachedHouseData = (IEnumerable<HouseData>)System.Web.HttpContext.Current.Application["HouseData"];
        var employerData =
          cachedHouseData.Where(h => h.HouseUnionId == eMemberRegistration.Employer_Union_Id)
            .Take(1)
            .Select(h => h.HouseName)
            .ToList();
        if (employerData.Count == 0) {
          ModelState.AddModelError("", "E304 - Invalid House " + eMemberRegistration.Employer_Union_Id + ".  Please enter Helpdesk ticket and contact Miki Foster. ");
        } else
          eMemberRegistration.Employer_Name = employerData[0];
      }
      //Set Labels for MBR_LEVEL1, MBR_LEVEL2, MBR_LEVEL3
      ViewData["MBR_LEVEL1"] = (string)System.Web.HttpContext.Current.Application["MBR_LEVEL1"];
      ViewData["MBR_LEVEL2"] = (string)System.Web.HttpContext.Current.Application["MBR_LEVEL2"];
      ViewData["MBR_LEVEL3"] = (string)System.Web.HttpContext.Current.Application["MBR_LEVEL3"];
      // Dropdown lists
      ViewBag.MbrTypeCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBR_TYPE_CODES"];
      ViewBag.Mbrlevel1Codes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBRLEVEL1_CODES"];
      ViewBag.Mbrlevel2Codes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBRLEVEL2_CODES"];
      ViewBag.Mbrlevel3Codes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBRLEVEL3_CODES"];
      ViewBag.DepartmentCodes = GetDepartmentList(eMemberRegistration.Employer_Union_Id);
      ViewBag.SectionCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["DEM_SECTION_CODES"];
      ViewBag.CraftCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["CRAFT_CODES"];
      ViewBag.FullPartTimeCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["DEMFULLPART_CODES"];
      ViewBag.DiscardReasonCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["DISCARDREASON_CODES"];
      // Set Comparison Codes to display
      ViewData["Comparison"] = comparisonMessagesList;
      //Store Model Errors so that we can store it in DB
      if (!ModelState.IsValid) {
        foreach(var val in ModelState.Values) {
          foreach (ModelError modelError in val.Errors) {
            errLogger.Error("while processing {0} {1}:\t{2} \n", eMemberRegistration.First_Name, eMemberRegistration.Last_Name, modelError.ErrorMessage);
          }          
        }
      }
    }

    private IEnumerable<SelectListItem> GetDepartmentList(string houseId) {      
      IEnumerable<ReferenceData> data = TimssData.GetDepartmentLookupData(houseId);
      List<SelectListItem> result = new List<SelectListItem>();
      result.Add(new SelectListItem() { Text = "--Select--", Value = string.Empty, Selected = true });
      foreach (var item in data) {
        result.Add(new SelectListItem() { Text = item.Description, Value = item.Code });
      }
      System.Web.HttpContext.Current.Session["DEPT_CODES"] = result;
      return result;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Position,Member_Union_Id, Mbr_Level1, Mbr_Level2, Mbr_Level3, Mbr_Type, Employer_Name, UpdateName, UpdateAddress, UpdateHouse, UpdatePhone, UpdateEmail, Department, Section, Craft, FullPartTime, Notes, TipActionCode, Validated_Flag, Validation_Status, Notes, Discard_Reason")] EMemberRegistration eMemberRegistration, string command) {
     

       //Set Labels for MBR_LEVEL1, MBR_LEVEL2, MBR_LEVEL3
       ViewData["MBR_LEVEL1"] = (string)System.Web.HttpContext.Current.Application["MBR_LEVEL1"];
        ViewData["MBR_LEVEL2"] = (string)System.Web.HttpContext.Current.Application["MBR_LEVEL2"];
        ViewData["MBR_LEVEL3"] = (string)System.Web.HttpContext.Current.Application["MBR_LEVEL3"];
        // Dropdown lists
        ViewBag.MbrTypeCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBR_TYPE_CODES"];
        ViewBag.Mbrlevel1Codes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBRLEVEL1_CODES"];
        ViewBag.Mbrlevel2Codes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBRLEVEL2_CODES"];
        ViewBag.Mbrlevel3Codes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["MBRLEVEL3_CODES"];
        ViewBag.DepartmentCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Session["DEPT_CODES"];
        ViewBag.SectionCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["DEM_SECTION_CODES"];
        ViewBag.CraftCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["CRAFT_CODES"];
        ViewBag.FullPartTimeCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["DEMFULLPART_CODES"];
        ViewBag.DiscardReasonCodes = (IEnumerable<SelectListItem>)System.Web.HttpContext.Current.Application["DISCARDREASON_CODES"];

      //        if (ModelState.IsValid) {
        EMemberRegistration dbRecord = await db.EMemberRegistration.FindAsync(eMemberRegistration.Id);
        if (dbRecord == null) {
          return HttpNotFound();
        }
        logger.Info(" \t\t\t\t\t"+ dbRecord.Last_Name + " " + dbRecord.First_Name + " record fetched from DB.");

        HandleMissingPdfFile(dbRecord);

        dbRecord.Validated_Flag = eMemberRegistration.Validated_Flag;
        dbRecord.Validation_Status = eMemberRegistration.Validation_Status;
        dbRecord.Notes = eMemberRegistration.Notes;
        dbRecord.Processed_Date = DateTime.Now;

        try {
          if (command == "Discard") {
            logger.Info(" \t\t\tDISCARD clicked.");
            dbRecord.Success_Flag = true;
            dbRecord.Processed_Flag = false;
            dbRecord.Processed_Status = "DISCARDED";
            //await db.SaveChangesAsync();
            EMemberRegistrationRepository.Edit(dbRecord);
            DbContext.Commit();
            return RedirectToAction("Index");
          } else if (command == "Hold") {
          logger.Info(" \t\t\tHOLD clicked.");
          dbRecord.Success_Flag = false;
            dbRecord.Processed_Flag = false;
            dbRecord.Processed_Status = "HOLD";
            //await db.SaveChangesAsync();
            EMemberRegistrationRepository.Edit(dbRecord);
            DbContext.Commit();
            return RedirectToAction("Index");
        } else if (command == "Confirm Manual") {
          logger.Info(" \t\t\tMANUAL clicked.");
          dbRecord.Success_Flag = true;
          dbRecord.Processed_Flag = true;
          dbRecord.Processed_Status = "MANUAL Attachment";
          if (eMemberRegistration.Member_Union_Id != null && eMemberRegistration.Member_Union_Id.Length >= 1)
            dbRecord.Processed_Status += " - " + eMemberRegistration.Member_Union_Id;
          //await db.SaveChangesAsync();
          EMemberRegistrationRepository.Edit(dbRecord);
          DbContext.Commit();
          return RedirectToAction("Index");
        } else { //Save
            logger.Info(" \t\t\tPROCESS clicked.");
            logger.Info(" \t\t\t\t\t Started processing record.");
            var localPrefix = (string)System.Web.HttpContext.Current.Application["LOCAL_PREFIX"];
            dbRecord.Member_Union_Id = eMemberRegistration.Member_Union_Id;
            dbRecord.Position = eMemberRegistration.Position;
            dbRecord.Mbr_Level1 = eMemberRegistration.Mbr_Level1;
            dbRecord.Mbr_Level2 = eMemberRegistration.Mbr_Level2;
            dbRecord.Mbr_Level3 = eMemberRegistration.Mbr_Level3;
            dbRecord.Mbr_Type = eMemberRegistration.Mbr_Type;
            dbRecord.Employer_Name = eMemberRegistration.Employer_Name;
            dbRecord.UpdateAddress = eMemberRegistration.UpdateAddress;
            dbRecord.UpdateName = eMemberRegistration.UpdateName;
            dbRecord.UpdateHouse = eMemberRegistration.UpdateHouse;
            dbRecord.Department = eMemberRegistration.Department;
            dbRecord.Section = eMemberRegistration.Section;
            dbRecord.Craft = eMemberRegistration.Craft;
            dbRecord.FullPartTime = eMemberRegistration.FullPartTime;
            dbRecord.Notes = eMemberRegistration.Notes;
            dbRecord.TipActionCode = eMemberRegistration.TipActionCode;
            dbRecord.Processed_Status = string.Empty;
             // Save the user entered values for later issue debugging purposes
             db.Entry(dbRecord).State = EntityState.Modified;
            EMemberRegistrationRepository.Edit(dbRecord);
            DbContext.Commit();
            logger.Info(" \t\t\t\t\t\t Current user selections saved in DB.");

            string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, dbRecord.Dues_Card_File_Name);
              string duesCardPath = (string)System.Web.HttpContext.Current.Application["DUES_CARD_IMAGE_LOCATION"];
              var duesCardFile = Path.Combine(duesCardPath, dbRecord.Ssn.Replace("-", "") + ".pdf");

              if (eMemberRegistration.Member_Union_Id != null && eMemberRegistration.Member_Union_Id.Length >= 1) {
               // Existing Member
               logger.Info(" \t\t\t\t\t\t Existing Member(" + dbRecord.Member_Union_Id + ")  - Going to update in TIMSS");
                // Validate TIP Action choice
                if (dbRecord.Tip_Opt_In_Flag && string.IsNullOrEmpty(eMemberRegistration.TipActionCode)) {
                  eMemberRegistration.Validation_Status = "Please chose appropriate action for TIP data!";
                  logger.Info(" \t\t\t\t\t TIP Action is not selected.");
                  ModelState.AddModelError("", "TIP Action is not selected.");
                  return View(eMemberRegistration);
                }
                TimssData.UpdateMember(dbRecord, duesCardFile);
                dbRecord.Processed_Status = "Existing Member(" + dbRecord.Member_Union_Id + ")  - Updated DuesCard";
                if (dbRecord.Tip_Opt_In_Flag && dbRecord.Tip_Contribution > 0 && (dbRecord.TipActionCode.ToUpper().Equals("ADD") || dbRecord.TipActionCode.ToUpper().Equals("UPDATE")))
                  dbRecord.Processed_Status += " and TIP fields";
                else
                  dbRecord.Processed_Status += ", but  no TIP info is uploaded";
                dbRecord.Processed_Status += "!";
                dbRecord.Processed_Flag = true;
                dbRecord.Processed_Date = DateTime.Now;
          } else {
            //New Member
                logger.Info(" \t\t\t\t\t\t New Member - Going to create member in TIMSS");

                bool isValidData = true;
                if (string.IsNullOrEmpty(eMemberRegistration.Mbr_Type)) {
                  ModelState.AddModelError("",  "Member Type should be selected.");
                  isValidData = false;
                }
                if (string.IsNullOrEmpty(eMemberRegistration.Mbr_Level1)) {
                  ModelState.AddModelError("", (string)System.Web.HttpContext.Current.Application["MBR_LEVEL1"] + " should be selected.");
                  isValidData = false;
                }
                if (string.IsNullOrEmpty(eMemberRegistration.Mbr_Level2)) {
                  ModelState.AddModelError("", (string)System.Web.HttpContext.Current.Application["MBR_LEVEL2"] + " should be selected.");
                  isValidData = false;
                }
                if (string.IsNullOrEmpty(eMemberRegistration.Mbr_Level3)) {
                  ModelState.AddModelError("", (string)System.Web.HttpContext.Current.Application["MBR_LEVEL3"] + " should be selected.");
                  isValidData = false;
                }
                if (isValidData) {
                  dbRecord.Member_Union_Id = TimssData.CreateNewMember(dbRecord, localPrefix, duesCardFile);
                  dbRecord.Processed_Status = "New Member(" + dbRecord.Member_Union_Id + ") ";
                  dbRecord.Processed_Flag = true;
                  dbRecord.Processed_Date = DateTime.Now;
              } else {
                return View(eMemberRegistration);
            }
          }

            db.Entry(dbRecord).State = EntityState.Modified;
            if (dbRecord.Member_Union_Id != null && dbRecord.Member_Union_Id.Length >= 1) {
              logger.Info(" \t\t\t\t\t\t Success in TIMSS");
              dbRecord.Success_Flag = true;
              EMemberRegistrationRepository.Edit(dbRecord);
              DbContext.Commit();
              //Move the Pdf file to SUCCESSFUL Location
              DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/DuesCardFiles_Success"));
              if (!dir.Exists) {
                dir.Create();
              }
              System.IO.File.Move(pdfFilePath, Path.Combine(Server.MapPath("~/DuesCardFiles_Success"), dbRecord.Dues_Card_File_Name));
              return RedirectToAction("Index");
            } else {
              logger.Info(" \t\t\t\t\t\t Could not process due to validation errors!");
            //eMemberRegistration.Validation_Status = "Could not process the record.  Please contact Miki Foster.";
            //dbRecord.Processed_Status = "Could not process the record.  Please contact Miki Foster.";
            //await db.SaveChangesAsync();
            //EMemberRegistrationRepository.Edit(dbRecord);
            //DbContext.Commit();
          }
          }
      } catch(Exception ex) {
        logger.Error("\t\t Exception: {0}\t\t\t{1} \n", ex.Message, ex.StackTrace);
        //throw (ex);
        string errMsg = string.Empty;
        if (ex.Message.Contains("Error while moving old dues card file")) {
          errMsg = "E404 - Mapped drive where Dues Cards are stored is not accessible.  Check permissions on " + LocalSettingsConfig.LocalSettings.TimssDuesCardsPath;
          //eMemberRegistration.Validation_Status = errMsg + ".  Please contact Miki Foster.";
          dbRecord.Processed_Status = errMsg + ".  Please contact Miki Foster.";
          errLogger.Error("while processing {0} {1}:\t{2} \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else if (ex.Message.Contains("Missing Membership info in TIMSS")) {
          errMsg = "E301 - Member info is missing in TIMSS!  Please enter the missing member info first in TIMSS for member " + dbRecord.Member_Union_Id;
          //eMemberRegistration.Validation_Status = "Member info is missing in TIMSS!  Please enter the missing member info first in TIMSS.";
          dbRecord.Processed_Status = errMsg;
          errLogger.Error("while processing {0} {1}:\t{2} \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else if (ex.Message.Contains("Error while updating Member Record")) {
          errMsg = "E504 - Unknown error while updating the member " + dbRecord.Member_Union_Id+" in TIMSS!  Please enter Helpdesk ticket. ";
          //eMemberRegistration.Validation_Status = "Member info is missing in TIMSS!  Please enter the missing member info first in TIMSS.";
          dbRecord.Processed_Status = errMsg;
          errLogger.Error("while processing {0} {1}:\t{2} \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else if (ex.Message.Contains("ORA-")) {
         errMsg = "E500 - Oracle error  " +ex.Message.Substring(ex.Message.IndexOf("ORA-"), ex.Message.IndexOf("at OracleInternal") - ex.Message.IndexOf("ORA-"));
          errMsg = errMsg.Replace("\"", "");
          //eMemberRegistration.Validation_Status = errMsg + ".  Please contact Miki Foster.";
          dbRecord.Processed_Status = errMsg + ".  Please enter Helpdesk ticket and contact Miki Foster.";
          errLogger.Error("while processing {0} {1}:\t{2}.  Contact development team. \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else if (ex.Message.Contains("Could not update Dues Card file in the database")) {
          errMsg = "E501 - Unknown error while updating Dues Card file in the database for member " + dbRecord.Member_Union_Id;
          //eMemberRegistration.Validation_Status = errMsg + ".  Please contact Miki Foster.";
          dbRecord.Processed_Status = errMsg + ".  Please enter Helpdesk ticket and contact Miki Foster.";
          errLogger.Error("while processing {0} {1}:\t{2}.  Contact development team. \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else if (ex.Message.Contains("Error while copying old dues card file")) {
          errMsg = "E502 - Unknown error while  copying old dues card file";
          //eMemberRegistration.Validation_Status = errMsg + ".  Please contact Miki Foster.";
          dbRecord.Processed_Status = errMsg + ".  Please enter Helpdesk ticket and contact Miki Foster.";
          errLogger.Error("while processing {0} {1}:\t{2}.  Contact development team. \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else if (ex.Message.Contains("Error while moving old dues card file")) {
          errMsg = "E503 - Unknown error while  moving old dues card file";
          //eMemberRegistration.Validation_Status = errMsg + ".  Please contact Miki Foster.";
          dbRecord.Processed_Status = errMsg + ".  Please enter Helpdesk ticket and contact Miki Foster.";
          errLogger.Error("while processing {0} {1}:\t{2}.  Contact development team. \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        } else {
          //eMemberRegistration.Validation_Status = "E505 - Unknown Error:  Could not process the record due to unknown error.  Please contact Miki Foster.";
          dbRecord.Processed_Status = "E505 - Unknown Error:  Could not process the record due to unknown error.  Please enter Helpdesk ticket and contact Miki Foster.";
          errMsg = "E505 - Unknown Error:  " + ex.Message;
          errLogger.Error("while processing {0} {1}:\t{2}.  Contact development team. \n", dbRecord.First_Name, dbRecord.Last_Name, errMsg);
        }
        //await db.SaveChangesAsync();
        dbRecord.Processed_Flag = false;
        dbRecord.Success_Flag = false;
        EMemberRegistrationRepository.Edit(dbRecord);
        DbContext.Commit();
      }
       eMemberRegistration = dbRecord;
      eMemberRegistration.Member_Union_Id = null;
      eMemberRegistration.Validated_Flag = true;
      eMemberRegistration.Validation_Status = dbRecord.Processed_Status;
      if (!string.IsNullOrEmpty(eMemberRegistration.Validation_Status)) {
        ModelState.AddModelError("", eMemberRegistration.Validation_Status);
        //Store Model Errors so that we can store it in DB
        errLogger.Error("while processing {0} {1}:\t{2} \n", eMemberRegistration.First_Name, eMemberRegistration.Last_Name, eMemberRegistration.Validation_Status);
      }
      //ValidateInTimss(eMemberRegistration);
      // Set Comparison Codes to display
      ViewData["Error"] = eMemberRegistration.Validation_Status;
      return View(eMemberRegistration);
      //      }
    }

    private void HandleMissingPdfFile(EMemberRegistration mbrRegistrationRecord) {
      string filePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, mbrRegistrationRecord.Dues_Card_File_Name);
      if (!System.IO.File.Exists(filePath)) {
        byte[] bytes = mbrRegistrationRecord.Dues_Card_Image;
        System.IO.File.WriteAllBytes(filePath, bytes);
      }
    }

    //// GET: EMemberRegistrations/Delete/5
    //public async Task<ActionResult> Delete(string id) {
    //  if (id == null) {
    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //  }
    //  EMemberRegistration eMemberRegistration = await db.EMemberRegistration.FindAsync(id);
    //  if (eMemberRegistration == null) {
    //    return HttpNotFound();
    //  }
    //  return View(eMemberRegistration);
    //}

    //// POST: EMemberRegistrations/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> DeleteConfirmed(string id) {
    //  EMemberRegistration eMemberRegistration = await db.EMemberRegistration.FindAsync(id);
    //  db.EMemberRegistration.Remove(eMemberRegistration);
    //  await db.SaveChangesAsync();
    //  return RedirectToAction("Index");
    //}

    public FileResult DisplayDuesCardPdf(string fileName) {
      string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, fileName);
      return DisplayPdf(pdfFilePath);
    }

    public FileResult DisplaySuccessfulDuesCardPdf(string fileName, string id = "") {
      string pdfFilePath = Path.Combine(Server.MapPath("~/DuesCardFiles_Success"), fileName);
      return DisplayPdf(pdfFilePath, id);
    }

    private FileResult DisplayPdf(string fullFilePath, string id = "") {
      FileStream fs = null;
      byte[] bytes;
      if (!System.IO.File.Exists(fullFilePath)) {
        if (!string.IsNullOrEmpty(id)) {
          EMemberRegistration eMemberRegistration =  db.EMemberRegistration.Find(id);
          if (eMemberRegistration != null) {
            bytes = eMemberRegistration.Dues_Card_Image;
            return File(bytes, "application/pdf", Path.GetFileName(fullFilePath));
          }
        }
        return null;
      }
      try {
        fs = System.IO.File.OpenRead(fullFilePath);
        bytes = new byte[fs.Length];
        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
      } finally {
        if (fs != null) {
          fs.Close();
          fs.Dispose();
        }
      }

      return File(bytes, "application/pdf", Path.GetFileName(fullFilePath));
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
