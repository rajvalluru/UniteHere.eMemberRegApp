using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniteHere.eMemberRegAppWeb.Models {
  public class EMemberRegistrationViewModel {
    [Display(Name = "uuid")]
    public string Id { get; set; }

    [Display(Name = "First Name")]
    public string First_Name { get; set; }
    [Display(Name = "Last Name")]
    public string Last_Name { get; set; }
    [Display(Name = "Middle Name")]
    public string Middle_Name { get; set; }
    public string Ssn { get; set; }

    [Display(Name = "Hire Date")]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? Date_Of_Hire { get; set; }

    [Display(Name = "Mobile")]
    public string Mobile_Phone { get; set; }

    [Display(Name = "Employer")]
    public string Employer_Union_Id { get; set; }
    [Display(Name = "Position")]
    public string Position { get; set; }

    [Display(Name = "TIMSS Id")]
    public string Member_Union_Id { get; set; }

    [Display(Name = "Registration Date")]
    public DateTime CreatedOn { get; set; }

    public string LocalNumber { get; set; }

    public string Dues_Card_File_Name { get; set; }

    [Display(Name = "Status")]
    public string Processed_Status { get; set; }

    public bool Deduction_Opt_Out_Flag { get; set; }
    [Display(Name = "SMS")]
    public bool Sms_Opt_In_Flag { get; set; }
    [Display(Name = "TIP")]
    public decimal Tip_Contribution { get; set; }

    [Display(Name = "New Member")]
    public bool IsNewMember { get; set; }

    public string Notes { get; set; }

    public string Header {
      get {
        return ("Last_Name, First_Name, New_Member, Mobile,   "
              + "Employer_Id, Position,"
              + "Checkoff, Sms_Optin, Tip_Amount, "
              + "Hire_Date, Registration_Date, Notes");
      }
    }

    public string Stringify {
      get {
        return (Last_Name.Replace(",", " ") + "," + First_Name.Replace(",", " ") + "," +(IsNewMember?"Y":"N") + "," + Mobile_Phone + "," 
              + Employer_Union_Id + "," + Position + "," 
              + (!Deduction_Opt_Out_Flag?"Y":"N") + "," + Sms_Opt_In_Flag + "," + Tip_Contribution + ","
              + Date_Of_Hire + "," + CreatedOn + "," + Notes );
      }
    }

  }
}