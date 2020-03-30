using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniteHere.eMemberRegApp.Entities {
  public class ImportFileResult {
    public string FileName { get; set; }
    public string FileSize { get; set; }
    public int NoOfLines { get; set; }
    public int InsertedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportLineError> Errors { get; set; } 
  }

  public class ImportLineError
  {
    public  int LineNo { get; set; }
    public string ErrorMessage { get; set; }
  }
}
