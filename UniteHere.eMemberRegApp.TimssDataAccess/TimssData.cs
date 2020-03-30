using NLog;
//using Oracle.DataAccess.Client;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegApp.TimssDataAccess {
  public static class TimssData {
    private static Logger logger = LogManager.GetCurrentClassLogger();

    public static IEnumerable<HouseData> GetHouseData(string local_number) {
      List<HouseData> houseData = new List<HouseData>();
      string qry = "Select c.customer, c.name, NVL(RIGHT_TO_WORK_HOUSE_FLAG, 'N') Right_To_Work from Customer c Where c.Record_Type = 'C'  "
                   + " And c.Customer_Status in ('ACTIVE', 'Active') "
                   + " order by c.name ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          var hd = new HouseData { LocalNumber = local_number, HouseUnionId = odr[0].ToString(), HouseName = odr[1].ToString(), DisplayName = odr[1].ToString(), IsRightToWork = odr[2].ToString() == "Y" ? true : false };
          houseData.Add(hd);
        }
      }

      return houseData;
    }

    public static string GetMemberId(string ssn) {
      string timssId = string.Empty;
      List<MemberData> mbrData = new List<MemberData>();
      string qry = "Select c.CUSTOMER MemberId "
                   + "  from Customer c "
                   + " Where  c.Record_Type = 'I'  "
                   + " And c.SSN LIKE '" + ssn.Substring(0, 3) + "%' "
                   + " And ( c.ssn = '" + ssn + "' OR REPLACE(c.ssn, '-', '') = '" + ssn.Replace("-", "") + "' )"
                   + " order by 1 ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          try {
            timssId = odr["MemberId"].ToString();
            return timssId;
          } catch {
            throw (new Exception(odr["MemberId"].ToString() + " record in source database has wrong data - Please check!"));
          }
        }
      }
      return timssId;
    }

    public static List<MemberData> GetMemberData(string ssn, bool tipOptInFlag = false) {
      List<MemberData> mbrData = new List<MemberData>();
      // Raj Valluru 07/25/2017  Using LEFT OUTER JOIN to handle situations where Membership record is missing, but Customer record exists
      string qry = "Select c.CUSTOMER MemberId, c.LAST_NAME LastName, c.MIDDLE_NAME MiddleName, c.FIRST_NAME FirstName "
                   + ", c.SSN Ssn "
                   + ", c.Address_1, c.Address_2, c.City, c.State, c.Zip"
                   + ", c.CO_CUSTOMER, c.CO_NAME "
                   + ", NVL(m.MBRSTATUS_CODE, 'MBR Missing') MBRSTATUS_CODE, m.CHECKOFF_SELFPAY_FLAG "
                   + "  from Customer c LEFT OUTER JOIN Membership m ON c.Customer = m.Customer "
                   + " Where  c.Record_Type = 'I'  "
                   + " And c.SSN LIKE '" + ssn.Substring(0, 3) + "%' "
                   + " And ( c.ssn = '" + ssn + "' OR REPLACE(c.ssn, '-', '') = '" + ssn.Replace("-","") + "' )"
                   + " order by c.LAST_NAME ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          try {
            var mbr = new MemberData() {
              MemberUnionId = odr["MemberId"].ToString()
                                         , LastName = odr["LastName"].ToString(), MiddleName = odr["MiddleName"].ToString(), FirstName = odr["FirstName"].ToString()
                                         , Ssn = odr["Ssn"].ToString()
                              , Address = odr["Address_1"].ToString(), Address_2 = odr["Address_2"].ToString(), City = odr["City"].ToString(), State = odr["State"].ToString(), Zip = odr["Zip"].ToString()
                              , HouseUnionId = odr["CO_CUSTOMER"].ToString(), HouseName = odr["CO_NAME"].ToString()
                              , MbrStatusCode = odr["MBRSTATUS_CODE"].ToString(), SelfPayFlag = odr["CHECKOFF_SELFPAY_FLAG"].ToString()
            };
            mbrData.Add(mbr);
          } catch {
            throw (new Exception(odr["MemberId"].ToString() + " record in source database has wrong data - Please check!"));
          }
        }
      }
      // There is only one member
      if(mbrData.Count == 1) {
        // Check if Dues Card exists
        var mbr = mbrData[0];
        var duesCardFileName = GetDuesCardFileName(mbr.MemberUnionId);
        mbr.DuesCardExists = (duesCardFileName != string.Empty) ;

        // Get existing TIP data from TIMSS
        if (tipOptInFlag) {
          var tips = GetTipData(mbr.MemberUnionId);
          mbr.TimssTipData = tips;
        } else
          mbr.TimssTipData = new List<string>();
      }

      return mbrData;
    }

    private static MemberData GetMemberDataById(string id) {
      string qry = "Select c.CUSTOMER MemberId, c.LAST_NAME LastName, c.MIDDLE_NAME MiddleName, c.FIRST_NAME FirstName "
                   + ", c.SSN Ssn "
                   + ", c.Address_1, c.Address_2, c.City, c.State, c.Zip"
                   + ", c.CO_CUSTOMER, c.CO_NAME "
                   + ", m.MBRSTATUS_CODE, m.CHECKOFF_SELFPAY_FLAG "
                   + "  from Customer c, Membership m "
                   + " Where c.Customer = m.Customer And c.Record_Type = 'I'  "
                   + " And c.Customer = '" + id + "' "
                   + " order by c.LAST_NAME ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          try {
            var mbr = new MemberData() {
              MemberUnionId = odr["MemberId"].ToString()
                                         , LastName = odr["LastName"].ToString(), MiddleName = odr["MiddleName"].ToString(), FirstName = odr["FirstName"].ToString()
                                         , Ssn = odr["Ssn"].ToString()
                              , Address = odr["Address_1"].ToString(), Address_2 = odr["Address_2"].ToString(), City = odr["City"].ToString(), State = odr["State"].ToString(), Zip = odr["Zip"].ToString()
                              , HouseUnionId = odr["CO_CUSTOMER"].ToString(), HouseName = odr["CO_NAME"].ToString()
                              , MbrStatusCode = odr["MBRSTATUS_CODE"].ToString(), SelfPayFlag = odr["CHECKOFF_SELFPAY_FLAG"].ToString()
            };
            return (mbr);
          } catch {
            throw (new Exception(odr["MemberId"].ToString() + " record in source database has wrong data - Please check!"));
          }
        }
      }
      return null;
    }

    private static List<string> GetTipData(string id) {
      List<string> tips = new List<string>();
      // Raj Valluru 07/25/2017  Using LEFT OUTER JOIN to handle situations where Membership record is missing, but Customer record exists
      string qry = "Select c.CUSTOMER ||' - '|| c.NAME ||' ( $'|| emp."
                   + LocalSettingsConfig.LocalSettings.TipAmountColumn
                   + " ||'  since '|| TO_CHAR( emp."
                   + LocalSettingsConfig.LocalSettings.TipStartDateColumn
                   + ", 'mm/dd/yyyy') ||') ' as data "
                   + "  from CUS_EMPLOYMENT emp INNER JOIN CUSTOMER c ON emp.EMPLOYER_CUSTOMER = c.CUSTOMER"
                   + " Where  emp.Customer = '" + id + "' "
                   + " And emp.END_DATE IS NULL "
                   + " And emp."+ LocalSettingsConfig.LocalSettings.TipAmountColumn + " > 0 "
                   + " order by  emp." + LocalSettingsConfig.LocalSettings.TipStartDateColumn + " DESC ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          try {
            var tipData = odr["data"].ToString();
            tips.Add(tipData);
          } catch {
            throw (new Exception(id + " tip data could not be fetched - Please check!"));
          }
        }
      }

      return tips;
    }

    public static IEnumerable<ReferenceData> GetReferenceData(string refType) {
      List<ReferenceData> refData = new List<ReferenceData>();
      string qry = "Select c.CODE, c.SDESCR from Code c Where c.TYPE = '" + refType + "' "
                   + " and c.ACTIVE = 'Y' "
                   + " Order  by c.SDESCR ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          var hd = new ReferenceData() { Code = odr[0].ToString(), Description = odr[1].ToString() };
          refData.Add(hd);
        }
      }

      return refData;
    }

    public static IEnumerable<ReferenceData> GetCountryData() {
      List<ReferenceData> refData = new List<ReferenceData>();
      string qry = "Select COUNTRY_CODE, COUNTRY FROM COUNTRY_CODE"
                   + " Order  by COUNTRY_CODE";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          var hd = new ReferenceData() { Code = odr[0].ToString(), Description = odr[1].ToString() };
          refData.Add(hd);
        }
      }

      return refData;
    }

    public static IEnumerable<ReferenceData> GetDepartmentLookupData(string houseId) {
      List<ReferenceData> refData = new List<ReferenceData>();
      string qry = "Select c.CODE, c.SDESCR from CUS_DEPARTMENT CD, Code c Where c.TYPE = 'CUSDEPT' "
                   + " and c.ACTIVE = 'Y' "
                   + " AND  C.CODE = CD.CUSDEPT_CODE AND  CD.CUSTOMER = '"+ houseId + "' "
                   + " Order  by c.SDESCR ";

      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          var hd = new ReferenceData() { Code = odr[0].ToString(), Description = odr[1].ToString() };
          refData.Add(hd);
        }
      }

      return refData;
    }

    public static string GetDuesCardFileName(string mbrId) {
      string fName = string.Empty;
      string qry = "Select FILE_NAME from DUES_CARD_MASTER c Where c.CUSTOMER = '" + mbrId + "' ";

      fName = ExecuteScalar(qry);

      return fName;
    }
    public static Dictionary<string, string> LoadRamsParameterData() {
      Dictionary<string, string> result = new Dictionary<string, string>();

      string qry = "Select PARAMETER_NAME, PARAMETER_VALUE from RAMS_PARAMETER WHERE " +
                    "PARAMETER_NAME in ('LOCAL_PREFIX', 'MBR_LEVEL1', 'MBR_LEVEL2', 'MBR_LEVEL3', 'DUES_CARD_IMAGE_LOCATION')";
      using (var odr = GetDataReader(qry)) {
        while (odr.Read()) {
          result.Add(odr[0].ToString(), odr[1].ToString());
        }
      }

      return result;
    }

    private static bool MoveDuesCardFileToHistory(string duesCardFileName, string houseID, string ssn) {
      bool result = true;
      //Existing Member - Move Old Card to History Location
      logger.Info("\t\t Move Duescard File to History :  {0}\t{1}\t{2}\n", duesCardFileName, houseID, ssn);
      try {
        if (!string.IsNullOrWhiteSpace(duesCardFileName) && System.IO.File.Exists(duesCardFileName)) {
          string fName = ssn.Replace("-","") + "_" + houseID + "_" +
                         DateTime.Now.ToString("yyyy-MM-dd") + Path.GetExtension(duesCardFileName);
          if (!string.IsNullOrWhiteSpace(fName)) {
            var duesCardHistoryPath = LocalSettingsConfig.LocalSettings.DuesCardsHistoryPath;
            var duesCardHistoryFileName = Path.Combine(duesCardHistoryPath, fName);
            if (!System.IO.File.Exists(duesCardFileName)) {
              logger.Info("\t\t\t\t Duescard File does not exist :  {0}\n", duesCardFileName);
              return true;
            }
            logger.Info("\t\t\t\t Duescard File Moved to : {0}\n", duesCardHistoryFileName);
            System.IO.File.Move(duesCardFileName, duesCardHistoryFileName);
          }
        }
      } catch (Exception ex) {
        result = false;
        throw new Exception("Error while moving old dues card file " + ex.ToString());
      }

      return result;
    }

    private static bool CopyDuesCardFileToHistory(string duesCardFileName, string houseID, string ssn) {
      bool result = true;
      //Existing Member - Move Old Card to History Location
      logger.Info("\t\t Copy Duescard File to History :  {0}\t{1}\t{2}\n", duesCardFileName, houseID, ssn);
      try {
        if (!string.IsNullOrWhiteSpace(duesCardFileName) && System.IO.File.Exists(duesCardFileName)) {
          string fName = ssn.Replace("-", "") + "_" + houseID + "_" +
                         DateTime.Now.ToString("yyyy-MM-dd") + Path.GetExtension(duesCardFileName);
          if (!string.IsNullOrWhiteSpace(fName)) {
            var duesCardHistoryPath = LocalSettingsConfig.LocalSettings.DuesCardsHistoryPath;
            var duesCardHistoryFileName = Path.Combine(duesCardHistoryPath, fName);
            if (!System.IO.File.Exists(duesCardFileName)) {
              logger.Info("\t\t\t\t Duescard File does not exist :  {0}\n", duesCardFileName);
              return true;
            }
            logger.Info("\t\t\t\t Duescard File Copied to : {0}\n", duesCardHistoryFileName);
            System.IO.File.Copy(duesCardFileName, duesCardHistoryFileName, true);
          }
        }
      } catch (Exception ex) {
        result = false;
        throw new Exception("Error while copying old dues card file " + ex.ToString());
      }

      return result;
    }

    private static bool CopyFile(string fromFilePath, string toFilePath) {
      bool result = true;
      try {
        System.IO.File.Copy(fromFilePath, toFilePath);
      } catch (Exception ex) {
        result = false;
        throw new Exception("Error while moving old dues card file " + ex.ToString());
      }

      return result;
    }
    private static bool MoveFile(string fromFilePath, string toFilePath) {
      bool result = true;
      try {
        logger.Info("\t\t Move Duescard File  from :  {0}\tTo:{1}\n", fromFilePath, toFilePath);
        System.IO.File.Move(fromFilePath, toFilePath);
      } catch (Exception ex) {
        result = false;
        throw new Exception("Error while moving old dues card file " + ex.ToString());
      }

      return result;
    }

    private static bool StoreNewPdfFile(string pdfFileName, string newDuesCardFileName) {
      bool result = true;
      //Copy the new Pdf File to TIMSS DuesCard Location
      string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, pdfFileName);
      logger.Info("\t\t Copy Duescard File  from :  {0}\tTo:{1}\n", pdfFileName, newDuesCardFileName);
      if (!CopyFile(pdfFilePath, newDuesCardFileName))
        return false;

      return result;
    }

    private static MemberData GetMemberDataById(OracleCommand cmd, string id) {
      string qry = "Select c.CUSTOMER MemberId, c.LAST_NAME LastName, c.MIDDLE_NAME MiddleName, c.FIRST_NAME FirstName "
                   + ", c.SSN Ssn "
                   + ", c.Address_1, c.Address_2, c.City, c.State, c.Zip"
                   + ", c.CO_CUSTOMER, c.CO_NAME "
                   + ", m.MBRSTATUS_CODE, m.CHECKOFF_SELFPAY_FLAG "
                   + "  from Customer c, Membership m "
                   + " Where c.Customer = m.Customer And c.Record_Type = 'I'  "
                   + " And c.Customer = '" + id + "' "
                   + " order by c.LAST_NAME ";
      cmd.CommandText = qry;
      using (var odr = cmd.ExecuteReader()) {
        while (odr.Read()) {
          try {
            var mbr = new MemberData() {
              MemberUnionId = odr["MemberId"].ToString()
                                         , LastName = odr["LastName"].ToString(), MiddleName = odr["MiddleName"].ToString(), FirstName = odr["FirstName"].ToString()
                                         , Ssn = odr["Ssn"].ToString()
                              , Address = odr["Address_1"].ToString(), Address_2 = odr["Address_2"].ToString(), City = odr["City"].ToString(), State = odr["State"].ToString(), Zip = odr["Zip"].ToString()
                              , HouseUnionId = odr["CO_CUSTOMER"].ToString(), HouseName = odr["CO_NAME"].ToString()
                              , MbrStatusCode = odr["MBRSTATUS_CODE"].ToString(), SelfPayFlag = odr["CHECKOFF_SELFPAY_FLAG"].ToString()
            };
            return (mbr);
          } catch {
            throw (new Exception(odr["MemberId"].ToString() + " record in source database has wrong data - Please check!"));
          }
        }
      }
      return null;
    }

    public static string CreateNewMember(EMemberRegistration mbr, string localPrefix, string fileName) {
      logger.Info("\t CreateNewMember :   {0}\t {1}\t {2}\t {3}\n", mbr.Last_Name, mbr.First_Name, localPrefix, fileName);
      using (OracleConnection timssDbConn = new OracleConnection(ConnectString)) {
        timssDbConn.Open();
        OracleTransaction transaction;
        transaction = timssDbConn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        using (OracleCommand cmd = new OracleCommand()) {
          cmd.Transaction = transaction;
          cmd.Connection = timssDbConn;
          string newMbrId = localPrefix + GetNewCustomerSeq(cmd);
          try {
            if (InsertIntoCustomer(cmd, mbr, newMbrId)) {
              InsertIntoPhone(cmd, newMbrId, "CELL", mbr.Mobile_Phone, true);
              InsertIntoPhone(cmd, newMbrId, "HOME", mbr.Home_Phone);
              InsertIntoPhone(cmd, newMbrId, "EMAIL", mbr.Email);
              InsertIntoPhone(cmd, newMbrId, "OFFICE", mbr.Work_Phone);
              mbr.Member_Union_Id = newMbrId;
              AddToEmploymentTable(cmd, mbr); // Should add Employment to ensure proper begin date before we add MbrShip record
              InsertIntoMembership(cmd, mbr, newMbrId);
              InsertIntoDuesCardMasterTable(cmd, newMbrId, fileName);
              if (mbr.Tip_Opt_In_Flag) {
                DateTime tipSignedDate = mbr.TIP_Card_Signed_Date ?? mbr.CreatedOn.Date;
                UpdateTipData(cmd, mbr.Member_Union_Id, mbr.Employer_Union_Id, mbr.Tip_Contribution, tipSignedDate);
              }
              if (LocalSettingsConfig.LocalSettings.SmsOptIn.TrackingAs.ToUpper() == "PHONE" && mbr.Sms_Opt_In_Flag) {
                InsertIntoPhone(cmd, newMbrId, LocalSettingsConfig.LocalSettings.SmsOptIn.PhoneType, mbr.Mobile_Phone);
              }
              if (!string.IsNullOrEmpty(mbr.Notes)) {
                InsertIntoComments(cmd, newMbrId, mbr.Notes);
              }
              AddBeneficiary(cmd, mbr.Member_Union_Id, mbr.Beneficiary);

              StoreNewPdfFile(mbr.Dues_Card_File_Name, fileName);
            }
            transaction.Commit();
            logger.Info("\t\t CreateNewMember Completed! \n");
            return newMbrId;
          } catch (Exception e) {
            transaction.Rollback();
            logger.Error("\t\t Exception: {0}\t\t\t{1} \n", e.Message, e.StackTrace);

            throw new Exception("Could not create new Member Record for " + mbr.First_Name + " " + mbr.Last_Name + ". " + e.ToString());
          }
        }
      }
    }

    public static bool UpdateMember(EMemberRegistration mbr, string newDuesCardFileName) {
      bool success = true;
      //MemberData timssMemberData = GetMemberDataById(mbr.Member_Union_Id);
      logger.Info("\t UpdateMember :  {0}\t {1}\t {2}\t  {3}\n", mbr.Member_Union_Id, mbr.Last_Name, mbr.First_Name, newDuesCardFileName);
      using (OracleConnection timssDbConn = new OracleConnection(ConnectString)) {
        timssDbConn.Open();
        //OracleTransaction transaction;
        using (OracleTransaction transaction = timssDbConn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) {
          using (OracleCommand cmd = new OracleCommand()) {
            cmd.Transaction = transaction;
            cmd.Connection = timssDbConn;
            try {
              MemberData timssMemberData = GetMemberDataById(cmd, mbr.Member_Union_Id);
              if( timssMemberData == null)
                throw new Exception("Missing Membership info in TIMSS for member  " + mbr.Member_Union_Id );
              UpdateCustomerTable(cmd, mbr);
              
              if (mbr.Employer_Union_Id == timssMemberData.HouseUnionId) {
                // Same House - Simply update.
                logger.Info("\t\t\t Same House - ");
                UpdateHouse(cmd, mbr);
                if (!UpdateDuesCard(mbr, newDuesCardFileName, timssMemberData.HouseUnionId, cmd))
                  throw new Exception("Could not update Dues Card file in the database!");
                StoreNewPdfFile(mbr.Dues_Card_File_Name, newDuesCardFileName);
              } else {
                // Different House
                if (mbr.UpdateHouse) {
                  // Different House - UpdateHouse = true
                  DiffHouseUpdateHouse(cmd, mbr, newDuesCardFileName, timssMemberData);
                } else {
                  // Different House - UpdateHouse = false
                  DiffHouseNoUpdateHouse(cmd, mbr, timssMemberData);
                }
              }

              if (mbr.Tip_Opt_In_Flag && mbr.Tip_Contribution > 0 &&
                     (mbr.TipActionCode.ToUpper().Equals("ADD") || mbr.TipActionCode.ToUpper().Equals("UPDATE"))) {
                DateTime tipSignedDate = mbr.TIP_Card_Signed_Date ?? mbr.CreatedOn.Date;
                UpdateTipData(cmd, mbr.Member_Union_Id, mbr.Employer_Union_Id, mbr.Tip_Contribution, tipSignedDate);
              }
              UpdatePhone(cmd, mbr.Member_Union_Id, "CELL", mbr.Mobile_Phone);
              UpdatePhone(cmd, mbr.Member_Union_Id, "HOME", mbr.Home_Phone);
              UpdatePhone(cmd, mbr.Member_Union_Id, "EMAIL", mbr.Email);
              UpdatePhone(cmd, mbr.Member_Union_Id, "OFFICE", mbr.Work_Phone);
              if (LocalSettingsConfig.LocalSettings.SmsOptIn.TrackingAs.ToUpper() == "PHONE" && mbr.Sms_Opt_In_Flag)
                UpdatePhone(cmd, mbr.Member_Union_Id, LocalSettingsConfig.LocalSettings.SmsOptIn.PhoneType, mbr.Mobile_Phone);
              AddBeneficiary(cmd, mbr.Member_Union_Id, mbr.Beneficiary);

              transaction.Commit();
              logger.Info("\t\t UpdateMember Completed! \n");
            } catch (Exception e) {
              success = false;
              transaction.Rollback();
              logger.Error("\t\t Exception: {0}\t\t\t{1} \n", e.Message, e.StackTrace);

              throw new Exception("Error while updating Member Record for " + mbr.Member_Union_Id + " : " + e.ToString());
            }
          }
        }
      }
      return success;
    }

    private static void DiffHouseNoUpdateHouse(OracleCommand cmd, EMemberRegistration mbr, MemberData timssMemberData) {
      #region Different House - UpdateHouse = false
      logger.Info("\t\t\t Different House - UpdateHouse = false \n");
      if (!mbr.Deduction_Opt_Out_Flag) {
        // Selected CheckOff on the new House
        #region Selected CheckOff on the new House
        if (timssMemberData.SelfPayFlag == "N") {
          // Already Checkoff in TIMSS 
          logger.Info("\t\t\t Selected CheckOff on the new House - Already Checkoff in TIMSS \n");
          AddToEmploymentTable(cmd, mbr);
          AddToMultipleBillings(cmd, mbr);
          string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, mbr.Dues_Card_File_Name);
          if (!CopyDuesCardFileToHistory(pdfFilePath, mbr.Employer_Union_Id, mbr.Ssn))
            throw new Exception("Could not copy Dues Card file to History folder!");
        } else {
          // SelfPay in TIMSS
          logger.Info("\t\t\t Selected CheckOff on the new House - SelfPay in TIMSS \n");
          //Need to update Checkoff without changing Primary House
          UpdateCheckOff(cmd, mbr);
          AddToEmploymentTable(cmd, mbr);
          AddToMultipleBillings(cmd, mbr);
          string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, mbr.Dues_Card_File_Name);
          if (!CopyDuesCardFileToHistory(pdfFilePath, mbr.Employer_Union_Id, mbr.Ssn))
            throw new Exception("Could not copy Dues Card file to History folder!");
        }
        #endregion
      } else {
        // Selected SelfPay on the new House
        #region Selected SelfPay on the new House
        if (timssMemberData.SelfPayFlag == "N") {
          // Already Checkoff in TIMSS 
          AddToEmploymentTable(cmd, mbr);
          string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, mbr.Dues_Card_File_Name);
          if (!CopyDuesCardFileToHistory(pdfFilePath, mbr.Employer_Union_Id, mbr.Ssn))
            throw new Exception("Could not copy Dues Card file to History folder!!");
        } else {
          // SelfPay in TIMSS
          AddToEmploymentTable(cmd, mbr);
          string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, mbr.Dues_Card_File_Name);
          if (!CopyDuesCardFileToHistory(pdfFilePath, mbr.Employer_Union_Id, mbr.Ssn))
            throw new Exception("Could not move Dues Card file to History folder!!");
        }
        #endregion
      }
      #endregion
    }

    private static void DiffHouseUpdateHouse(OracleCommand cmd, EMemberRegistration mbr, string newDuesCardFileName, MemberData timssMemberData) {
      #region Different House - UpdateHouse = true
      logger.Info("\t\t\t Different House - UpdateHouse = true \n");
      if ((timssMemberData.SelfPayFlag == "N" && !mbr.Deduction_Opt_Out_Flag)
         || (timssMemberData.SelfPayFlag == "Y")) {
        UpdatePrimaryHouse(cmd, mbr, newDuesCardFileName, timssMemberData);
      } else {
        // Already Checkoff in TIMSS & selected SelfPay on the new House 
        logger.Info("\t\t\t Already Checkoff in TIMSS & selected SelfPay on the new House  \n");
        AddToEmploymentTable(cmd, mbr);
        string pdfFilePath = Path.Combine(LocalSettingsConfig.LocalSettings.DuesCardsPath, mbr.Dues_Card_File_Name);
        if (!CopyDuesCardFileToHistory(pdfFilePath, mbr.Employer_Union_Id, mbr.Ssn))
          throw new Exception("Could not move Dues Card file to History folder!!");
      }
      #endregion
    }

    private static void UpdatePrimaryHouse(OracleCommand cmd, EMemberRegistration mbr, string newDuesCardFileName, MemberData timssMemberData) {
      // selected CheckOff on the new House OR SelfPay in TIMSS
      logger.Info("\t\t\t selected CheckOff on the new House OR SelfPay in TIMSS \n");
      if (!UpdateDuesCard(mbr, newDuesCardFileName, timssMemberData.HouseUnionId, cmd))
        throw new Exception("Could not update Dues Card file in the database!");
      DateTime dtEndDate = mbr.Date_Of_Hire ?? mbr.CreatedOn;
      UpdateEmployment(cmd, mbr.Member_Union_Id, timssMemberData.HouseUnionId, dtEndDate, true);
      logger.Info("\t\t\t Update old Employment record for House {0} \n", timssMemberData.HouseUnionId);
      AddToEmploymentTable(cmd, mbr, true);
      logger.Info("\t\t\t Inserted into CUS_Employment table for House {0} \n", mbr.Employer_Union_Id);
      UpdateHouse(cmd, mbr);
      logger.Info("\t\t\t Updated Primary House in Membership table \n");
      StoreNewPdfFile(mbr.Dues_Card_File_Name, newDuesCardFileName);
      logger.Info("\t\t\t Copy the DuesCard to TIMSS \n");
    }

    private static bool UpdateDuesCard(EMemberRegistration mbr, string newDuesCardFileName, string oldHouseID, OracleCommand cmd) {
      string oldDuesCardFileName = GetDuesCardFileName(mbr.Member_Union_Id);
      if (!string.IsNullOrWhiteSpace(oldDuesCardFileName)) {
        // Record exists in DUES_CARD_MASTER table
        string oldDuesCardFileNameNormalized = Path.Combine(LocalSettingsConfig.LocalSettings.TimssDuesCardsPath, Path.GetFileName(oldDuesCardFileName));
        if (!MoveDuesCardFileToHistory(oldDuesCardFileNameNormalized, oldHouseID, mbr.Ssn))
          return false;
        UpdateDuesCardMasterTable(cmd, mbr.Member_Union_Id, newDuesCardFileName);
      } else {
        // We noticed that files do exist with SSN name even though there is no record in DUES_CARD_MASTER table
        var duesCardHistoryPath = LocalSettingsConfig.LocalSettings.TimssDuesCardsPath;
        var duesCardHistoryFileName = Path.Combine(duesCardHistoryPath, mbr.Ssn.Replace("-",""));
        string oldDuesCardFileNameNormalized = Path.Combine(LocalSettingsConfig.LocalSettings.TimssDuesCardsPath, mbr.Ssn.Replace("-", "")+".pdf");
        if (!MoveDuesCardFileToHistory(oldDuesCardFileNameNormalized, oldHouseID, mbr.Ssn))
          return false;
        InsertIntoDuesCardMasterTable(cmd, mbr.Member_Union_Id, newDuesCardFileName);
      }
      return true;
    }

    private static void UpdatePhone(OracleCommand cmd, string mbrId, string phoneType, string phone) {
      if (string.IsNullOrEmpty(phone))  // Nothing to do if phone is missing
        return;
      StringBuilder sql = new StringBuilder();
      sql.Append("MERGE INTO PHONE p USING ");
      sql.Append(" (SELECT :customer CUSTOMER, :phoneType PHONE_TYPE, :phone PHONE FROM DUAL) d ");
      sql.Append(" ON (p.CUSTOMER = d.CUSTOMER AND p.PHONE_TYPE = d.PHONE_TYPE) ");
      sql.Append(" WHEN NOT MATCHED THEN INSERT(CUSTOMER, PHONE_TYPE, PHONE, ADDOPER, ADDDATE, ROWVERSION) ");
      sql.Append("   VALUES(d.CUSTOMER, d.PHONE_TYPE, d.PHONE, 'eMbrReg', SYSDATE, 0) ");
      sql.Append(" WHEN MATCHED THEN UPDATE SET PHONE = d.PHONE, MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1 ");
      sql.Append("                   WHERE p.PHONE != d.PHONE");

      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", mbrId));
      parameters.Add(new OracleParameter("phoneType", phoneType));
      parameters.Add(new OracleParameter("phone", phone));
      ExecuteNonQuery(cmd, sql.ToString(), parameters);
    }

    private static void AddBeneficiary(OracleCommand cmd, string mbrId, string beneficiaryName) {
      if (string.IsNullOrEmpty(beneficiaryName))  // Nothing to do if beneficiaryName is missing
        return;
      StringBuilder sql = new StringBuilder();
      sql.Append("MERGE INTO CUS_CONTACT p USING ");
      sql.Append(" (SELECT :customer CUSTOMER, 'BENEFICIARY' CONTACT_CLASS, :beneficiaryName MAIL_NAME FROM DUAL) d ");
      sql.Append(" ON (p.CUSTOMER = d.CUSTOMER AND p.CONTACT_CLASS = d.CONTACT_CLASS) ");
      sql.Append(" WHEN NOT MATCHED THEN INSERT(CONTACT_NO, CUSTOMER, CONTACT_CLASS, MAIL_NAME, ADDOPER, ADDDATE, ROWVERSION) ");
      sql.Append("   VALUES(SEQ_CONTACT.NEXTVAL, d.CUSTOMER, d.CONTACT_CLASS, d.MAIL_NAME, 'eMbrReg', SYSDATE, 0) ");
      sql.Append(" WHEN MATCHED THEN UPDATE SET MAIL_NAME = d.MAIL_NAME, MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1 ");
      sql.Append("                   WHERE UPPER(p.MAIL_NAME) != UPPER(d.MAIL_NAME) ");

      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", mbrId));
      parameters.Add(new OracleParameter("beneficiaryName", beneficiaryName));
      ExecuteNonQuery(cmd, sql.ToString(), parameters);
    }

    private static void AddToEmploymentTable(OracleCommand cmd, EMemberRegistration mbr, bool primaryHouse = false) {
      StringBuilder existsSql = new StringBuilder();
      existsSql.Append("SELECT '1' FROM CUS_EMPLOYMENT WHERE CUSTOMER= :customer AND EMPLOYER_CUSTOMER = :house ");
      existsSql.Append(" AND END_DATE IS NULL");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", mbr.Member_Union_Id));
      parameters.Add(new OracleParameter("house", mbr.Employer_Union_Id));

      string existsEmpRecord = string.Empty;
      existsEmpRecord = ExecuteScalar(cmd, existsSql.ToString(), parameters);
      if (!string.IsNullOrEmpty(existsEmpRecord))
        return; //Employment record already exists

      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT INTO CUS_EMPLOYMENT (EMPLOYMENT_ID, CUSTOMER, EMPLOYER_CUSTOMER, START_DATE, PRIMARY_FLAG ");
      insertSql.Append(", HEALTH_INSURANCE_FLAG, POLITICAL_DEDUCT_FLAG ");
      insertSql.Append(", CUSDEPT_CODE, DEMSECTION_CODE, CRAFT_CODE, DEMFULLPART_CODE ");
      insertSql.Append(", OFF_MONDAY_FLAG, OFF_TUESDAY_FLAG,  OFF_WEDNESDAY_FLAG, OFF_THURSDAY_FLAG ");
      insertSql.Append(", OFF_FRIDAY_FLAG, OFF_SATURDAY_FLAG,  OFF_SUNDAY_FLAG ");
      insertSql.Append(", ADDOPER, ADDDATE, ROWVERSION) ");
      insertSql.Append(" VALUES( SEQ_EMPLOYMENT_ID.NEXTVAL, :customer, :house, :emp_date, :primary ");
      insertSql.Append(" , 'N', :tip_deduction_flag ");
      insertSql.Append(", :cusdept_code, :demsection_code, :craft_code, :demfullpart_code ");
      insertSql.Append(" , 'N', 'N', 'N', 'N' ");
      insertSql.Append(" , 'N', 'N', 'N' ");
      insertSql.Append(", 'eMbrReg', SYSDATE, 0 ) ");

      parameters.Add(new OracleParameter("emp_date", mbr.Date_Of_Hire ?? mbr.CreatedOn));
      parameters.Add(new OracleParameter("primary", primaryHouse ? "Y" : "N"));
      parameters.Add(new OracleParameter("tip_deduction_flag", mbr.Tip_Opt_In_Flag ? "Y" : "N"));
      parameters.Add(new OracleParameter("cusdept_code", mbr.Department));
      parameters.Add(new OracleParameter("demsection_code", mbr.Section));
      parameters.Add(new OracleParameter("craft_code", mbr.Craft));
      parameters.Add(new OracleParameter("demfullpart_code", mbr.FullPartTime));

      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Inserted into CUS_Employment table for House {0} \n", mbr.Employer_Union_Id);
    }

    private static void AddToMultipleBillings(OracleCommand cmd, EMemberRegistration mbr) {
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT INTO MEMBERSHIP_BILLING (CUSTOMER, HOUSE_CUSTOMER, ADDOPER, ADDDATE, ROWVERSION) ");
      insertSql.Append(" SELECT :customer, :house, 'eMbrReg', SYSDATE, 0 FROM DUAL ");
      insertSql.Append(" WHERE NOT EXISTS (SELECT 1 FROM MEMBERSHIP_BILLING ");
      insertSql.Append("  WHERE CUSTOMER= :customer2 AND HOUSE_CUSTOMER = :house2 )");

      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", mbr.Member_Union_Id));
      parameters.Add(new OracleParameter("house", mbr.Employer_Union_Id));
      parameters.Add(new OracleParameter("customer2", mbr.Member_Union_Id));
      parameters.Add(new OracleParameter("house2", mbr.Employer_Union_Id));
      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Inserted into Membership_Billing table for House {0} \n", mbr.Employer_Union_Id);
    }
    private static void DeleteFromMultipleBillings(OracleCommand cmd, EMemberRegistration mbr) {
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("DELETE FROM MEMBERSHIP_BILLING  ");
      insertSql.Append(" WHERE CUSTOMER= :customer AND HOUSE_CUSTOMER = :house ");

      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", mbr.Member_Union_Id));
      parameters.Add(new OracleParameter("house", mbr.Employer_Union_Id));
      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
    }

    private static void UpdateCheckOff(OracleCommand cmd, EMemberRegistration mbr) {
      StringBuilder updSql = new StringBuilder();
      updSql.Append("UPDATE MEMBERSHIP SET CHECKOFF_SELFPAY_FLAG = :checkoff");
      updSql.Append(", MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1");
      updSql.Append(" WHERE CUSTOMER = :customer");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("checkoff", mbr.Deduction_Opt_Out_Flag ? "Y" : "N"));
      parameters.Add(new OracleParameter("customer", mbr.Member_Union_Id));
      ExecuteNonQuery(cmd, updSql.ToString(), parameters);
      logger.Info("\t\t\t Update CheckOff in Membership table\n");
      //Since Triggers create Multiple Billings record, we need to delete it
      DeleteFromMultipleBillings(cmd, mbr);
    }

    private static void UpdateHouse(OracleCommand cmd, EMemberRegistration mbr) {
      StringBuilder updSql = new StringBuilder();
      updSql.Append("UPDATE MEMBERSHIP SET HOUSE_CUSTOMER = :house, CHECKOFF_SELFPAY_FLAG = :checkoff");
      updSql.Append(", DUES_CARD_DATE = :duescard_date, DUES_CARD_FLAG = 'Y' ");
      updSql.Append(", MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1");
      //updSql.Append(" WHERE CUSTOMER = '"+mbr.Member_Union_Id+"' ");
      updSql.Append(" WHERE CUSTOMER = :customer");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("house", mbr.Employer_Union_Id));
      parameters.Add(new OracleParameter("checkoff", mbr.Deduction_Opt_Out_Flag ? "Y" : "N"));
      parameters.Add(new OracleParameter("duescard_date", mbr.CreatedOn.Date));
      parameters.Add(new OracleParameter("customer", mbr.Member_Union_Id));
      logger.Info("\t\t\t Update Primary House in Membership table \n");
      ExecuteNonQuery(cmd, updSql.ToString(), parameters);
    }

    private static void UpdateEmployment(OracleCommand cmd, string mbrId, string houseId, DateTime date, bool updateEndDate) {
      StringBuilder updSql = new StringBuilder();
      updSql.Append("UPDATE CUS_EMPLOYMENT  SET ");
      if (updateEndDate)
        updSql.Append(" END_DATE = :emp_date, PRIMARY_FLAG = 'N' ");
      else
        updSql.Append(" START_DATE = :emp_date, PRIMARY_FLAG = 'Y' ");
      updSql.Append(", MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1");
      updSql.Append(" WHERE CUSTOMER = :customer and EMPLOYER_CUSTOMER  = :house ");
      updSql.Append(" and END_DATE IS NULL ");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("emp_date", date));
      parameters.Add(new OracleParameter("customer", mbrId));
      parameters.Add(new OracleParameter("house", houseId));
      ExecuteNonQuery(cmd, updSql.ToString(), parameters);
    }

    private static bool UpdateCustomerTable(OracleCommand cmd, EMemberRegistration mbr) {
      bool success = true;
      bool updRequired = false;
      StringBuilder updSql = new StringBuilder();
      var countryCode = mbr.Country == "United States" ? "USA" : "CAN";

      List<OracleParameter> parameters = new List<OracleParameter>();

      updSql.Append("UPDATE CUSTOMER SET  ");
      if (mbr.UpdateName) {
        updSql.Append(" LAST_NAME = :lastName, MIDDLE_NAME = :middleName, FIRST_NAME = :firstName");
        parameters.Add(new OracleParameter("lastName", mbr.Last_Name));
        parameters.Add(new OracleParameter("middleName", mbr.Middle_Name));
        parameters.Add(new OracleParameter("firstName", mbr.First_Name));

        updRequired = true;
      }
      if (mbr.UpdateAddress) {
        if (updRequired)
          updSql.Append(", ");

        updSql.Append(" ADDRESS_1 = :address, ADDRESS_2 = :address_2, CITY = :city");
        updSql.Append(", STATE = :state, ZIP = :postal_code, COUNTRY_CODE = :country");
        updSql.Append(", ADDR_STATUS = 'G', ADDR_STATUS_DATE = SYSDATE, ADDR_CHNGDATE = SYSDATE");
        parameters.Add(new OracleParameter("address", mbr.Address));
        parameters.Add(new OracleParameter("address_2", mbr.Address_2));
        parameters.Add(new OracleParameter("city", mbr.City));
        parameters.Add(new OracleParameter("state", mbr.State));
        parameters.Add(new OracleParameter("postal_code", mbr.Postal_Code));
        parameters.Add(new OracleParameter("country", countryCode));

        updRequired = true;
      }
      { // MBR_CARD_SIGNED_DATE
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" MBR_CARD_SIGNED_DATE = :mbr_card_signed_date ");
        parameters.Add(new OracleParameter("mbr_card_signed_date", mbr.Dues_Card_Signed_Date ?? mbr.CreatedOn.Date));
        updRequired = true;
      }
      if (mbr.Dob != null && !mbr.Dob.ToString("MMddyyyy").Equals("01010001")) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" BIRTHDATE = :dob ");
        parameters.Add(new OracleParameter("dob", mbr.Dob));
        updRequired = true;
      }
      if (!string.IsNullOrEmpty(mbr.Gender)) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" GENDER = :gender ");
        parameters.Add(new OracleParameter("gender", mbr.Gender.Substring(0,1)));
        updRequired = true;
      }
      if (!string.IsNullOrEmpty(mbr.Country_Of_Origin)) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" BIRTH_COUNTRY = :birthCountry ");
        parameters.Add(new OracleParameter("birthCountry", mbr.Country_Of_Origin));
        updRequired = true;
      }
      if (!string.IsNullOrEmpty(mbr.Ethnicity)) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" ETHNICITY = :ethnicity ");
        parameters.Add(new OracleParameter("ethnicity", mbr.Ethnicity));
        updRequired = true;
      }
      if (!string.IsNullOrEmpty(mbr.First_Language)) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" LANGUAGE_CODE = :language_code ");
        parameters.Add(new OracleParameter("language_code", mbr.First_Language));
        updRequired = true;
      }
      if (!string.IsNullOrEmpty(mbr.Second_Language) && !string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.SecongLanguageColumn)) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" " + LocalSettingsConfig.LocalSettings.SecongLanguageColumn + " = :second_language ");
        parameters.Add(new OracleParameter("second_language", mbr.Second_Language));
        updRequired = true;
      }
      if (!string.IsNullOrEmpty(mbr.Race) && !string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.RaceColumn)) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" " + LocalSettingsConfig.LocalSettings.RaceColumn + " = :race ");
        parameters.Add(new OracleParameter("race", mbr.Race));
        updRequired = true;
      }
      if (mbr.Sms_Opt_In_Flag && LocalSettingsConfig.LocalSettings.SmsOptIn.TrackingAs.ToUpper() == "CUSTOMER") {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" " + LocalSettingsConfig.LocalSettings.SmsOptIn.UserFlag + " = :sms_opt_in ");
        parameters.Add(new OracleParameter("sms_opt_in", mbr.Sms_Opt_In_Flag ? "Y" : "N"));
        updRequired = true;
      }
      if (LocalSettingsConfig.LocalSettings.StudentFlag.TableName.ToUpper() == "CUSTOMER"  && LocalSettingsConfig.LocalSettings.StudentFlag.UserFlag.Length>5) {
        if (updRequired)
          updSql.Append(", ");
        updSql.Append(" " + LocalSettingsConfig.LocalSettings.StudentFlag.UserFlag + " = :studentFlag ");
        parameters.Add(new OracleParameter("studentFlag", mbr.Student_Flag ? "Y" : "N"));
        updRequired = true;
      }
      if (!updRequired)
        return true; // Nothing to update

      updSql.Append(", MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1");
      updSql.Append(" WHERE CUSTOMER = :customer");
      parameters.Add(new OracleParameter("customer", mbr.Member_Union_Id));
      ExecuteNonQuery(cmd, updSql.ToString(), parameters);
      logger.Info("\t\t\t Update Customer table \n");
      return success;
    }

    private static bool InsertIntoCustomer(OracleCommand cmd, EMemberRegistration mbr, string newMbrId) {
      bool success = true;
      string customerClass = LocalSettingsConfig.LocalSettings.CustomerClass;
      string customerStatus = LocalSettingsConfig.LocalSettings.CustomerStatus;
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT  INTO CUSTOMER( ");
      insertSql.Append(" CUSTOMER, SSN, NAME, MAIL_NAME, SEARCH_NAME, FIRST_NAME, LAST_NAME, MIDDLE_NAME, CUSTOMER_CLASS, CUSTOMER_STATUS");
      insertSql.Append(", RECORD_TYPE");
      insertSql.Append(", CO_CUSTOMER, CO_NAME, CO_MAIL_NAME, CO_SEARCH_NAME");
      insertSql.Append(", ADDRESS_1, ADDRESS_2, CITY, STATE, ZIP, COUNTRY_CODE, COUNTRY, ADDR_TYPE, ADDR_STATUS");
      insertSql.Append(", JOIN_DATE, BIRTHDATE");
      if (!string.IsNullOrEmpty(mbr.Gender)) {
        insertSql.Append(", GENDER");
      }
      if (!string.IsNullOrEmpty(mbr.Country_Of_Origin)) {
        insertSql.Append(", BIRTH_COUNTRY");
      }
      insertSql.Append(", ETHNICITY, LANGUAGE_CODE, MBR_CARD_SIGNED_DATE");
      if (!string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.SecongLanguageColumn))
        insertSql.Append(", " + LocalSettingsConfig.LocalSettings.SecongLanguageColumn);
      if (!string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.RaceColumn))
        insertSql.Append(", " + LocalSettingsConfig.LocalSettings.RaceColumn);
      if (LocalSettingsConfig.LocalSettings.SmsOptIn.TrackingAs.ToUpper() == "CUSTOMER")
        insertSql.Append(", " + LocalSettingsConfig.LocalSettings.SmsOptIn.UserFlag);
      if (LocalSettingsConfig.LocalSettings.StudentFlag.TableName.ToUpper() == "CUSTOMER")
        insertSql.Append(", " + LocalSettingsConfig.LocalSettings.StudentFlag.UserFlag);
      insertSql.Append(", ADDOPER, ADDDATE, ROWVERSION");
      insertSql.Append(" ) VALUES ( ");
      insertSql.Append(" :customer, :ssn, :name, :mail_name, :search_name, :first_name, :last_name, :middle_name, :customer_class, :customer_status");
      insertSql.Append(", :record_type");
      insertSql.Append(", :co_customer, :co_name, :co_mail_name, :co_search_name");
      insertSql.Append(", :address_1, :address_2, :city, :state, :zip, :country_code, :country, :addr_type, :addr_status");
      insertSql.Append(", :join_date, :birthdate");
      if (!string.IsNullOrEmpty(mbr.Gender)) {
        insertSql.Append(", :gender");
      }
      if (!string.IsNullOrEmpty(mbr.Country_Of_Origin)) {
        insertSql.Append(", :birthCountry");
      }
      insertSql.Append(", :ethnicity, :language_code, :mbr_card_signed_date");
      if (!string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.SecongLanguageColumn))
        insertSql.Append(", :second_language");
      if (!string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.RaceColumn))
        insertSql.Append(", :race");
      if (LocalSettingsConfig.LocalSettings.SmsOptIn.TrackingAs.ToUpper() == "CUSTOMER")
        insertSql.Append(", :sms_opt_in_flag");
      if (LocalSettingsConfig.LocalSettings.StudentFlag.TableName.ToUpper() == "CUSTOMER" && LocalSettingsConfig.LocalSettings.StudentFlag.UserFlag.Length > 5)
        insertSql.Append(", :studentFlag");
      insertSql.Append(", 'eMbrReg', SYSDATE, 0 ");
      insertSql.Append(" )");

      var countryCode = mbr.Country == "United States" ? "USA" : "CAN";
      List<OracleParameter> parameters = new List<OracleParameter>();

      var name = mbr.Last_Name + ";" + mbr.First_Name + ";" + mbr.Middle_Name;
      parameters.Add(new OracleParameter("customer", newMbrId));
      parameters.Add(new OracleParameter("ssn", mbr.Ssn));
      parameters.Add(new OracleParameter("name", name));
      parameters.Add(new OracleParameter("mail_name", mbr.First_Name + " " + mbr.Middle_Name + " " + mbr.Last_Name));
      parameters.Add(new OracleParameter("search_name", name.ToUpper()));
      parameters.Add(new OracleParameter("first_name", mbr.First_Name));
      parameters.Add(new OracleParameter("last_name", mbr.Last_Name));
      parameters.Add(new OracleParameter("middle_name", mbr.Middle_Name));
      parameters.Add(new OracleParameter("customer_class", customerClass));
      parameters.Add(new OracleParameter("customer_status", customerStatus));
      parameters.Add(new OracleParameter("record_type", "I"));
      parameters.Add(new OracleParameter("co_customer", mbr.Employer_Union_Id));
      parameters.Add(new OracleParameter("co_name", mbr.Employer_Name));
      parameters.Add(new OracleParameter("co_mail_name", mbr.Employer_Name));
      parameters.Add(new OracleParameter("co_search_name", mbr.Employer_Name.ToUpper()));
      parameters.Add(new OracleParameter("address_1", mbr.Address));
      parameters.Add(new OracleParameter("address_2", mbr.Address_2));
      parameters.Add(new OracleParameter("city", mbr.City));
      parameters.Add(new OracleParameter("state", mbr.State));
      parameters.Add(new OracleParameter("zip", mbr.Postal_Code));
      parameters.Add(new OracleParameter("country_code", countryCode));
      parameters.Add(new OracleParameter("country", mbr.Country));
      parameters.Add(new OracleParameter("addr_type", "HOME"));
      parameters.Add(new OracleParameter("addr_status", "G"));
      parameters.Add(new OracleParameter("join_date", mbr.Date_Of_Hire ?? mbr.CreatedOn));
      if (mbr.Dob != null && !mbr.Dob.ToString("MMddyyyy").Equals("01010001"))
        parameters.Add(new OracleParameter("birthdate", mbr.Dob));
      else
        parameters.Add(new OracleParameter("birthdate", null));
      if (!string.IsNullOrEmpty(mbr.Gender)) {
        parameters.Add(new OracleParameter("gender", mbr.Gender.Substring(0, 1)));
      }
      if (!string.IsNullOrEmpty(mbr.Country_Of_Origin)) {
        parameters.Add(new OracleParameter("birthCountry", mbr.Country_Of_Origin));
      }
      parameters.Add(new OracleParameter("ethnicity", mbr.Ethnicity));
      parameters.Add(new OracleParameter("language_code", mbr.First_Language));
      parameters.Add(new OracleParameter("mbr_card_signed_date", mbr.Dues_Card_Signed_Date ?? mbr.CreatedOn.Date));
      if (!string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.SecongLanguageColumn))
        parameters.Add(new OracleParameter("second_language", mbr.Second_Language));
      if (!string.IsNullOrWhiteSpace(LocalSettingsConfig.LocalSettings.RaceColumn))
        parameters.Add(new OracleParameter("race", mbr.Race));
      if (LocalSettingsConfig.LocalSettings.SmsOptIn.TrackingAs.ToUpper() == "CUSTOMER")
        parameters.Add(new OracleParameter("sms_opt_in_flag", mbr.Sms_Opt_In_Flag ? "Y" : "N"));
      if (LocalSettingsConfig.LocalSettings.StudentFlag.TableName.ToUpper() == "CUSTOMER" && LocalSettingsConfig.LocalSettings.StudentFlag.UserFlag.Length > 5)
        parameters.Add(new OracleParameter("studentFlag", mbr.Student_Flag ? "Y" : "N"));

      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Inserted into Customer table :   {0}\n", newMbrId);
      return success;
    }

    private static bool InsertIntoMembership(OracleCommand cmd, EMemberRegistration mbr, string newMbrId) {
      bool success = true;
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT  INTO MEMBERSHIP( ");
      insertSql.Append(" MBR_LOCAL_CODE, CUSTOMER, SSN, CUSTOMER_NAME, CUSTOMER_MAIL_NAME, CUSTOMER_SEARCH_NAME");
      insertSql.Append(", MBR_TYPE_CODE, MBRSTATUS_CODE, MBRSTATUS_DATE");
      insertSql.Append(", HOUSE_CUSTOMER");
      insertSql.Append(", MBRLEVEL1_CODE, MBRLEVEL2_CODE, MBRLEVEL3_CODE");
      insertSql.Append(", CHECKOFF_SELFPAY_FLAG, DUES_CARD_DATE, DUES_CARD_FLAG");
      insertSql.Append(", ADDOPER, ADDDATE, MODOPER, MODDATE, ROWVERSION");
      insertSql.Append(" ) VALUES ( ");
      insertSql.Append(" :mbr_local_code, :customer, :ssn, :name, :mail_name, :search_name");
      insertSql.Append(", :mbr_type_code, :mbrstatus_code, SYSDATE");
      insertSql.Append(", :house_customer");
      insertSql.Append(", :mbrlevel1_code, :mbrlevel2_code, :mbrlevel3_code");
      insertSql.Append(", :checkoff_selfpay_flag, :duescard_date, :dues_card_flag");
      insertSql.Append(", USER, SYSDATE, USER, SYSDATE, 0 ");
      insertSql.Append(" )");
      List<OracleParameter> parameters = new List<OracleParameter>();
      string localNumber = (string.IsNullOrEmpty(mbr.LocalNumber) || mbr.LocalNumber == "0" || mbr.LocalNumber.Length < 4)
                                                     ? LocalSettingsConfig.LocalSettings.LocalNumber : mbr.LocalNumber;
      var name = mbr.Last_Name + ";" + mbr.First_Name + ";" + mbr.Middle_Name;
      parameters.Add(new OracleParameter("mbr_local_code", localNumber));
      parameters.Add(new OracleParameter("customer", newMbrId));
      parameters.Add(new OracleParameter("ssn", mbr.Ssn));
      parameters.Add(new OracleParameter("name", name));
      parameters.Add(new OracleParameter("mail_name", mbr.First_Name + " " + mbr.Middle_Name + " " + mbr.Last_Name));
      parameters.Add(new OracleParameter("search_name", name.ToUpper()));
      parameters.Add(new OracleParameter("mbr_type_code", mbr.Mbr_Type));
      parameters.Add(new OracleParameter("mbrstatus_code", mbr.Mbr_Type == "F" ? "N" : "P"));
      parameters.Add(new OracleParameter("house_customer", mbr.Employer_Union_Id));
      parameters.Add(new OracleParameter("mbrlevel1_code", mbr.Mbr_Level1));
      parameters.Add(new OracleParameter("mbrlevel2_code", mbr.Mbr_Level2));
      parameters.Add(new OracleParameter("mbrlevel3_code", mbr.Mbr_Level3));
      parameters.Add(new OracleParameter("checkoff_selfpay_flag", mbr.Deduction_Opt_Out_Flag ? "Y" : "N"));
      parameters.Add(new OracleParameter("duescard_date", mbr.Dues_Card_Signed_Date ?? mbr.CreatedOn.Date));
      parameters.Add(new OracleParameter("dues_card_flag", "Y"));
      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Inserted into Membership table\n");
      return success;
    }

    private static bool InsertIntoDuesCardMasterTable(OracleCommand cmd, string newMbrId, string fileName) {
      bool success = true;
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT  INTO DUES_CARD_MASTER( ");
      insertSql.Append(" CUSTOMER, FILE_NAME, STORAGE_OPTION, ADDOPER, ADDDATE");
      insertSql.Append(" ) VALUES ( ");
      insertSql.Append(" :customer, :file_name, :storage_option, 'eMbrReg', SYSDATE");
      insertSql.Append(" )");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", newMbrId));
      parameters.Add(new OracleParameter("file_name", fileName));
      parameters.Add(new OracleParameter("storage_option", "FILE"));
      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Inserted into Dues_Card_Master table\n");
      return success;
    }

    private static bool UpdateTipData(OracleCommand cmd, string mbrId, string houseId, decimal tipAmount, DateTime tipSignedDate) {
      bool success = true;
      StringBuilder updSql = new StringBuilder();
      updSql.Append("UPDATE CUS_EMPLOYMENT SET ");
      updSql.Append(" POLITICAL_DEDUCT_FLAG = 'Y' ");
      updSql.Append(", " + LocalSettingsConfig.LocalSettings.TipStartDateColumn + " = :tip_signed_date ");
      updSql.Append(", " + LocalSettingsConfig.LocalSettings.TipAmountColumn + " = :tip_amount");
      updSql.Append(", MODOPER = 'eMbrReg', MODDATE = SYSDATE, ROWVERSION = ROWVERSION + 1 ");
      updSql.Append(" WHERE CUSTOMER = :customer AND EMPLOYER_CUSTOMER = :employer_customer ");
      updSql.Append(" AND START_DATE < SYSDATE AND END_DATE IS NULL ");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("tip_signed_date", tipSignedDate));
      parameters.Add(new OracleParameter("tip_amount", tipAmount));
      parameters.Add(new OracleParameter("customer", mbrId));
      parameters.Add(new OracleParameter("employer_customer", houseId));
      ExecuteNonQuery(cmd, updSql.ToString(), parameters);
      logger.Info("\t\t\t Update Tip Data in CUS_Employment table \n");
      return success;
    }

    private static bool UpdateDuesCardMasterTable(OracleCommand cmd, string mbrId, string fileName) {
      bool success = true;
      StringBuilder updSql = new StringBuilder();
      updSql.Append("UPDATE DUES_CARD_MASTER SET ");
      updSql.Append(" FILE_NAME = :file_name");
      updSql.Append(", MODOPER = 'eMbrReg', MODDATE = SYSDATE");
      updSql.Append(" WHERE CUSTOMER = :customer");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("file_name", fileName));
      parameters.Add(new OracleParameter("customer", mbrId));
      ExecuteNonQuery(cmd, updSql.ToString(), parameters);
      return success;
    }

    private static bool InsertIntoPhone(OracleCommand cmd, string newMbrId, string phoneType, string phone, bool isPrimary = false) {
      if (string.IsNullOrEmpty(phone))  // No Phone number, nothing to Insert
        return true;
        bool success = true;
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT  INTO PHONE( ");
      insertSql.Append(" CUSTOMER, PHONE_TYPE, PHONE, PRIMARY_FLAG, ADDOPER, ADDDATE, ROWVERSION");
      insertSql.Append(" ) VALUES ( ");
      insertSql.Append(" :customer, :phone_type, :phone");
      insertSql.Append(", '" + (isPrimary ? "Y" : "N") + "'");
      insertSql.Append(", 'eMbrReg', SYSDATE, 0");
      insertSql.Append(" )");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", newMbrId));
      parameters.Add(new OracleParameter("phone_type", phoneType));
      parameters.Add(new OracleParameter("phone", phone));
      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Inserted into Phone table ({0}) \n", phoneType);
      return success;
    }

    private static bool InsertIntoComments(OracleCommand cmd, string newMbrId, string comments) {
      bool success = true;
      StringBuilder insertSql = new StringBuilder();
      insertSql.Append("INSERT  INTO COMMENTS( ");
      insertSql.Append(" APPL, IDN1, IDN2, COMMENTS, ADDOPER, ADDDATE, ROWVERSION");
      insertSql.Append(" ) VALUES ( ");
      insertSql.Append(" 'DEM002', :customer, null, :comments");
      insertSql.Append(", 'eMbrReg', SYSDATE, 0");
      insertSql.Append(" )");
      List<OracleParameter> parameters = new List<OracleParameter>();

      parameters.Add(new OracleParameter("customer", newMbrId));
      parameters.Add(new OracleParameter("comments", "eMembership Comments: "+comments));
      ExecuteNonQuery(cmd, insertSql.ToString(), parameters);
      logger.Info("\t\t\t Insert into Comments table \n");
      return success;
    }

    private static string GetNewCustomerSeq(OracleCommand cmd) {
      string sql = "select SEQ_CUSTOMER.NextVal from dual";
      string seqValue = ExecuteScalar(cmd, sql);
      seqValue = seqValue.PadLeft(6, '0'); //Note: TIMSS ID is 8 Characters, but the first 2 are used for Local PREFIX
      return seqValue;
    }

    private static int ExecuteNonQuery(string qry, List<OracleParameter> parameters) {
      int result;
      using (OracleConnection timssDbConn = new OracleConnection(ConnectString)) {
        timssDbConn.Open();

        using (OracleCommand cmd = new OracleCommand(qry, timssDbConn)) {
          foreach (var p in parameters)
            cmd.Parameters.Add(p);
          result = cmd.ExecuteNonQuery();
        }
      }
      return result;
    }

    private static int ExecuteNonQuery(OracleCommand cmd, string qry, List<OracleParameter> parameters) {
      int result;
      cmd.CommandText = qry;
      cmd.Parameters.Clear();
      foreach (var p in parameters)
        cmd.Parameters.Add(p);
      result = cmd.ExecuteNonQuery();
      return result;
    }
    private static string ExecuteScalar(OracleCommand cmd, string qry) {
      string result = string.Empty;
      cmd.CommandText = qry;
      var data = cmd.ExecuteScalar();
      if (data != null)
        result = data.ToString();
      return result;
    }
    private static string ExecuteScalar(OracleCommand cmd, string qry, List<OracleParameter> parameters) {
      string result = string.Empty;
      cmd.CommandText = qry;
      cmd.Parameters.Clear();
      foreach (var p in parameters)
        cmd.Parameters.Add(p);

      var data = cmd.ExecuteScalar();
      if (data != null)
        result = data.ToString();

      return result;
    }
    private static string ExecuteScalar(string qry) {
      string result = string.Empty;
      using (OracleConnection timssDbConn = new OracleConnection(ConnectString)) {
        timssDbConn.Open();

        using (OracleCommand cmd = new OracleCommand(qry, timssDbConn)) {
          var data = cmd.ExecuteScalar();
          if (data != null)
            result = data.ToString();
        }
      }
      return result;
    }
    private static OracleDataReader GetDataReader(string qry) {
      OracleConnection timssDbConn = new OracleConnection(ConnectString);

      timssDbConn.Open();

      OracleCommand cmd = new OracleCommand(qry, timssDbConn);
      cmd.CommandTimeout = 3000;

      OracleDataReader rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
      return rd;
    }
    private static string ConnectString {
      get { return ConfigurationManager.ConnectionStrings["Timss"].ConnectionString; }
    }

  }
}