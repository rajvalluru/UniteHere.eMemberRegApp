using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegAppWeb.Configurations {
  public class EMemberRegistrationConfiguration : EMemberRegAppEntityBaseConfiguration<EMemberRegistration> {
    public EMemberRegistrationConfiguration() {
      Property(m => m.First_Name).HasMaxLength(100);
      Property(m => m.Last_Name).HasMaxLength(100);
      Property(m => m.Middle_Name).HasMaxLength(50);
      Property(m => m.Address).HasMaxLength(100);
      Property(m => m.Address_2).HasMaxLength(100);
      Property(m => m.City).HasMaxLength(100);
      Property(m => m.State).HasMaxLength(10);
      Property(m => m.Postal_Code).HasMaxLength(20);
      Property(m => m.Home_Phone).HasMaxLength(100);
      Property(m => m.Mobile_Phone).HasMaxLength(100);
      Property(m => m.Email).HasMaxLength(100);
      Property(m => m.Employer_Union_Id).HasMaxLength(20);
      Property(m => m.Position).HasMaxLength(100);
      Property(m => m.Ssn).HasMaxLength(12);
      Property(m => m.Race).HasMaxLength(20);
      Property(m => m.Gender).HasMaxLength(20);
      Property(m => m.Other_Gender).HasMaxLength(100);
      Property(m => m.Ethnicity).HasMaxLength(20);
      Property(m => m.Country).HasMaxLength(20);
      Property(m => m.First_Language).HasMaxLength(100);
      Property(m => m.Second_Language).HasMaxLength(100);
      Property(m => m.Tip_Frequency).HasMaxLength(20);
      Property(m => m.LocalNumber).HasMaxLength(20);
      Property(m => m.Validation_Status).HasMaxLength(1000);
      Property(m => m.Processed_Status).HasMaxLength(256);
      Property(m => m.Member_Union_Id).HasMaxLength(20);
      Property(m => m.Dues_Card_File_Name).HasMaxLength(100);
      Property(m => m.Mbr_Level1).HasMaxLength(20);
      Property(m => m.Mbr_Level2).HasMaxLength(20);
      Property(m => m.Mbr_Level3).HasMaxLength(20);
      Property(m => m.Mbr_Type).HasMaxLength(20);
      Property(m => m.Employer_Name).HasMaxLength(100);
      Property(m => m.Department).HasMaxLength(20);
      Property(m => m.Section).HasMaxLength(20);
      Property(m => m.Craft).HasMaxLength(20);
      Property(m => m.FullPartTime).HasMaxLength(20);
      Property(m => m.Notes).HasMaxLength(2000);
      Property(m => m.TipActionCode).HasMaxLength(10);
      Property(m => m.Country_Of_Origin).HasMaxLength(5);
      Property(m => m.Discard_Reason).HasMaxLength(100);
      Property(m => m.Beneficiary).HasMaxLength(100);
      Property(m => m.Work_Phone).HasMaxLength(100);
      Property(m => m.Dues_Card_BoxId).HasMaxLength(256);
    }
  }
}
