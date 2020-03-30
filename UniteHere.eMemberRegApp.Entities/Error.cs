using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegApp.Entities {
  public class Error : IEntityBase {
    [JsonProperty(PropertyName = "uuid")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "code")]
    public int ErrorCode { get; set; }
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }
    [JsonProperty(PropertyName = "stack_trace")]
    public string StackTrace { get; set; }
    [JsonProperty(PropertyName = "date_created")]
    public DateTime DateCreated { get; set; }
  }
}
