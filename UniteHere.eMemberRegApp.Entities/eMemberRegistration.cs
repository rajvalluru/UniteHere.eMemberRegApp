using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegApp.Entities {
 public  class EMemberRegistration : IMemberRegAppEntityBase {
    [JsonProperty(PropertyName = "uuid")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "first_name")]
    public string First_Name { get; set; }
    [JsonProperty(PropertyName = "last_name")]
    public string Last_Name { get; set; }
    [JsonProperty(PropertyName = "middle_name")]
    public string Middle_Name { get; set; }
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    [JsonProperty(PropertyName = "dob")]
    public DateTime Dob { get; set; }
    [JsonProperty(PropertyName = "address")]
    public string Address { get; set; }
    [JsonProperty(PropertyName = "address_2")]
    public string Address_2 { get; set; }
    [JsonProperty(PropertyName = "city")]
    public string City { get; set; }
    [JsonProperty(PropertyName = "state")]
    public string State { get; set; }
    [JsonProperty(PropertyName = "postal_code")]
    public string Postal_Code { get; set; }
    [JsonProperty(PropertyName = "deduction_opt_out")]
    public bool Deduction_Opt_Out_Flag { get; set; }
    [JsonProperty(PropertyName = "home_phone")]
    public string Home_Phone { get; set; }
    [JsonProperty(PropertyName = "mobile_phone")]
    public string Mobile_Phone { get; set; }
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }
    [JsonProperty(PropertyName = "employer_id")]
    public string Employer_Union_Id { get; set; }
    [JsonProperty(PropertyName = "position")]
    public string Position { get; set; }
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    [JsonProperty(PropertyName = "date_of_hire")]
    public DateTime? Date_Of_Hire { get; set; }
    [JsonProperty(PropertyName = "ssn")]
    public string Ssn { get; set; }
    [JsonProperty(PropertyName = "sms_opt_in")]
    public bool Sms_Opt_In_Flag { get; set; }
    [JsonProperty(PropertyName = "race_id")]
    public string Race { get; set; }
    [JsonProperty(PropertyName = "gender_id")]
    public string Gender { get; set; }
    [JsonProperty(PropertyName = "other_gender")]
    public string Other_Gender { get; set; }
    [JsonProperty(PropertyName = "ethnicity_id")]
    public string Ethnicity { get; set; }
    [JsonProperty(PropertyName = "country_id")]
    public string Country { get; set; }
    [JsonProperty(PropertyName = "first_language_id")]
    public string First_Language { get; set; }
    [JsonProperty(PropertyName = "second_language_id")]
    public string Second_Language { get; set; }
    [JsonProperty(PropertyName = "tip_opt_in")]
    public bool Tip_Opt_In_Flag { get; set; }
    [JsonProperty(PropertyName = "tip_frequency_id")]
    public string Tip_Frequency { get; set; }
    [JsonProperty(PropertyName = "tip_contribution")]
    public decimal Tip_Contribution { get; set; }
    [JsonProperty(PropertyName = "local_number")]
    public string LocalNumber { get; set; }

    public bool Validated_Flag { get; set; }
    public string Validation_Status { get; set; }
    public DateTime? Validation_Date { get; set; }
    public bool Processed_Flag { get; set; }
    public string Processed_Status { get; set; }
    public DateTime? Processed_Date { get; set; }
    public bool Success_Flag { get; set; }
    public string Member_Union_Id { get; set; }


    [JsonProperty(PropertyName = "created_by")]
    public string CreatedBy { get; set; }
    [JsonProperty(PropertyName = "created_on")]
    public DateTime CreatedOn { get; set; }
    [JsonProperty(PropertyName = "modified_by")]
    public string ModifiedBy { get; set; }
    [JsonProperty(PropertyName = "modified_on")]
    public DateTime? ModifiedOn { get; set; }
    [JsonProperty(PropertyName = "rowversion")]
    public int RowVersion { get; set; }

    public string Dues_Card_File_Name { get; set; }
    public byte[] Dues_Card_Image { get; set; }
    public string Mbr_Level1 { get; set; }
    public string Mbr_Level2 { get; set; }
    public string Mbr_Level3 { get; set; }
    public string Mbr_Type { get; set; }
    public string Employer_Name { get; set; }

    public  bool UpdateName { get; set; }
    public bool UpdateAddress { get; set; }
    public bool UpdateHouse { get; set; }
    public bool UpdatePhone { get; set; }
    public bool UpdateEmail { get; set; }

    public string Department { get; set;  }
    public string Section { get; set; }
    public string Craft { get; set; }
    [JsonProperty(PropertyName = "employment_type")]
    public string FullPartTime { get; set; }

    public string Notes { get; set; }

    //Adding TipAction Code - Add/Replace/Ignore.  This will decide if we update the TIP information
    public string TipActionCode { get; set; }
    [JsonProperty(PropertyName = "country_of_origin")]
    public string Country_Of_Origin { get; set; }
    public string Discard_Reason { get; set; }
    [JsonProperty(PropertyName = "dues_card_signed_date")]
    public DateTime? Dues_Card_Signed_Date { get; set; }
    [JsonProperty(PropertyName = "tip_card_signed_date")]
    public DateTime? TIP_Card_Signed_Date { get; set; }

    [JsonProperty(PropertyName = "student_flag")]
    public bool Student_Flag { get; set; }
    [JsonProperty(PropertyName = "beneficiary")]
    public string Beneficiary { get; set; }
    [JsonProperty(PropertyName = "work_phone")]
    public string Work_Phone { get; set; }

    [JsonProperty(PropertyName = "duescard_box_id")]
    public string Dues_Card_BoxId { get; set; }
  }
}
