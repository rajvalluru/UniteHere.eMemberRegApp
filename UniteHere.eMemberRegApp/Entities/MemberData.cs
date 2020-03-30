using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniteHere.eMemberRegApp.Entities {
  public class MemberData {
    public string MemberUnionId { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string FirstName { get; set; }

    public string Ssn { get; set; }

    public string Address { get; set; }
    public string Address_2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }

    public string HouseUnionId { get; set; }
    public string HouseName { get; set; }

  }
}