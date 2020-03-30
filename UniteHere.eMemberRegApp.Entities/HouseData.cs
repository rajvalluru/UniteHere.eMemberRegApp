using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UniteHere.eMemberRegApp.Entities {
  public class HouseData {
    [JsonProperty(PropertyName = "local_number")]
    public string LocalNumber { get; set; }

    [JsonProperty(PropertyName = "timss_id")]
    public string HouseUnionId { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string HouseName { get; set; }
    [JsonProperty(PropertyName = "display_name")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "is_right_to_work")]
    public bool IsRightToWork { get; set; }
  }
}