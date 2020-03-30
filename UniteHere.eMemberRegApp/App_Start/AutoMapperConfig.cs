using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Models;
using AutoMapper;

namespace UniteHere.eMemberRegApp {
  public class AutoMapperConfig {
    public static void RegisterMappings() {
      Mapper.Initialize(cfg => { cfg.CreateMap<EMemberRegistration, EMemberRegistrationModel>().ReverseMap(); });
    }
  }
}