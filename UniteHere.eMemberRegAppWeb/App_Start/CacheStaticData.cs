using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.TimssDataAccess;

namespace UniteHere.eMemberRegAppWeb {
  public static class CacheStaticData {
    public static void LoadData() {
      LoadRamsParameterData();
      //Check if DUES_CARD_IMAGE_LOCATION is accessible
      string duesCardPath = (string)System.Web.HttpContext.Current.Application["DUES_CARD_IMAGE_LOCATION"];
      DirectoryInfo dir = new DirectoryInfo(duesCardPath);
      if (!dir.Exists) {
        System.Web.HttpContext.Current.Application["DUES_CARD_IMAGE_LOCATION"] = LocalSettingsConfig.LocalSettings.TimssDuesCardsPath;
        //throw new Exception("Cannot access " + duesCardPath);
      }
      LoadHouseData();
      LoadMbrLevelReferenceData();
      LoadDiscardReasonsList();
    }

    private static void LoadDiscardReasonsList() {
      List<SelectListItem> result = new List<SelectListItem>();
      result.Add(new SelectListItem() { Text = "--Select--", Value = string.Empty, Selected = true });
      foreach (var item in DiscardReasonSettingsConfig.DiscardReasonSettings.DiscardReasons.All) {
        result.Add(new SelectListItem() { Text = item.Description, Value = item.Code });
      }

      HttpContext.Current.Application["DISCARDREASON_CODES"] = result;
    }

    private static void LoadMbrLevelReferenceData() {
      GetSelectList("MBR_TYPE");
      GetSelectList("MBRLEVEL1");
      GetSelectList("MBRLEVEL2");
      GetSelectList("MBRLEVEL3");
      GetSelectList("DEM_SECTION");
      GetSelectList("CRAFT");
      GetSelectList("DEMFULLPART");
    }

    private static void GetSelectList(string referenceType) {
      IEnumerable<ReferenceData> data = TimssData.GetReferenceData(referenceType);
      List<SelectListItem> result = new List<SelectListItem>();
      result.Add(new SelectListItem() { Text = "--Select--", Value = string.Empty, Selected = true });
      foreach (var item in data) {
        result.Add(new SelectListItem() { Text = item.Description, Value = item.Code });
      }
      //var list = from item in data
      //             select new SelectListItem {
      //               Text = item.Description
      //             , Value = item.Code
      //             };
      //var result = list.ToList();
      //result.Add(new SelectListItem() { Text = "--Select", Value = string.Empty, Selected = true});

      HttpContext.Current.Application[referenceType + "_CODES"] = result;
    }

    private static void LoadHouseData() {
      HttpContext.Current.Application["HouseData"] = TimssData.GetHouseData(ConfigurationManager.AppSettings["LocalNumber"]);
    }

    private static void LoadRamsParameterData() {
      Dictionary<string, string> paramData = TimssData.LoadRamsParameterData();
      foreach (var p in paramData) {
        HttpContext.Current.Application[p.Key] = p.Value;
      }
    }
  }
}