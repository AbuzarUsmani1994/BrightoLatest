using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Imaging;
using FoS.Web.UI.Report;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Web.Http.Results;

namespace FOS.Web.UI.Controllers
{
    public class SetupController : Controller
    {
        FOSDataModel dbContext = new FOSDataModel();
        #region SchemeInformation
        public JsonResult GetEditScheme(int SchemeID)
        {
            var Response = ManageScheme.GetEditScheme(SchemeID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public int DeleteScheme(int schemeID)
        {
            return FOS.Setup.ManageScheme.DeleteScheme(schemeID);
        }
        public JsonResult SchemeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<SchemeData>();

                dtsource = ManageScheme.GetSchemesForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SchemeData> data = ManageScheme.GetSchemeResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageScheme.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<SchemeData> result = new DTResult<SchemeData>
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
        [CustomAuthorize]
        //View Work...
        public ActionResult SchemeInformation()
        {
            return View();
        }
        public JsonResult AddUpdateScheme(SchemeData tas)
        {

            try
            {
                //var serialize = JsonConvert.DeserializeObject<List<TblMasterScheme>>(cont);
                FOSDataModel dbContext = new FOSDataModel();
                ValidationResult results = new ValidationResult();
                //if (serialize != null)
                //{
                TblMasterScheme ms = new TblMasterScheme();
                if (tas.SchemeID == 0)
                {
                    //ms.RangeID = tas.RangeID;
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    ms.isActive = true;
                    TblMasterScheme ms2 = dbContext.TblMasterSchemes.Where(x => x.isActive == true).FirstOrDefault();
                    if (ms2 != null)
                    {
                        ms2.isActive = false;
                    }
                    dbContext.TblMasterSchemes.Add(ms);
                    dbContext.SaveChanges();
                }
                else
                {
                    ms = dbContext.TblMasterSchemes.Where(u => u.MasterSchemeID == tas.SchemeID).FirstOrDefault();
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    ms.isActive = true;
                    TblMasterScheme ms2 = dbContext.TblMasterSchemes.Where(x => x.isActive == true).FirstOrDefault();
                    if (ms2 != null)
                    {
                        ms2.isActive = false;
                    }
                    dbContext.SaveChanges();
                }
                //}
                return Json("0");
            }
            catch (Exception ex)
            {
                return Json("1");
            }


        }
        #endregion

        #region REGION

        [CustomAuthorize]
        //View Work...
        public ActionResult Region()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateRegion([Bind(Exclude = "TID")] RegionData newRegion)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.RegionID == 0)
                    {
                        RegionValidator validator = new RegionValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageRegion.AddUpdateRegion(newRegion);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult RegionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<RegionData>();

                dtsource = ManageRegion.GetRegionForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionData> data = ManageRegion.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionData> result = new DTResult<RegionData>
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
        public int DeleteRegion(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteRegion(RegionID);
        }

        #endregion REGION

        #region CITY

        [CustomAuthorize]
        //View Work...
        public ActionResult City()
        {
            // Load Region Data For City Records ...
            var objCity = new CityData();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            int THID = FOS.Web.UI.Controllers.AdminPanelController.GetTHIDRelatedToUser();
            if (THID > 0)
            {

                objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(THID);
            }
            else
            {
                objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
            }



            return View(objCity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateCity([Bind(Exclude = "TID")] CityData newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newCity != null)
                    {
                        if (newCity.ID == 0)
                        {
                            CityValidator validator = new CityValidator();
                            results = validator.Validate(newCity);
                            boolFlag = results.IsValid;
                        }

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddUpdateCity(newCity);
                            if (Response == 1)
                            {
                                return Content("1");
                            }
                            else if (Response == 2)
                            {
                                return Content("2");
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
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult CityDataHandler(DTParameters param, Int32 RegionID)
        {
            try
            {
                var dtsource = new List<CityData>();

                dtsource = ManageCity.GetCityForGrid(RegionID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<CityData> data = ManageCity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageCity.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<CityData> result = new DTResult<CityData>
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
        public int DeleteCity(int cityID)
        {
            return FOS.Setup.ManageCity.DeleteCity(cityID);
        }

        // Get One City For Edit
        public JsonResult GetEditCity(int CityID)
        {
            var Response = ManageCity.GetEditCity(CityID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        #endregion CITY

        #region AREA

        [CustomAuthorize]
        // View ...
        public ActionResult Area()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            var objRegion = RegionObj.FirstOrDefault();
            List<CityData> CityObj = FOS.Setup.ManageCity.GetCityListByRegionID(objRegion.ID);
            ViewData["CityObj"] = CityObj;

            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.Cities = CityObj;

            return View(objArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateArea([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateArea(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult AreaDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetAreaForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetCityListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionID(RegionID);
            return Json(result);
        }


        public JsonResult GetSOListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
            return Json(result);
        }


        //Delete Region...
        public int DeleteArea(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteArea(areaID);
        }

        #endregion AREA


        #region KPIS

        [CustomAuthorize]
        //View Work...
        public ActionResult KPI()
        {
            // Load Region Data For City Records ...
            var objCity = new CityData();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            int THID = FOS.Web.UI.Controllers.AdminPanelController.GetTHIDRelatedToUser();
            if (THID > 0)
            {

                objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(THID);
            }
            else
            {
                objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
            }



            return View(objCity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateKPIS([Bind(Exclude = "TID")] CityData newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
               
                    if (newCity != null)
                    {
                      

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddKPIS(newCity);
                            if (Response == 1)
                            {
                                return Content("1");
                            }
                            else if (Response == 2)
                            {
                                return Content("2");
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

        //Get All Region Method...
        public JsonResult KPIDataHandler(DTParameters param, Int32 RegionID)
        {
            try
            {
                var dtsource = new List<CityData>();

                dtsource = ManageCity.GetKPIForGrid(RegionID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<CityData> data = ManageCity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);

                foreach (var itm in data)
                {
                    if (itm.LastUpdate.HasValue)
                    {

                        itm.FormattedDate = Convert.ToDateTime(itm.LastUpdate).ToString("dd-MM-yyyy");
                        
                    }


                }

                int count = ManageCity.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<CityData> result = new DTResult<CityData>
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
        //public int DeleteCity(int cityID)
        //{
        //    return FOS.Setup.ManageCity.DeleteCity(cityID);
        //}

        //// Get One City For Edit
        //public JsonResult GetEditCity(int CityID)
        //{
        //    var Response = ManageCity.GetEditCity(CityID);
        //    return Json(Response, JsonRequestBehavior.AllowGet);
        //}

        #endregion KPIS


        #region AttendanceAndPunc

        [CustomAuthorize]
        // View ...
        public ActionResult AttendanceAndPunctuality()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            // List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var objRegion = regionalHeadData.FirstOrDefault();
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(objRegion.ID, true);



            var objArea = new AreaData();
            objArea.RegionalHead = regionalHeadData;
            objArea.Salesofficerdata = SaleOfficerObj;

            return View(objArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttendanceAndPunctuality([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateAttendance(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult AttendanceAndPunctualityDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetAttendanceForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetAttendanceListByID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetAttendanceListByRegionID(RegionID);
            return Json(result);
        }


        //public JsonResult GetSOListByRegionID(int RegionID)
        //{
        //    var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
        //    return Json(result);
        //}


        ////Delete Region...
        //public int DeleteArea(int areaID)
        //{
        //    return FOS.Setup.ManageArea.DeleteArea(areaID);
        //}

        #endregion AttendanceAndPunc


        #region Training

        [CustomAuthorize]
        // View ...
        public ActionResult Training()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            // List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var objRegion = regionalHeadData.FirstOrDefault();
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(objRegion.ID, true);



            var objArea = new AreaData();
            objArea.RegionalHead = regionalHeadData;
            objArea.Salesofficerdata = SaleOfficerObj;

            return View(objArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTraining([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateTraining(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult AddTrainingDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetTrainingForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetTrainingListByID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GettrainingListByRegionID(RegionID);
            return Json(result);
        }


        //public JsonResult GetSOListByRegionID(int RegionID)
        //{
        //    var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
        //    return Json(result);
        //}


        ////Delete Region...
        //public int DeleteArea(int areaID)
        //{
        //    return FOS.Setup.ManageArea.DeleteArea(areaID);
        //}

        #endregion AttendanceAndPunc





        //#region CITY

        //[CustomAuthorize]
        ////View Work...
        //public ActionResult Zone()
        //{
        //    // Load Region Data For City Records ...
        //    var objCity = new ZonesData();
        //    int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
        //    objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
        //    objCity.Zones = FOS.Setup.ManageRegion.GetZones();
        //    return View(objCity);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddUpdateZone([Bind(Exclude = "TID")] ZonesData newCity)
        //{
        //    Boolean boolFlag = true;
        //    ValidationResult results = new ValidationResult();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (newCity != null)
        //            {
        //                if (newCity.ID == 0)
        //                {
        //                    zoneValidator validator = new zoneValidator();
        //                    results = validator.Validate(newCity);
        //                    boolFlag = results.IsValid;
        //                }

        //                if (boolFlag)
        //                {
        //                    int Response = ManageCity.AddUpdateZones(newCity);
        //                    if (Response == 1)
        //                    {
        //                        return Content("1");
        //                    }
        //                    else if (Response == 2)
        //                    {
        //                        return Content("2");
        //                    }
        //                    else
        //                    {
        //                        return Content("0");
        //                    }
        //                }
        //                else
        //                {
        //                    IList<ValidationFailure> failures = results.Errors;
        //                    StringBuilder sb = new StringBuilder();
        //                    sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
        //                    foreach (ValidationFailure failer in results.Errors)
        //                    {
        //                        sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
        //                        Response.StatusCode = 422;
        //                        return Json(new { errors = sb.ToString() });
        //                    }
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

        ////Get All Region Method...
        //public JsonResult ZoneDataHandler(DTParameters param, Int32 RegionID)
        //{
        //    try
        //    {
        //        var dtsource = new List<ZonesData>();

        //        dtsource = ManageCity.GetZonesForGrid(RegionID);

        //        List<String> columnSearch = new List<string>();

        //        foreach (var col in param.Columns)
        //        {
        //            columnSearch.Add(col.Search.Value);
        //        }

        //        List<ZonesData> data = ManageCity.GetResult4(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
        //        int count = ManageCity.Count4(param.Search.Value, dtsource, columnSearch);
        //        DTResult<ZonesData> result = new DTResult<ZonesData>
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

        ////Delete Region Method...
        //public int DeleteZone(int cityID)
        //{
        //    return FOS.Setup.ManageCity.DeleteCity(cityID);
        //}

        //// Get One City For Edit
        //public JsonResult GetEditZone(int CityID)
        //{
        //    var Response = ManageCity.GetEditCity(CityID);
        //    return Json(Response, JsonRequestBehavior.AllowGet);
        //}

        //#endregion CITY


        #region ActivityPurpose

        [CustomAuthorize]
        //View Work...
        public ActionResult ActivityPurpose()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateActivityPurpose([Bind(Exclude = "TID")] PurposeOfActivityData newRegion)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.ID == 0)
                    {
                        ActivityPurposeValidator validator = new ActivityPurposeValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageRegion.AddUpdateActivityPurpose(newRegion);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult ActivityPurposeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<PurposeOfActivityData>();

                dtsource = ManageRegion.GetActivityPurposeForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<PurposeOfActivityData> data = ManageRegion.GetResult5(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count5(param.Search.Value, dtsource, columnSearch);
                DTResult<PurposeOfActivityData> result = new DTResult<PurposeOfActivityData>
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
        public int DeleteActivityPurpose(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteActivityPurpose(RegionID);
        }

        #endregion ActivityPurpose

        #region AccessGrid

        public JsonResult AccessDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetAccessForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
                DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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


        public void AccessDataHandler1( Int32 HeadID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetAccessForGrid(HeadID);

                List<String> columnSearch = new List<string>();


                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNO \",\"Head Name\",\"SaleOfficer Name\",\"Reported To\",\"Reported For\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=ReportingheirarchyRpt" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                int srNo = 1;

                foreach (var retailer in dtsource)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",

                        srNo,
                     retailer.RegionName,
                      retailer.SaleOfficerName,
                    retailer.ReportedToName,
                      retailer.ReportedForName,

                    srNo++


                    ));
                }





                Response.Write(sw.ToString());
                Response.End();



         




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region SOREGIONSGrid

        public JsonResult SORegionsDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetSORegionsForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
                DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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


        #endregion SOREGIONSGrid


        #endregion ActivityPurpose


        [CustomAuthorize]
        public ActionResult SchemeInfo()
        {
            FOSDataModel db = new FOSDataModel();
            var objSaleOffice = new SchemeData();
            List<CategoryData> catData = new List<CategoryData>();
            catData = ManageCategory.GetCat();
            catData.Insert(0, new CategoryData
            {
                MainCategID = 0,
                MainCategDesc = "Select Range"
            });
            objSaleOffice.RangeData = catData;
            return View(objSaleOffice);

        }

        public JsonResult ItemDataHandler(DTParameters param, int? RangeID)
        {
            try
            {
                var dtsource = new List<Items>();

                dtsource = ManageItem.GetItemList(RangeID);


                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        //public JsonResult SubmitItem(string cont, SchemeData tas)
        //{

        //    try
        //    {
        //        var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);
        //        FOSDataModel dbContext = new FOSDataModel();
        //        ValidationResult results = new ValidationResult();
        //        if (serialize != null)
        //        {
        //            var Schemeinfos = dbContext.TblMasterSchemes.Where(x => x.rangeID == tas.RangeID).ToList();
        //            foreach (var item in Schemeinfos)
        //            {
        //                item.isActive = false;
        //                item.isDeleted = true;
        //                dbContext.SaveChanges();
        //            }

        //            TblMasterScheme ms = new TblMasterScheme();
        //            TblDetailScheme ds = new TblDetailScheme();
        //            ms.rangeID = tas.RangeID;
        //            ms.SchemeDateFrom = tas.SchemeDateFrom;
        //            ms.SchemeDateTo = tas.SchemeDateTo;
        //            ms.SchemeInfo = tas.SchemeInfo;
        //            ms.isActive = true;
        //            ms.isDeleted = false;
        //            dbContext.TblMasterSchemes.Add(ms);
        //            dbContext.SaveChanges();
        //            ms.MasterSchemeID = dbContext.TblMasterSchemes.OrderByDescending(u => u.MasterSchemeID).Select(u => u.MasterSchemeID).FirstOrDefault();
        //            if (ms.MasterSchemeID > 0)
        //            {
        //                foreach (var items in serialize)
        //                {
        //                    ds.ItemID = items.ItemID;
        //                    ds.ItemName = items.ItemName;
        //                    ds.Packing = items.ItemPacking.ToString();
        //                    ds.TradePrice = items.ItemPrice.ToString();
        //                    ds.Scheme = items.Scheme;
        //                    if (items.SchemePrice == "")
        //                    {
        //                        ds.SchemePrice = 0;
        //                    }
        //                    else
        //                    {
        //                        ds.SchemePrice = Convert.ToInt32(items.SchemePrice);
        //                    }

        //                    ds.MasterID = ms.MasterSchemeID;
        //                    dbContext.TblDetailSchemes.Add(ds);
        //                    dbContext.SaveChanges();
        //                }
        //            }
        //        }
        //        return Json("1");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("2");
        //    }


        //}


        //public JsonResult SubmitDealerDSRPrint(string cont, SchemeData tas)
        //{
        //    DateTime start = DateTime.Now;
        //    DateTime end= DateTime.Now;
        //    DateTime final= DateTime.Now;
        //    if (tas.SchemeDateFrom != null && tas.SchemeDateTo != null)
        //    {
        //         start = tas.SchemeDateFrom;
        //         end   = tas.SchemeDateTo;
        //         final = end.AddDays(1);
        //    }
        //    else
        //    {
        //        start = DateTime.UtcNow.AddHours(5);

        //        end = start.Date;
        //        final = end.AddDays(1);
        //    }



        //    FOSDataModel dbContext = new FOSDataModel();
        //    var rangeid = dbContext.Dealers.Where(x => x.ShopName == tas.DealerName && x.IsActive == true).FirstOrDefault();
        //    string FilePathReturn = "";
        //    string BillNo = "";
        //    var schemeData = new TblDetailScheme();
        //    var editdata = new DealerDispatchCalculation();
        //    var counter = dbContext.DealerDispatchCalculations.Where(x => x.DealerID==rangeid.ID).OrderByDescending(u => u.ID).Select(u => u.BillNo).FirstOrDefault();


        //    if (counter == null)
        //    {   var datein= DateTime.Now.Year.ToString();
        //        var ticketCount = 1;
        //        string s = ticketCount.ToString().PadLeft(3, '0');
        //        BillNo = rangeid.DealerCode+"-"+ datein  + "-" + s;
        //    }
        //    else
        //    {
        //        var datein = DateTime.Now.Year.ToString();
        //        var splittedcounter = counter.Split('-');
        //        var val = splittedcounter[2];
        //        int value = Convert.ToInt32(val) + 1;
        //        string s = value.ToString().PadLeft(3, '0');
        //        BillNo = rangeid.DealerCode + "-" + datein + "-" + s;
        //    }
        //    List<Items> Itemdata = new List<Items>();
        //    DealerDSRDispatch ms = new DealerDSRDispatch();
        //    DealerDispatchCalculation cal = new DealerDispatchCalculation();
        //    Items cty;
        //    var Orderid = 0;
        //    decimal GrossAmount = 0;
        //    decimal Scheme = 0;
        //    var masterID = dbContext.TblMasterSchemes.Where(x => x.rangeID == rangeid.RangeID && x.isActive == true).FirstOrDefault();
        //    try
        //    {
        //        var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);

        //        var OrderIdData = serialize.Select(x => x.OrderID).FirstOrDefault();
        //        editdata = dbContext.DealerDispatchCalculations.Where(x => x.OrderID == OrderIdData).FirstOrDefault();
        //        if (editdata == null)
        //        {
        //            ValidationResult results = new ValidationResult();
        //            if (serialize != null)
        //            {

        //                foreach (var items in serialize)
        //                {
        //                    Orderid = items.OrderID;
        //                    cty = new Items();
        //                    ms.ItemID = items.ItemID;
        //                    ms.ItemName = items.ItemName;
        //                    ms.JobID = items.OrderID;
        //                    ms.OrderQuantity = items.OrderedQuan;
        //                    ms.SOID = tas.SOID;
        //                    ms.FromDate = start;
        //                    ms.ToDate = final;
        //                    if (items.Scheme == "")
        //                    {
        //                        ms.DispatchQuantity = items.OrderedQuan;
        //                    }
        //                    else
        //                    {
        //                        ms.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                    }

        //                    ms.Createddate = DateTime.UtcNow.AddHours(5);

        //                    cty.ItemID = items.ItemID;
        //                    var data = dbContext.Items.Where(x => x.ItemID == items.ItemID).FirstOrDefault();
        //                    if (masterID != null)
        //                    {
        //                        schemeData = dbContext.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
        //                    }
        //                    cty.ItemName = items.ItemName;
        //                    cty.OrderID = items.OrderID;
        //                    cty.Packing = data.Packing;
        //                    if (schemeData.SchemePrice != null)
        //                    {
        //                        cty.Slab = "RS" +schemeData.SchemePrice  + "/" + data.Packing;

        //                    }
        //                    else
        //                    {
        //                        cty.Slab = "0";
        //                        cty.SchemeValue = 0;
        //                    }
        //                    cty.Rate = data.Price;


        //                    cty.OrderedQuan = items.OrderedQuan;
        //                    if (items.Scheme == "")
        //                    {
        //                        cty.DispatchQuantity = items.OrderedQuan;
        //                        cty.Value = data.Price * items.OrderedQuan;
        //                        cty.Amount = data.Price * items.OrderedQuan;
        //                        GrossAmount += data.Price * items.OrderedQuan;
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = items.OrderedQuan / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;

        //                        }
        //                    }
        //                    else
        //                    {
        //                        cty.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                        cty.Value = data.Price * Convert.ToInt32(items.Scheme);
        //                        cty.Amount = data.Price * Convert.ToInt32(items.Scheme);
        //                        GrossAmount += data.Price * Convert.ToInt32(items.Scheme);
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = Convert.ToInt32(items.Scheme) / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;
        //                        }
        //                    }

        //                    cty.Createddate = DateTime.UtcNow.AddHours(5);
        //                    Itemdata.Add(cty);
        //                    dbContext.DealerDSRDispatches.Add(ms);
        //                    dbContext.SaveChanges();


        //                }
        //            }
        //        }
        //        else
        //        {
        //            DealerDispatchCalculation obj = dbContext.DealerDispatchCalculations.Where(u => u.OrderID == OrderIdData).FirstOrDefault();
        //            dbContext.DealerDispatchCalculations.Remove(obj);
        //            var obj2 = dbContext.DealerDSRDispatches.Where(u => u.JobID == OrderIdData).ToList();
        //            foreach (var item in obj2)
        //            {
        //                dbContext.DealerDSRDispatches.Remove(item);
        //            }

        //            if (serialize != null)
        //            {

        //                foreach (var items in serialize)
        //                {
        //                    Orderid = items.OrderID;
        //                    cty = new Items();
        //                    ms.ItemID = items.ItemID;
        //                    ms.ItemName = items.ItemName;
        //                    ms.JobID = items.OrderID;
        //                    ms.OrderQuantity = items.OrderedQuan;
        //                    if (items.Scheme == "")
        //                    {
        //                        ms.DispatchQuantity = items.OrderedQuan;
        //                    }
        //                    else
        //                    {
        //                        ms.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                    }

        //                    ms.Createddate = DateTime.UtcNow.AddHours(5);
        //                    ms.SOID = tas.SOID;
        //                    cty.ItemID = items.ItemID;
        //                    var data = dbContext.Items.Where(x => x.ItemID == items.ItemID).FirstOrDefault();
        //                    if (masterID != null)
        //                    {
        //                        schemeData = dbContext.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
        //                    }
        //                    cty.ItemName = items.ItemName;
        //                    cty.OrderID = items.OrderID;
        //                    cty.Packing = data.Packing;
        //                    if (schemeData.SchemePrice != null)
        //                    {
        //                        cty.Slab = schemeData.SchemePrice + "RS" + "/" + data.Packing;

        //                    }
        //                    else
        //                    {
        //                        cty.Slab = "0";
        //                        cty.SchemeValue = 0;
        //                    }
        //                    cty.Rate = data.Price;


        //                    cty.OrderedQuan = items.OrderedQuan;
        //                    if (items.Scheme == "")
        //                    {
        //                        cty.DispatchQuantity = items.OrderedQuan;
        //                        cty.Value = data.Price * items.OrderedQuan;
        //                        cty.Amount = data.Price * items.OrderedQuan;
        //                        GrossAmount += data.Price * items.OrderedQuan;
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = items.OrderedQuan / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;

        //                        }
        //                    }
        //                    else
        //                    {
        //                        cty.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                        cty.Value = data.Price * Convert.ToInt32(items.Scheme);
        //                        cty.Amount = data.Price * Convert.ToInt32(items.Scheme);
        //                        GrossAmount += data.Price * Convert.ToInt32(items.Scheme);
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = Convert.ToInt32(items.Scheme) / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;
        //                        }
        //                    }

        //                    cty.Createddate = DateTime.UtcNow.AddHours(5);
        //                    Itemdata.Add(cty);
        //                    dbContext.DealerDSRDispatches.Add(ms);
        //                    dbContext.SaveChanges();


        //                }
        //            }


        //        }
        //        cal.GrossAmount = GrossAmount;
        //        var display = GrossAmount * 3 / 100;
                
        //        cal.Display = Math.Round(display, 2);
        //        cal.OrderID = Orderid;
        //        var dis = GrossAmount * 2 / 100;
        //        var balamount1= GrossAmount - display;
        //        cal.Balanceamount1 = Math.Round(balamount1, 2);
        //        var balamo1 = cal.Balanceamount1;
        //        cal.Scheme = Scheme;
               
        //        cal.Balanceamount2 = balamo1 - dis;
        //        var balamo2 = cal.Balanceamount2;
               
        //        cal.WSDiscount = dis;
        //        cal.NetAmount = cal.Balanceamount2 - Scheme;
        //        cal.BillNo = BillNo;
        //        cal.DealerID = rangeid.ID;
        //        var Net = cal.Balanceamount2 - Scheme;
               
        //            dbContext.DealerDispatchCalculations.Add(cal);
        //            dbContext.SaveChanges();
            
        //        var changeStatus = dbContext.JobsDetails.Where(x => x.JobID == Orderid).FirstOrDefault();
        //        changeStatus.Dispatchstatus = "Invoiced";
        //        dbContext.SaveChanges();

        //        try
        //        {
        //            // Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new LocalReport();
        //            ReportViewer reportViewer = new ReportViewer();
        //            var DealerName = dbContext.JobsDetails.Where(x => x.JobID == Orderid).Select(x => x.RetailerID).FirstOrDefault();

        //            var ShopName = dbContext.Retailers.Where(x => x.ID == DealerName).FirstOrDefault();

        //            ReportParameter[] prm = new ReportParameter[12];


        //            prm[0] = new ReportParameter("DealerName", tas.DealerName + "/"+ rangeid.DealerCode );
        //            prm[1] = new ReportParameter("SOName", tas.SOName);
        //            prm[2] = new ReportParameter("ShopName", ShopName.ShopName);
        //            prm[3] = new ReportParameter("GrossAmount", GrossAmount.ToString());
        //            prm[4] = new ReportParameter("Display", display.ToString());
        //            prm[5] = new ReportParameter("Balance1", balamo1.ToString());
        //            prm[6] = new ReportParameter("Scheme", Scheme.ToString());
        //            prm[7] = new ReportParameter("Balance2", balamo2.ToString());
        //            prm[8] = new ReportParameter("Discount", dis.ToString());
        //            prm[9] = new ReportParameter("NetAmount", Net.ToString());
        //            prm[10] = new ReportParameter("Address", ShopName.Address);
        //            prm[11] = new ReportParameter("BillNo", BillNo);
        //            reportViewer.ProcessingMode = ProcessingMode.Local;
        //            reportViewer.LocalReport.ReportPath = Server.MapPath("~\\Views\\Reports\\DealerDSR.rdlc");
        //            reportViewer.LocalReport.EnableExternalImages = true;
        //            ReportDataSource dt1 = new ReportDataSource("DataSet1", Itemdata);

        //            reportViewer.LocalReport.SetParameters(prm);
        //            reportViewer.LocalReport.DataSources.Clear();
        //            reportViewer.LocalReport.DataSources.Add(dt1);
        //            reportViewer.LocalReport.Refresh();
        //            // PrintReport.PrintToPrinter(ReportViewer1);

        //            Warning[] warnings;
        //            string[] streamids;
        //            string mimeType, encoding, filenameExtension;

        //            byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

        //            //File  
        //            string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
        //            string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

        //            //create and set PdfReader  
        //            PdfReader reader = new PdfReader(bytes);
        //            FileStream output = new FileStream(FilePath, FileMode.Create);

        //            string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

        //            //create and set PdfStamper  
        //            PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

        //            if (Agent.Contains("Firefox"))
        //                pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
        //            else
        //                pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

        //            pdfStamper.FormFlattening = false;
        //            pdfStamper.Close();
        //            reader.Close();

        //            //return file path  
        //            FilePathReturn = @"TempFiles/" + FileName;

        //        }

        //        catch (Exception exp)
        //        {
        //            Log.Instance.Error(exp, "Report Not Working");

        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return Json(FilePathReturn);
        //}



        public JsonResult SubmitDealerDSRPrintForAll( SchemeData tas)
        {
            string FilePathReturn = "";
            FOSDataModel dbContext = new FOSDataModel();
            DateTime start = tas.SchemeDateFrom;
            DateTime end = tas.SchemeDateTo;
            DateTime final = end.AddDays(1);

          
                var Sodata = dbContext.Sp_GetAllDataForExportofSO(tas.SOID, start, final).ToList();

                var rangeid = dbContext.Dealers.Where(x => x.ShopName == tas.DealerName && x.IsActive == true).FirstOrDefault();
                
                string BillNo = "";
                var schemeData = new TblDetailScheme();
                var editdata = new DealerDispatchCalculation();

                List<Items> Itemdata = new List<Items>();
                DealerDSRDispatch ms = new DealerDSRDispatch();
                DealerDispatchCalculation cal = new DealerDispatchCalculation();
                //Items cty;
                //var Orderid = 0;
                //decimal GrossAmount = 0;
                //decimal Scheme = 0;
                var masterID = dbContext.TblMasterSchemes.Where(x => x.rangeID == rangeid.RangeID && x.isActive == true).FirstOrDefault();
                try
                {
                    // var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);

                    // var OrderIdData = serialize.Select(x => x.OrderID).FirstOrDefault();
                    //editdata = dbContext.DealerDispatchCalculations.Where(x => x.OrderID == OrderIdData).FirstOrDefault();

                    ValidationResult results = new ValidationResult();


                    foreach (var items in Sodata)
                    {
                    var ID = dbContext.DealerDSRDispatches.Where(x => x.JobID == items.JobID && x.ItemID==items.ItemID).Select(x => x.JobID).ToList();
                    if (ID.Count == 0)
                    {
                        ms.ItemID = items.ItemID;
                        ms.ItemName = items.itemname;
                        ms.JobID = items.JobID;
                        ms.OrderQuantity = Convert.ToInt32(items.quantity);
                        ms.SOID = tas.SOID;

                        ms.DispatchQuantity = Convert.ToInt32(items.quantity);
                        ms.Createddate = DateTime.UtcNow.AddHours(5);

                        ms.FromDate = start;
                        ms.ToDate = final;

                        if (masterID != null)
                        {
                            schemeData = dbContext.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
                        }



                        if (schemeData.SchemePrice != null)
                        {
                            ms.Slab = "RS" + schemeData.SchemePrice + "/" + items.packing;

                        }
                        else
                        {
                            ms.Slab = "0";
                            ms.Schemevalue = 0;
                        }




                        if (schemeData.SchemePrice != 0)
                        {
                            double val = Convert.ToDouble(items.quantity / items.packing);
                            int rounded = (int)Math.Round(val);
                            ms.Schemevalue = rounded * schemeData.SchemePrice;

                        }
                        else
                        {
                            ms.Slab = "0";
                            ms.Schemevalue = 0;

                        }





                        dbContext.DealerDSRDispatches.Add(ms);
                        dbContext.SaveChanges();

                    }
                    }

                    var CalData = dbContext.Sp_GetAllDataForExportofSOCalculations(tas.SOID, start, final).ToList();

                foreach (var cals in CalData)
                {
                    var Jobid = dbContext.DealerDispatchCalculations.Where(x => x.OrderID == cals.jobid).Select(x => x.OrderID).FirstOrDefault();
                    if (Jobid == null)
                    {

                        var counter = dbContext.DealerDispatchCalculations.Where(x => x.DealerID == rangeid.ID).OrderByDescending(u => u.ID).Select(u => u.BillNo).FirstOrDefault();


                        if (counter == null)
                        {
                            var datein = DateTime.Now.Year.ToString();
                            var ticketCount = 1;
                            string s = ticketCount.ToString().PadLeft(3, '0');
                            BillNo = rangeid.DealerCode + "-" + datein + "-" + s;
                        }
                        else
                        {
                            var datein = DateTime.Now.Year.ToString();
                            var splittedcounter = counter.Split('-');
                            var val = splittedcounter[2];
                            int value = Convert.ToInt32(val) + 1;
                            string s = value.ToString().PadLeft(3, '0');
                            BillNo = rangeid.DealerCode + "-" + datein + "-" + s;
                        }
                        cal.GrossAmount = cals.GrossAmount;
                        var display = cals.Display;

                        cal.Display = cals.Display;
                        cal.OrderID = cals.jobid;

                        cal.Balanceamount1 = cals.BalAmount1;



                        cal.Balanceamount2 = cals.balAmount2;
                        cal.WSDiscount = cals.WSDisplay;
                        if (cals.Scheme == null)
                        {
                            cal.Scheme = 0;
                            cal.NetAmount = cal.Balanceamount2 - 0;
                        }
                        else
                        {
                            cal.Scheme = cals.Scheme;
                            cal.NetAmount = cal.Balanceamount2 - cals.Scheme;
                        }

                        cal.BillNo = BillNo;
                        cal.DealerID = rangeid.ID;


                        dbContext.DealerDispatchCalculations.Add(cal);
                        dbContext.SaveChanges();
                        var changeStatus = dbContext.JobsDetails.Where(x => x.JobID == cals.jobid).FirstOrDefault();
                        changeStatus.Dispatchstatus = "Invoiced";
                        dbContext.SaveChanges();
                    }
                }
                }
                catch (Exception ex)
                {

                }
          


            try
            {

                var ItemData = dbContext.Sp_GetAllDataForExportofSOInReport(tas.SOID, start, final).ToList();
                //var ItemData2 = dbContext.Sp_GetAllDataForExportofSOCalculationsForReport(tas.SOID, start, final).ToList();
                // Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new LocalReport();
                ReportViewer reportViewer = new ReportViewer();
                //var DealerName = dbContext.JobsDetails.Where(x => x.JobID == Orderid).Select(x => x.RetailerID).FirstOrDefault();

                //var ShopName = dbContext.Retailers.Where(x => x.ID == DealerName).FirstOrDefault();

                //ReportParameter[] prm = new ReportParameter[12];


                //prm[0] = new ReportParameter("DealerName", tas.DealerName + "/" + rangeid.DealerCode);
                //prm[1] = new ReportParameter("SOName", tas.SOName);
                //prm[2] = new ReportParameter("ShopName", ShopName.ShopName);
                //prm[3] = new ReportParameter("GrossAmount", GrossAmount.ToString());
                //prm[4] = new ReportParameter("Display", display.ToString());
                //prm[5] = new ReportParameter("Balance1", balamo1.ToString());
                //prm[6] = new ReportParameter("Scheme", Scheme.ToString());
                //prm[7] = new ReportParameter("Balance2", balamo2.ToString());
                //prm[8] = new ReportParameter("Discount", dis.ToString());
                //prm[9] = new ReportParameter("NetAmount", Net.ToString());
                //prm[10] = new ReportParameter("Address", ShopName.Address);
                //prm[11] = new ReportParameter("BillNo", BillNo);
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = Server.MapPath("~\\Views\\Reports\\DealerDispatchAll.rdlc");
                reportViewer.LocalReport.EnableExternalImages = true;
                ReportDataSource dt1 = new ReportDataSource("DataSet1", ItemData);
                //ReportDataSource dt2 = new ReportDataSource("DataSet2", ItemData2);
                // reportViewer.LocalReport.SetParameters(prm);
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(dt1);
                //reportViewer.LocalReport.DataSources.Add(dt2);
                reportViewer.LocalReport.Refresh();
                // PrintReport.PrintToPrinter(ReportViewer1);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, filenameExtension;

                byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                //File  
                string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
                string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

                //create and set PdfReader  
                PdfReader reader = new PdfReader(bytes);
                FileStream output = new FileStream(FilePath, FileMode.Create);

                string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

                //create and set PdfStamper  
                PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

                if (Agent.Contains("Firefox"))
                    pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
                else
                    pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

                pdfStamper.FormFlattening = false;
                pdfStamper.Close();
                reader.Close();

                //return file path  
                FilePathReturn = @"TempFiles/" + FileName;

            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

            return Json(FilePathReturn);

        }
            

            
     


        #region DelieveryBoys

        [CustomAuthorize]
        //View Work...
        public ActionResult DelieveryBoys()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateDelieveryBoys([Bind(Exclude = "TID")] RegionData newRegion)
        {
            
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.RegionID == 0)
                    {
                        RegionValidator validator = new RegionValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        var userID = Convert.ToInt32(Session["UserID"]);
                        var DealerID = dbContext.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
                        int Response = ManageRegion.AddUpdateDelieveryBoys(newRegion,DealerID);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
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

        //Get All Region Method...
        public JsonResult DelieveryBoysDataHandler(DTParameters param)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var DealerID = dbContext.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
            try
            {
                var dtsource = new List<RegionData>();

                dtsource = ManageRegion.GetDelieveryBoysForGrid(DealerID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionData> data = ManageRegion.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionData> result = new DTResult<RegionData>
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

       // Delete Region Method...
        public int DeleteDelieveryBoys(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteBoys(RegionID);
        }

        #endregion DelieveryBoys



    }
}
 

