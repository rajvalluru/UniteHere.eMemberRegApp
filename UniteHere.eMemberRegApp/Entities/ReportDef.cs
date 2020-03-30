using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UniteHere.eMemberRegApp.Entities {
  public class ReportDef : IMemberRegAppEntityBase {
    [JsonProperty(PropertyName = "uuid")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "created_by")]
    public string CreatedBy { get; set; }
    [JsonProperty(PropertyName = "created_on")]
    public DateTime CreatedOn { get; set; }
    [JsonProperty(PropertyName = "modified_by")]
    public string ModifiedBy { get; set; }
    [JsonProperty(PropertyName = "modified_on")]
    public Nullable<DateTime> ModifiedOn { get; set; }
    [JsonProperty(PropertyName = "rowversion")]
    public int RowVersion { get; set; }

    [JsonIgnore]
    public virtual IList<ReportSecurity> ReportSecurities { get; set; }
    public virtual IList<ReportParameter> ReportParameters { get; set; }
  }

  public class ReportSecurity : IEntityBase {
    [JsonProperty(PropertyName = "uuid")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "report_id")]
    public string ReportDefId { get; set; }
    [JsonProperty(PropertyName = "role")]
    public string Role { get; set; }
  }

  public class ReportParameter : IEntityBase {
    [JsonProperty(PropertyName = "uuid")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "report_id")]
    public string ReportDefId { get; set; }
    [JsonProperty(PropertyName = "parameter_name")]
    public string ParameterName { get; set; }
    [JsonProperty(PropertyName = "data_type")]
    public string DataType { get; set; }
    [JsonProperty(PropertyName = "required")]
    public bool IsRequired { get; set; }
    [JsonProperty(PropertyName = "default_value")]
    public string DefaultValue { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }
  }

  public class UserParamValues {
    [JsonProperty(PropertyName = "parameter_name")]
    public string ParameterName { get; set; }
    [JsonProperty(PropertyName = "parameter_value")]
    public object ParameterValue { get; set; }
  }
}
