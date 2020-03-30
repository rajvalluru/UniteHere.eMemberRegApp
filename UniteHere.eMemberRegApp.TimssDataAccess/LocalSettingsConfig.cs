using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace UniteHere.eMemberRegApp.TimssDataAccess {
  public class LocalSettingsConfig : ConfigurationSection {
    private static LocalSettingsConfig localSettings = ConfigurationManager.GetSection("LocalSettings") as LocalSettingsConfig;
    public static LocalSettingsConfig LocalSettings { get { return localSettings; } }

    [ConfigurationProperty("localNumber", DefaultValue = "IU", IsRequired = true)]
    public string LocalNumber { get { return (string)this["localNumber"]; } }

    [ConfigurationProperty("customerClass", DefaultValue = "IND", IsRequired = true)]
    public string CustomerClass { get { return (string)this["customerClass"]; } }

    [ConfigurationProperty("customerStatus", DefaultValue = "ACTIVE", IsRequired = true)]
    public string CustomerStatus { get { return (string)this["customerStatus"]; } }

    [ConfigurationProperty("secongLanguageColumn", DefaultValue = "USER_T2", IsRequired = true)]
    public string SecongLanguageColumn { get { return (string)this["secongLanguageColumn"]; } }

    [ConfigurationProperty("raceColumn", DefaultValue = "", IsRequired = true)]
    public string RaceColumn { get { return (string)this["raceColumn"]; } }

    [ConfigurationProperty("tipStartDateColumn", DefaultValue = "USER_D1", IsRequired = true)]
    public string TipStartDateColumn { get { return (string)this["tipStartDateColumn"]; } }

    [ConfigurationProperty("tipAmountColumn", DefaultValue = "USER_N1", IsRequired = true)]
    public string TipAmountColumn { get { return (string)this["tipAmountColumn"]; } }

    [ConfigurationProperty("duesCardsPath", IsRequired = true)]
    public string DuesCardsPath { get { return (string)this["duesCardsPath"]; } }

    [ConfigurationProperty("duesCardsHistoryPath", DefaultValue = @"\\server2008\DuesCards_History", IsRequired = true)]
    public string DuesCardsHistoryPath { get { return (string)this["duesCardsHistoryPath"]; } }

    [ConfigurationProperty("smsOptIn", IsRequired = true)]
    public SmsOptInConfigElement SmsOptIn { get { return (SmsOptInConfigElement)this["smsOptIn"]; } }

    [ConfigurationProperty("timssDuesCardsPath", IsRequired = true)]
    public string TimssDuesCardsPath { get { return (string)this["timssDuesCardsPath"]; } }

    [ConfigurationProperty("studentFlag", IsRequired = true)]
    public StudentFlagConfigElement StudentFlag { get { return (StudentFlagConfigElement)this["studentFlag"]; } }


  }

  public class SmsOptInConfigElement : ConfigurationElement {
    [ConfigurationProperty("trackingAs", DefaultValue = "PHONE", IsRequired = true)]
    public string TrackingAs { get { return this["trackingAs"] as string; } }

    [ConfigurationProperty("phoneType", DefaultValue = "TXCELL")]
    public string PhoneType { get { return this["phoneType"] as string; } }

    [ConfigurationProperty("userFlag", DefaultValue = "USER_F9")]
    public string UserFlag { get { return this["userFlag"] as string; } }

  }

  public class StudentFlagConfigElement : ConfigurationElement {
    [ConfigurationProperty("tableName", DefaultValue = "CUSTOMER", IsRequired = true)]
    public string TableName { get { return this["tableName"] as string; } }

    [ConfigurationProperty("userFlag", DefaultValue = "USER_F15")]
    public string UserFlag { get { return this["userFlag"] as string; } }

  }

}