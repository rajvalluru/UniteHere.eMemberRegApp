using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace UniteHere.eMemberRegAppWeb {
  public class DiscardReasonSettingsConfig : ConfigurationSection {
    private static DiscardReasonSettingsConfig discardSettings = ConfigurationManager.GetSection("DiscardReasonSettings") as DiscardReasonSettingsConfig;
    public static DiscardReasonSettingsConfig DiscardReasonSettings { get { return discardSettings; } }

    [ConfigurationProperty("DiscardReasons", IsDefaultCollection = false)]
    [ConfigurationCollection(typeof(DiscardReasonCollection))]
    public DiscardReasonCollection DiscardReasons {
      get {return (DiscardReasonCollection)this["DiscardReasons"];      }
      set {this["DiscardReasons"] = value;  }
    }


  }

  public class DiscardReasonCollection : ConfigurationElementCollection {
    public List<DiscardReasonElement> All {  get { return this.Cast<DiscardReasonElement>().ToList();  } }

    public DiscardReasonElement this[int index] {
      get { return (DiscardReasonElement)BaseGet(index); }
      set {
        if (BaseGet(index) != null)
          BaseRemoveAt(index);
        BaseAdd(index, value);
      }
    }

    new public DiscardReasonElement this[string code] {
      get { return (DiscardReasonElement)BaseGet(code); }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new DiscardReasonElement();
    }

    protected override object GetElementKey(ConfigurationElement element) {
     return((DiscardReasonElement)element).Code;
    }
  }

  public class DiscardReasonElement : ConfigurationElement {
    [ConfigurationProperty("Code", IsRequired = true)]
    public string Code { get { return (string)this["Code"]; } }
    [ConfigurationProperty("Description", IsRequired = true)]
    public string Description { get { return (string)this["Description"]; } }
  }
}