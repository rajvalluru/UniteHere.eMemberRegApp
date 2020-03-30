using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegAppWeb.Models;

namespace UniteHere.eMemberRegAppWeb {
  public class AutoMapperConfig {
    public static void RegisterMappings() {
      Mapper.Initialize(cfg => { cfg.CreateMap<EMemberRegistration, EMemberRegistrationViewModel>().ReverseMap(); });
    }
  }
}