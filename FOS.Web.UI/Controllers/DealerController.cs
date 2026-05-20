using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class DealerController : Controller
    {
        private FOSDataModel db = new FOSDataModel();

        #region DEALER

        [CustomAuthorize]
        // View ...
        public ActionResult Plan()
        {
            ViewData["RegionalHead"] = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            ViewData["Dealers"] = FOS.Setup.ManageDealer.GetDealerList();
            ViewData["SaleOfficer"] = new List<SaleOfficerData>();//FOS.Setup.ManageSaleOffice.GetSaleOfficerList(true);
            ViewData["City"] = FOS.Setup.ManageCity.GetCityList();
            ViewData["Zone"] = new List<ZoneData>();// FOS.Setup.ManageZone.GetZoneList();

            Session["Month"] = DateTime.Today.ToString("MMMM");

            var objDealer = ManageDealer.PlannedDistributors(new PlannedRetailerFilter());
            return View(new PlannedRetailerFilter
            {
                DealerList = objDealer
            });
        }

        [HttpPost]
        // View ...
        public ActionResult Plan(PlannedRetailerFilter model)
        {
            ViewData["RegionalHead"] = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            ViewData["Dealers"] = FOS.Setup.ManageDealer.GetDealerList();
            ViewData["SaleOfficer"] = new List<SaleOfficerData>();// FOS.Setup.ManageSaleOffice.GetSaleOfficerList(true);
            ViewData["City"] = FOS.Setup.ManageCity.GetCityList();
            ViewData["Zone"] = new List<ZoneData>();// FOS.Setup.ManageZone.GetZoneList();
            // set dropdowns in session

            Session["Month"] = model.month;
            Session["RegionalHeadID"] = model.RegionalHeadID;
            Session["DealerID"] = model.DealerID;
            Session["SaleOfficerID"] = model.SaleOfficerID;
            Session["CityID"] = model.CityID;
            Session["ZoneID"] = model.ZoneID;

            var objDealer = ManageDealer.PlannedDistributors(model);
            return View(new PlannedRetailerFilter
            {
                DealerList = objDealer
            });
        }


        //public void ExportToExcel()
        //{

        //    // Example data
        //    StringWriter sw = new StringWriter();

        //    sw.WriteLine("\"Fixed ID\",\"Person Name\",\"Customer Name\",\"Zone\",\"City Name\",\"Area Name\",\"Address\",\"Dealer Code\",\"CNIC\",\"Phone1\",\"Phone2\",\"Dealer Type\"");

        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment;filename=Dealers" + DateTime.Now + ".csv");
        //    Response.ContentType = "application/octet-stream";

        //    var retailers = ManageDealer.GetDealersForExportinExcel();

        //    foreach (var retailer in retailers)
        //    {
        //        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",

        //        retailer.ID,
        //        retailer.Name,
        //        retailer.ShopName,
        //        retailer.RegionName,
                
        //        retailer.CItyName,
        //        retailer.AreaName,
        //        retailer.Address,
        //        retailer.RetailerCode,
        //        retailer.CNIC,
        //        retailer.Phone1,
        //        retailer.Phone2
                


        //        ));
        //    }
        //    Response.Write(sw.ToString());
        //    Response.End();


        //}

        //[CustomAuthorize]
        // View ...
        public ActionResult DealerCities()
        {
            // Load Region Data For City Records ...
            var objDealer = new DealerCityData();
            objDealer.RegionalHeads = ManageRegionalHead.GetTerritorialHeadRegionalHeadList();
            objDealer.Dealers = ManageDealer.GetAllDealersListRelatedToRegionalHead(objDealer.RegionalHeads.Select(rh => rh.ID).FirstOrDefault(), true, "Select");
            //objDealer.Cities = ManageCity.GetCityListCombo();
            objDealer.Cities = ManageCity.GetCityListByRegionHeadID(objDealer.RegionalHeads.Select(rh => rh.ID).FirstOrDefault());
            //objDealer.Areas = ManageArea.GetAreaListByCityID(objDealer.Cities.Select(c => c.ID).FirstOrDefault());
            return View(objDealer);
        }
        [HttpPost]
        public ActionResult GetDealerCityAndAreas(int DealerID, int CityID)
        {
            var result = FOS.Setup.ManageDealer.GetDealerCityAndAreas(DealerID, CityID);

            string chkboxAllChecked = "checked='checked'";

            if (CityID > 0)
            {
                chkboxAllChecked = "";
            }
            string html = "<div><table class='table table-striped table-hover table-bordered no-footer'>";
            html += "<thead><tr><th><input id='chkboxAll' onchange='chkboxAllChange(this);' type='checkbox' " + chkboxAllChecked + "/>Select All</th><th>City</th><th>Area</th></tr></thead>";
            List<int> areaIds = new List<int>();
            int i = 1;
            string row = "odd";

            if (result.Count > 0)
            {

                foreach (var deal in result)
                {
                    html += "<tr class='" + row + "'>";

                    html += "<td><input type='checkbox' checked='checked'"
                        + " id='" + deal.ID
                        + "' dealer-id='" + deal.DealerID
                        + "' city-id='" + deal.CityID
                        + "' area-id='" + deal.AreaID
                        + "' /></td>";

                    html += "<td>" + deal.CityName + "</td>";
                    html += "<td>" + deal.AreaName + "</td>";

                    html += "</tr>";

                    areaIds.Add(deal.AreaID);

                    i++;
                    row = i % 2 == 0 ? "even" : "odd";
                }
            }
            else if (CityID == 0)
            {
                return Content("<br/><div style='margin-top: 10px;margin-left:32px;float:left;color:red;'>No data found</div>");
            }

            if (CityID > 0)
            {
                // get all other areas that are not in areaIds list of this city
                List<AreaData> cityAreas = ManageArea.GetAreaListByCityIDForDealers(CityID);
                List<AreaData> finalAreas = cityAreas.Where(x => !areaIds.Contains(x.ID))
                                            .OrderBy(p => p.Name).ToList();

                foreach (var deal in finalAreas)
                {
                    html += "<tr class='" + row + "'>";

                    html += "<td><input type='checkbox'"
                        + " id='" + deal.ID
                        + "' dealer-id='" + DealerID
                        + "' city-id='" + deal.CityID
                        + "' area-id='" + deal.ID
                        + "' /></td>";

                    html += "<td>" + deal.CityName + "</td>";
                    html += "<td>" + deal.Name + "</td>";

                    html += "</tr>";

                    i++;
                    row = i % 2 == 0 ? "even" : "odd";
                }
            }

            html += "</table></div>";

            return Content(html);
        }
        [HttpPost]
        public ActionResult GetDealerCityAndAreasCitywise(int DealerID)
        {
            var dealerAllCityAreas = FOS.Setup.ManageDealer.GetDealerCityAndAreasCitywise(DealerID);

            string chkboxAllChecked = "";

            
            string html = "<div><table class='table table-striped table-hover table-bordered no-footer'>";
            html += "<thead><tr><th><input id='chkboxAll' onchange='chkboxAllChange(this);' type='checkbox' " + chkboxAllChecked + "/>Select All</th><th>City</th></tr></thead>";
            
            int i = 1;
            string row = "odd";
            List<CityData> allCities = ManageCity.GetCityList();
            if (allCities.Count > 0)
            {
                var checkboxChecked = "";
                foreach (var city in allCities)
                {
                    List<AreaData> cityAreas = ManageArea.GetAreaListByCityIDForDealers(city.ID);
                    if (cityAreas.Count > 0)
                    {
                        html += "<tr class='" + row + "'>";

                        int dealerAreacount = dealerAllCityAreas.Where(p => p.CityID == city.ID).Count();
                        if (cityAreas.Count == dealerAreacount)
                        {
                            checkboxChecked = "checked='checked'";
                        }
                        else
                        {
                            checkboxChecked = "";
                        }

                        html += "<td><input type='checkbox' " + checkboxChecked + " "
                            + " id='" + city.ID
                            + "' dealer-id='" + DealerID
                            + "' city-id='" + city.ID
                            + "' /></td>";

                        html += "<td>" + city.Name + "</td>";

                        html += "</tr>";

                        i++;
                        row = i % 2 == 0 ? "even" : "odd";
                    }                     
                }
            }
            else 
            {
                return Content("<br/><div style='margin-top: 10px;margin-left:32px;float:left;color:red;'>No data found</div>");
            }


            html += "</table></div>";

            return Content(html);
        }

        [HttpPost]
        public ActionResult SaveDealerCityAndAreas(int DealerID, string cityAreas, int CityID)
        {
            if (DealerID > 0 && !string.IsNullOrEmpty(cityAreas) && cityAreas.Length > 1)
            {
                int userId = SessionManager.Get<int>("UserID");
                string[] cityAreasSplit = cityAreas.Split(';');
                if(cityAreasSplit.Length > 0)
                {
                    for (int i = 0; i < cityAreasSplit.Length; i++)
                    {
                        string[] cityarea = cityAreasSplit[i].Split('-');
                        if(cityarea.Length == 2)
                        {
                            int cityId = int.Parse(cityarea[0]);
                            int areaId = int.Parse(cityarea[1]);
                            if (i == 1)
                            {
                                // delete old records
                                ManageDealer.DeleteDealerCityAreas(DealerID, CityID);
                            }
                            //add new now
                            ManageDealer.AddDealerCityAreas(DealerID, cityId, areaId, userId);
                        }
                    }
                }
                return Json("ok");
            }
            else if (DealerID > 0 && CityID > 0)
            {
                ManageDealer.DeleteDealerCityAreas(DealerID, CityID);
                return Json("deleteok");
            }
            return Json("notok");
        }


        [HttpPost]
        public ActionResult SaveDealerCityAndAreasCitywise(int DealerID, string cities)
        {
            if (DealerID > 0)
            {
                if (!string.IsNullOrEmpty(cities) && cities.Length > 1)
                {
                    int userId = SessionManager.Get<int>("UserID");
                    string[] citySplit = cities.Split(';');
                    if (citySplit.Length > 0)
                    {
                        for (int i = 0; i < citySplit.Length; i++)
                        {
                            if (citySplit[i].Length > 0)
                            {
                                int cityId = int.Parse(citySplit[i]);
                                // delete old records
                                ManageDealer.DeleteDealerCityAreasCitywise(DealerID, cityId);

                                //add all areas of every city
                                List<AreaData> cityAreas = ManageArea.GetAreaListByCityIDForDealers(cityId);
                                foreach (var area in cityAreas)
                                {
                                    ManageDealer.AddDealerCityAreas(DealerID, cityId, area.ID, userId);
                                }
                            }
                        }
                    }
                    return Json("ok");
                }
                else
                {
                    // delete old records
                    //ManageDealer.DeleteDealerCityAreasCitywise(DealerID);
                    return Json("notok");
                }
            }
            return Json("notok");
        }

        [CustomAuthorize]
        // View ...
        public ActionResult Dealer()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
          
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToDealer(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
           var regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            //List<DealerData> DealerObj = ManageDealer.GetDealerListBySaleOfficerID(objSaleOff.ID);

            var objRetailer = new RetailerData();
            objRetailer.RegionsList = ranges;
            
            objRetailer.RegionalHead = regionalHeadData;
            objRetailer.SaleOfficerss = SaleOfficerObj;
            //objRetailer.Dealers = DealerObj;
            objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
          //  objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.Banks = ManageRetailer.GetBanks();

            return View(objRetailer);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUpdateDealer([Bind(Exclude = "TID,SaleOfficers")] DealerData newDealer)
        {
            Boolean boolFlag = true;
            Boolean PhoneNumberFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newDealer != null)
                    {
                        if (newDealer.ID == 0)
                        {
                            DealerValidator validator = new DealerValidator();
                            results = validator.Validate(newDealer);
                            boolFlag = results.IsValid;
                        }

                        if (newDealer.Phone1 != null)
                        {
                            if (FOS.Web.UI.Common.NumberCheck.CheckDealerNumber1Exist(newDealer.ID, newDealer.Phone1 == null ? "" : newDealer.Phone1) == 1)
                            {
                                return Content("2");
                            }
                        }

                        if (newDealer.Phone2 != null)
                        {
                            if (FOS.Web.UI.Common.NumberCheck.CheckDealerNumber2Exist(newDealer.ID, newDealer.Phone2 == null ? "" : newDealer.Phone2) == 1)
                            {
                                return Content("2");
                            }
                        }

                        if (newDealer.Phone1 != null && newDealer.Phone2 != null)
                        {
                            if (FOS.Web.UI.Common.NumberCheck.CheckDealerNumberExist(newDealer.ID, newDealer.Phone1 == null ? "" : newDealer.Phone1, newDealer.Phone2 == null ? "" : newDealer.Phone2) == 1)
                            {
                                PhoneNumberFlag = false;
                            }
                        }

                        if (PhoneNumberFlag)
                        {
                            if (boolFlag)
                            {
                                int Response = ManageDealer.AddUpdateDealer(newDealer);
                                if (Response == 1)
                                {
                                    return Content("1");
                                }
                                else if (Response == 2)
                                {
                                    return Content("3");
                                }
                                else
                                {
                                    return Content("0");
                                }
                            }
                            else
                            {
                                IList<ValidationFailure> failures = results.Errors;
                                StringBuilder sb = new StringBuilder();
                                sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                                foreach (ValidationFailure failer in results.Errors)
                                {
                                    sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                                    Response.StatusCode = 422;
                                    return Json(new { errors = sb.ToString() });
                                }
                            }
                        }
                        else
                        {
                            return Content("2");
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        public JsonResult GetSaleOfficerByRegionalHeadID(int ID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetSaleOfficerByRegionalHeadID(ID);
            return Json(result);
        }

        //Get All Region Method...
        public JsonResult DistributorDataHandler(DTParameters param)
        {
            try
            {
                var userID = Convert.ToInt32(Session["UserID"]);
                var dtsource = new List<RetailerData>();

                int RegionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

              

                    dtsource = ManageDealer.GetDistributorForGrid(param.RangeID);
            
                //else
                //{
                //    dtsource = ManageRetailer.GetRetailerForGrid(RegionalheadID);
                //}

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RetailerData> data = ManageRetailer.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRetailer.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RetailerData> result = new DTResult<RetailerData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Region Method...
        public int DeleteDealer(int dealerID)
        {
            return ManageDealer.DeleteDealer(dealerID);
        }

        public JsonResult GetCityListByRegionHeadID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(RegionID, "Select");
            return Json(result);
        }

        public JsonResult GetCityListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionID(RegionID, "Select");
            return Json(result);
        }

        public JsonResult GetRetailerListByRegionID(int RegionID,int CityID)
        {
            var result = FOS.Setup.ManageCity.GetRetailersByRegionID(RegionID,CityID ,"Select");
            return Json(result);
        }

        public JsonResult GetCityListByRegionHeadIDSelectAllText(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(RegionalHeadID, "All");
            return Json(result);
        }

        public JsonResult GetAreaListByCityID(int CityID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityID(CityID);
            return Json(result);
        }

        public JsonResult GetAllDealersListRelatedToRegionalHead(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageDealer.GetAllDealersListRelatedToRegionalHead(RegionalHeadID);
            return Json(result);
        }
        public JsonResult GetDealersByRegionalHeadId(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageDealer.GetAllDealersListRelatedToRegionalHead(RegionalHeadID, true, "Select");
            return Json(result);
        }

        public JsonResult GetAllDealersListRelatedToRegionalHeadSelectOption(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageDealer.GetAllDealersListRelatedToRegionalHeadSelectOption(RegionalHeadID, true);
            return Json(result);
        }

        public JsonResult UpdatePlannedUnplanned(int DealerID)
        {
            var result = FOS.Setup.ManageDealer.UpdatePlannedUnplanned(DealerID);
            return Json(result);
        }



        public ActionResult NewDealer()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetSaleOfficerListByRegionalHeadID(regId);
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new RetailerData();
            objRetailer.Rangess = ranges;
            objRetailer.RegionalHead = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Dealers = DealerObj;
            objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Banks = ManageRetailer.GetBanks();

            return View(objRetailer);
        }



        public ActionResult NewUpdateDistributor([Bind(Exclude = "TID,SaleOfficers,Dealers")] RetailerData newRetailer)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                    if (newRetailer.ID == 0)
                    {
                        RetailerValidator validator = new RetailerValidator();
                        results = validator.Validate(newRetailer);
                        boolFlag = results.IsValid;
                    }

                    //if (newRetailer.Phone1 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumber1Exist(newRetailer.ID, newRetailer.Phone1 == null ? "" : newRetailer.Phone1) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newRetailer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumber2Exist(newRetailer.ID, newRetailer.Phone2 == null ? "" : newRetailer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newRetailer.Phone1 != null && newRetailer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumberExist(newRetailer.ID, newRetailer.Phone1 == null ? "" : newRetailer.Phone1, newRetailer.Phone2 == null ? "" : newRetailer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckCNICExist(newRetailer.CNIC, newRetailer.ID) == 1)
                    //{
                    //    return Content("3");
                    //}
                    //else
                    //{
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckAccountNoExist(newRetailer.AccountNo, newRetailer.ID) == 1)
                    //{
                    //    return Content("4");
                    //}
                    //else
                    //{
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckCardNoExist(newRetailer.CardNumber, newRetailer.ID) == 1)
                    //{
                    //    return Content("5");
                    //}
                    //else
                    //{
                    //}

                    if (boolFlag)
                    {
                        try
                        {

                            newRetailer.CreatedBy = SessionManager.Get<int>("UserID");
                            newRetailer.UpdatedBy = SessionManager.Get<int>("UserID");

                        }
                        catch { newRetailer.CreatedBy = 1; }

                        int Res = ManageDealer.AddUpdateDistributor(newRetailer);

                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else if (Res == 4)
                        {
                            return Content("4");
                        }
                        else if (Res == 5)
                        {
                            return Content("5");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }

                }
                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }


        public ActionResult DistributorMapView()
        {
            ViewData["RegionalHead"] = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
         //   ViewData["Dealers"] = FOS.Setup.ManageDealer.GetDealerList();
            ViewData["SaleOfficer"] = FOS.Setup.ManageSaleOffice.GetSaleOfficerList(true);
            ViewData["Region"] = FOS.Setup.ManageRegion.GetRegionForGrid();
            ViewData["City"] = FOS.Setup.ManageCity.GetCityList();
            ViewData["Zone"] = FOS.Setup.ManageZone.GetZoneList();

            return View();
        }


        public JsonResult GetEditDealer(int DealerID)
        {
            var Response = ManageDealer.GetEditDealer(DealerID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }






        #endregion DEALER


        #region DispatchingDealer

        public ActionResult DealerDispatch()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var DealerID = db.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
            var Dealerrange = db.Dealers.Where(x => x.ID == DealerID).Select(x => x.RangeID).FirstOrDefault();

    

            List<RegionData> RegionObj = FOS.Setup.ManageCity.GetDelieveryBoysList(DealerID);
            var objregion = RegionObj.FirstOrDefault();

            List<SaleOfficerData> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoDealerDSR((int)DealerID, Dealerrange);
            var objRetailer = new RetailerData();
            
            objRetailer.Regions = RegionObj;
            var IDs = objRetailer.Regions.FirstOrDefault();
            objRetailer.SaleOfficers = SaleOfficerObj;
       

            return View(objRetailer);
        }

        //public ActionResult NewUpdateDistributorDispatches([Bind(Exclude = "TID,SaleOfficers,Dealers")] RetailerData newRetailer, string StartingDate1)
        //{
        //    Boolean boolFlag = true;
        //    ValidationResult results = new ValidationResult();
        //    try
        //    {
        //        List<Items> Itemdata = new List<Items>();
        //        Items cty;
        //        DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate1) ? DateTime.Now.ToString() : StartingDate1);
        //        DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate1) ? DateTime.Now.ToString() : StartingDate1);
        //        DateTime final = end.AddDays(1);
        //        string Res = "0";

        //        if (newRetailer != null)
        //        {
        //            if (newRetailer.ID == 0)
        //            {
        //                RetailerValidator validator = new RetailerValidator();
        //                results = validator.Validate(newRetailer);
        //                boolFlag = results.IsValid;
        //            }


        //            if (boolFlag)
        //            {
        //                if (StartingDate1 == null)
        //                {
        //                    Res = "2";
        //                }

        //                else
        //                {
        //                    var userID = Convert.ToInt32(Session["UserID"]);
        //                    var DealerID = db.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();

        //                    Res = ManageDealer.AddDistributorDispatchNote(newRetailer, StartingDate1,DealerID);

        //                    if (Res == "1")
        //                    {
        //                        var DBOY = "";
        //                        var SOName = "";
        //                        String[] strRegionId = newRetailer.CityIDs.Split(',');
        //                        string FilePathReturn = "";
        //                        try
        //                        {
        //                            foreach (var item in strRegionId)
        //                            {
        //                                SOName +="/"+ db.SaleOfficers.Where(x => x.ID.ToString() == item).Select(x=>x.Name).FirstOrDefault();
        //                                var ID = Convert.ToInt32(item);
        //                                var ReportData = db.Sp_GetDispatchDataForReport(ID, start, final).ToList();
        //                                foreach (var items in ReportData)
        //                                {
        //                                    cty = new Items();
        //                                    cty.ItemID = (int)items.ItemID;
        //                                        cty.ItemName = items.itemName;
        //                                        cty.Packing = (int)items.packing;
        //                                        cty.DispatchQuantity = (int)items.Quantity;
        //                                        cty.ItemPrice = items.price;
                                                
                                        
        //                                    Itemdata.Add(cty);
        //                                }


        //                            }
        //                            var data = Itemdata.GroupBy(d => d.ItemID)
        //                                            .Select(
        //                                                g => new
        //                                                {
        //                                                    Key = g.Key,
        //                                                    Value = g.Sum(s => s.DispatchQuantity),
        //                                                    Name = g.First().ItemName,
        //                                                    ItemID = g.First().ItemID,
        //                                                    Packing= g.First().Packing,
        //                                                    Quantity = g.Sum(s => s.DispatchQuantity),
        //                                                    Price = g.First().ItemPrice,
        //                                                });
        //                            DBOY = db.DelieveryBoys.Where(x => x.ID == newRetailer.RegionID).Select(x=>x.Name).FirstOrDefault();
        //                            // Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new LocalReport();
        //                            ReportViewer reportViewer = new ReportViewer();
        //                            //var DealerName = db.JobsDetails.Where(x => x.JobID == Orderid).Select(x => x.RetailerID).FirstOrDefault();

        //                            //var ShopName = dbContext.Retailers.Where(x => x.ID == DealerName).FirstOrDefault();

        //                            ReportParameter[] prm = new ReportParameter[4];


        //                            prm[0] = new ReportParameter("DBoy", DBOY);
        //                            prm[1] = new ReportParameter("SOName", SOName);
        //                            prm[2] = new ReportParameter("DateFrom", StartingDate1);
        //                            prm[3] = new ReportParameter("DateTo", StartingDate1);
        //                            reportViewer.ProcessingMode = ProcessingMode.Local;
        //                            reportViewer.LocalReport.ReportPath = Server.MapPath("~\\Views\\Reports\\DispatchForVan.rdlc");
        //                            reportViewer.LocalReport.EnableExternalImages = true;
        //                            ReportDataSource dt1 = new ReportDataSource("DataSet1", data);

        //                            reportViewer.LocalReport.SetParameters(prm);
        //                            reportViewer.LocalReport.DataSources.Clear();
        //                            reportViewer.LocalReport.DataSources.Add(dt1);
        //                            reportViewer.LocalReport.Refresh();
        //                            // PrintReport.PrintToPrinter(ReportViewer1);

        //                            Warning[] warnings;
        //                            string[] streamids;
        //                            string mimeType, encoding, filenameExtension;

        //                            byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

        //                            //File  
        //                            string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
        //                            string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

        //                            //create and set PdfReader  
        //                            PdfReader reader = new PdfReader(bytes);
        //                            FileStream output = new FileStream(FilePath, FileMode.Create);

        //                            string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

        //                            //create and set PdfStamper  
        //                            PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

        //                            if (Agent.Contains("Firefox"))
        //                                pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
        //                            else
        //                                pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

        //                            pdfStamper.FormFlattening = false;
        //                            pdfStamper.Close();
        //                            reader.Close();

        //                            //return file path  
        //                            FilePathReturn = @"TempFiles/" + FileName;

        //                        }

        //                        catch (Exception exp)
        //                        {
        //                            Log.Instance.Error(exp, "Report Not Working");

        //                        }


        //                        return Content(FilePathReturn);
        //                    }
        //                    else if (Res == "3")
        //                    {
        //                        return Content("3");
        //                    }
        //                    else if (Res == "4")
        //                    {
        //                        return Content("4");
        //                    }
        //                    else if (Res == "5")
        //                    {
        //                        return Content("5");
        //                    }
        //                    else
        //                    {
        //                        return Content("0");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                IList<ValidationFailure> failures = results.Errors;
        //                StringBuilder sb = new StringBuilder();
        //                sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
        //                foreach (ValidationFailure failer in results.Errors)
        //                {
        //                    sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
        //                    Response.StatusCode = 422;
        //                    return Json(new { errors = sb.ToString() });
        //                }
        //            }

        //        }
        //        return Content("0");
        //    }
        //    catch (Exception exp)
        //    {
        //        return Content("Exception : " + exp.Message);
        //    }
        //}


        //public JsonResult DispatchInVanDataHandler(DTParameters param)
        //{
        //    try
        //    {
        //        var dtsource = new List<JobsDetailData>();


        //        dtsource = ManageJobs.GetDispatchForvanForGrid(param.StartingDate1, param.StartingDate2, param.BoyID);


        //        List<String> columnSearch = new List<String>();

        //        foreach (var col in param.Columns)
        //        {
        //            columnSearch.Add(col.Search.Value);
        //        }

        //        List<JobsDetailData> data = ManageJobs.GetResult12(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);
        //        foreach (var itm in data)
        //        {
        //            if (itm.Dispatchdate.HasValue)
        //            {

        //                itm.VisitDateFormatted = Convert.ToDateTime(itm.Dispatchdate).ToString("dd-MM-yyyy");
        //                itm.InvoicedateFormatted = Convert.ToDateTime(itm.VisitDate).ToString("dd-MM-yyyy");
        //            }


        //        }
        //        int count = ManageJobs.Count12(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
        //        DTResult<JobsDetailData> result = new DTResult<JobsDetailData>
        //        {
        //            draw = param.Draw,
        //            data = data,
        //            recordsFiltered = count,
        //            recordsTotal = count
        //        };
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}


        #endregion



        #region DEALERTransfer


        public ActionResult TransferDealer()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetSaleOfficerListByRegionalHeadID(regId);
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<RegionData> RegionObj = FOS.Setup.ManageCity.GetRegionList();
            var objregion = RegionObj.FirstOrDefault();
            IEnumerable<RegionData> Ranges = FOS.Setup.ManageRegion.GetRanges(1);
            var objrange = Ranges.FirstOrDefault();
            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            List<DealerData> DealerObjs = ManageDealer.GetDistributors(objregion.RegionID, objrange.RegionID);
            var objRetailer = new RetailerData();
            objRetailer.Ranges = FOS.Setup.ManageRegion.GetRanges(1);
            objRetailer.RegionalHead = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Dealers = DealerObjs;
            objRetailer.Regions = RegionObj;
            var IDs = objRetailer.Regions.FirstOrDefault();
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Banks = ManageRetailer.GetBanks();

            return View(objRetailer);
        }



        public ActionResult NewUpdateDistributorTransfer([Bind(Exclude = "TID,SaleOfficers,Dealers")] RetailerData newRetailer)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                    if (newRetailer.ID == 0)
                    {
                        RetailerValidator validator = new RetailerValidator();
                        results = validator.Validate(newRetailer);
                        boolFlag = results.IsValid;
                    }

                   
                    if (boolFlag)
                    {
                        
                        int Res = ManageDealer.AddUpdateDistributorTransfer(newRetailer);

                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else if (Res == 4)
                        {
                            return Content("4");
                        }
                        else if (Res == 5)
                        {
                            return Content("5");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }

                }
                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }



        public ActionResult TransferRetailers()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetSaleOfficerListByRegionalHeadID(regId);
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<RegionData> RegionObj = FOS.Setup.ManageCity.GetRegionList();
            var objregion = RegionObj.FirstOrDefault();
            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);
            var Ranges = FOS.Setup.ManageRegion.GetRanges(1);
            var objrange = Ranges.FirstOrDefault();
            List<DealerData> DealerObjs = ManageDealer.GetDistributorsForRetailerTransfer(objregion.RegionID,objrange.RegionID);
            var objRetailer = new RetailerData();
            objRetailer.Ranges = FOS.Setup.ManageRegion.GetRanges(1);
            objRetailer.RegionalHead = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Dealers = DealerObjs;
            objRetailer.DealersTo = DealerObjs;
            objRetailer.Regions = RegionObj;
            var IDs = objRetailer.Regions.FirstOrDefault();
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Banks = ManageRetailer.GetBanks();

            return View(objRetailer);
        }



        public ActionResult NewUpdateTransferRetailers([Bind(Exclude = "TID,SaleOfficers,Dealers")] RetailerData newRetailer)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                    if (newRetailer.ID == 0)
                    {
                        RetailerValidator validator = new RetailerValidator();
                        results = validator.Validate(newRetailer);
                        boolFlag = results.IsValid;
                    }


                    if (boolFlag)
                    {

                        int Res = ManageDealer.AddUpdateRETAILERTransfer(newRetailer);

                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else if (Res == 4)
                        {
                            return Content("4");
                        }
                        else if (Res == 5)
                        {
                            return Content("5");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }

                }
                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }





        #endregion DEALER



        #region DealerDSR

        [CustomAuthorize]
        public ActionResult DistributorDSR()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
          
           



            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var data = db.Users.Where(x => x.ID == userID).FirstOrDefault();
            var Dealer= db.Dealers.Where(x=>x.ID==data.DealerRefNo).Select(
                                    u => new DealerData
                                    {
                                        ID = u.ID,
                                        Name = u.ShopName,
                                        rangeID=u.RangeID

                                    }).ToList();


            var dealerid = Dealer.FirstOrDefault();
           
                List<SaleOfficerData> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoDealerDSR(dealerid.ID, dealerid.rangeID);
          
          

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficers = SaleOfficerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Dealer = Dealer;
            objJob.Range = ranges;


            return View(objJob);
        }


        public JsonResult DSRDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<JobsDetailData>();

              
                dtsource = ManageJobs.GetDSRDealerForGrid(param.StartingDate1, param.StartingDate2, param.ZoneID, param.SOID);
              

                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<JobsDetailData> data = ManageJobs.GetResult12(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);
                foreach (var itm in data)
                {
                    if (itm.AssignDate.HasValue)
                    {

                        itm.VisitDateFormatted = Convert.ToDateTime(itm.AssignDate).ToString("dd-MM-yyyy");
                    }


                }
                int count = ManageJobs.Count12(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
                DTResult<JobsDetailData> result = new DTResult<JobsDetailData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }



        public JsonResult SubmitDealerDSR(string cont, SchemeData tas)
        {

            try
            {
                var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);
                FOSDataModel dbContext = new FOSDataModel();
                ValidationResult results = new ValidationResult();
                if (serialize != null)
                {
                    DealerDSRDispatch ms = new DealerDSRDispatch();
                    
                  
                  
                        foreach (var items in serialize)
                        {
                        ms.ItemID = items.ItemId;
                        ms.ItemName = items.ItemName;
                        ms.JobID = items.OrderID;
                        ms.OrderQuantity = items.OrderedQuan;
                        if (items.Scheme == "")
                        {
                            ms.DispatchQuantity = items.OrderedQuan;
                        }
                        else
                        {
                            ms.DispatchQuantity = Convert.ToInt32(items.Scheme);
                        }
                       
                        ms.Createddate = DateTime.UtcNow.AddHours(5);
                        dbContext.DealerDSRDispatches.Add(ms);
                        dbContext.SaveChanges();
                    }
                    
                }
                return Json("1");
            }
            catch (Exception ex)
            {
                return Json("2");
            }


        }



        //public JsonResult ReprintInvoice(int OrderID)
        //{
        //    decimal Scheme = 0;
        //    var schemeData = new TblDetailScheme();
        //    var userID = Convert.ToInt32(Session["UserID"]);
        //    var DealerID = db.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
        //    var Orderid = 0;
        //    var ParentData = db.DealerDispatchCalculations.Where(x => x.OrderID == OrderID).FirstOrDefault();
        //    var ChildData = db.DealerDSRDispatches.Where(x => x.JobID == OrderID).ToList();
        //    var rangeid = db.Dealers.Where(x => x.ID == DealerID && x.IsActive == true).FirstOrDefault();
        //    var masterID = db.TblMasterSchemes.Where(x => x.rangeID == rangeid.RangeID && x.isActive == true).FirstOrDefault();
        //    List<Items> Itemdata = new List<Items>();
        //    Items cty;
        //    string FilePathReturn = "";

        //    foreach (var items in ChildData)
        //    {
        //        Orderid = (int)items.JobID;
        //        cty = new Items();
        //        cty.ItemID = (int) items.ItemID;
        //        var data = db.Items.Where(x => x.ItemID == items.ItemID).FirstOrDefault();
        //        if (masterID != null)
        //        {
        //            schemeData = db.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
        //        }
        //        cty.ItemName = items.ItemName;
        //        cty.OrderID =(int) items.JobID;
        //        cty.Packing = data.Packing;
        //        if (schemeData.SchemePrice != null)
        //        {
        //            cty.Slab = "RS" + schemeData.SchemePrice + "/" + data.Packing;

        //        }
        //        else
        //        {
        //            cty.Slab = "0";
        //            cty.SchemeValue = 0;
        //        }
        //        cty.Rate = data.Price;


        //        cty.OrderedQuan = (int) items.OrderQuantity;
               
        //            cty.DispatchQuantity = items.DispatchQuantity;
        //            cty.Value = data.Price * items.OrderQuantity;
        //        cty.Amount = data.Price * items.DispatchQuantity;
        //        if (schemeData.SchemePrice != 0)
        //            {
        //                double val = Convert.ToDouble( items.OrderQuantity / data.Packing);
        //                int rounded = (int)Math.Round(val);
        //                cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                Scheme += Convert.ToDecimal(cty.SchemeValue);
        //            }
        //            else
        //            {
        //                cty.Slab = "0";
        //                cty.SchemeValue = 0;

        //            }
               
              

        //        cty.Createddate = DateTime.UtcNow.AddHours(5);
        //        Itemdata.Add(cty);
        //    }

        //    try
        //    {
        //        // Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new LocalReport();
        //        ReportViewer reportViewer = new ReportViewer();
        //        var DealerName = db.JobsDetails.Where(x => x.JobID == Orderid).FirstOrDefault();

        //        var ShopName = db.Retailers.Where(x => x.ID == DealerName.RetailerID).FirstOrDefault();
        //        var SOName = db.SaleOfficers.Where(x => x.ID == DealerName.SalesOficerID).Select(x => x.Name).FirstOrDefault();

        //        ReportParameter[] prm = new ReportParameter[12];


        //        prm[0] = new ReportParameter("DealerName", rangeid.ShopName + "/" + rangeid.DealerCode);
        //        prm[1] = new ReportParameter("SOName", SOName);
        //        prm[2] = new ReportParameter("ShopName", ShopName.ShopName);
        //        prm[3] = new ReportParameter("GrossAmount", ParentData.GrossAmount.ToString());
        //        prm[4] = new ReportParameter("Display", ParentData.Display.ToString());
        //        prm[5] = new ReportParameter("Balance1", ParentData.Balanceamount1.ToString());
        //        prm[6] = new ReportParameter("Scheme", ParentData.Scheme.ToString());
        //        prm[7] = new ReportParameter("Balance2", ParentData.Balanceamount2.ToString());
        //        prm[8] = new ReportParameter("Discount", ParentData.WSDiscount.ToString());
        //        prm[9] = new ReportParameter("NetAmount", ParentData.NetAmount.ToString());
        //        prm[10] = new ReportParameter("Address", ShopName.Address);
        //        prm[11] = new ReportParameter("BillNo", ParentData.BillNo);
        //        reportViewer.ProcessingMode = ProcessingMode.Local;
        //        reportViewer.LocalReport.ReportPath = Server.MapPath("~\\Views\\Reports\\DealerDSR.rdlc");
        //        reportViewer.LocalReport.EnableExternalImages = true;
        //        ReportDataSource dt1 = new ReportDataSource("DataSet1", Itemdata);

        //        reportViewer.LocalReport.SetParameters(prm);
        //        reportViewer.LocalReport.DataSources.Clear();
        //        reportViewer.LocalReport.DataSources.Add(dt1);
        //        reportViewer.LocalReport.Refresh();
        //        // PrintReport.PrintToPrinter(ReportViewer1);

        //        Warning[] warnings;
        //        string[] streamids;
        //        string mimeType, encoding, filenameExtension;

        //        byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

        //        //File  
        //        string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
        //        string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

        //        //create and set PdfReader  
        //        PdfReader reader = new PdfReader(bytes);
        //        FileStream output = new FileStream(FilePath, FileMode.Create);

        //        string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

        //        //create and set PdfStamper  
        //        PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

        //        if (Agent.Contains("Firefox"))
        //            pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
        //        else
        //            pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

        //        pdfStamper.FormFlattening = false;
        //        pdfStamper.Close();
        //        reader.Close();

        //        //return file path  
        //        FilePathReturn = @"TempFiles/" + FileName;

        //    }

        //    catch (Exception exp)
        //    {
        //        Log.Instance.Error(exp, "Report Not Working");

        //    }

        //    return Json(FilePathReturn);
        //}

        #endregion DealerDSR

     

    }
}