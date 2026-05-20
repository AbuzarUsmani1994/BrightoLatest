using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class RegionalHeadController : Controller
    {
        [CustomAuthorize]
        // View ...
        public ActionResult RegionalHead()
        {
            // Load Region Data For City Records ...

            var objReigionalHead = new RegionalHeadData();
            objReigionalHead.Regions = FOS.Setup.ManageRegion.GetRegionForRegionalHead(1);
            objReigionalHead.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objReigionalHead.Ranges = FOS.Setup.ManageRegion.GetRangeType();
            return View(objReigionalHead);
        }

        public JsonResult GetRegionForRegionalHead(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageRegion.GetRegionForRegionalHead(RegionalHeadType);
            return Json(result);
        }

        public JsonResult GetRegionForRegionalHeadEdit(int ID, int RegionalHeadType)
        {
            var result = FOS.Setup.ManageRegion.GetRegionForRegionalHeadEdit(ID, RegionalHeadType);
            return Json(result);
        }

        //Insert Update Region Method...

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUpdateRegionalHead([Bind(Exclude = "TID,Regions")] RegionalHeadData newRegionalHead)
        {
            Boolean boolFlag = true;
            Boolean PhoneNumberFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newRegionalHead != null)
                    {
                        if (newRegionalHead.ID == 0)
                        {
                            RegionalHeadValidator validator = new RegionalHeadValidator();
                            results = validator.Validate(newRegionalHead);
                            boolFlag = results.IsValid;
                        }

                        //if (newRegionalHead.Phone1 != null)
                        //{
                        //    if (FOS.Web.UI.Common.NumberCheck.CheckRHNumber1Exist(newRegionalHead.ID, newRegionalHead.Phone1 == null ? "" : newRegionalHead.Phone1) == 1)
                        //    {
                        //        return Content("2");
                        //    }
                        //}

                        //if (newRegionalHead.Phone2 != null)
                        //{
                        //    if (FOS.Web.UI.Common.NumberCheck.CheckRHNumber2Exist(newRegionalHead.ID, newRegionalHead.Phone2 == null ? "" : newRegionalHead.Phone2) == 1)
                        //    {
                        //        return Content("2");
                        //    }
                        //}

                        //if (newRegionalHead.Phone1 != null && newRegionalHead.Phone2 != null)
                        //{
                        //    if (FOS.Web.UI.Common.NumberCheck.CheckRegionalHeadNumberExist(newRegionalHead.ID, newRegionalHead.Phone1 == null ? "" : newRegionalHead.Phone1, newRegionalHead.Phone2 == null ? "" : newRegionalHead.Phone2) == 1)
                        //    {
                        //        PhoneNumberFlag = false;
                        //    }
                        //}

                        if (PhoneNumberFlag)
                        {
                            if (boolFlag)
                            {
                                if (ManageRegionalHead.AddUpdateRegionalHead(newRegionalHead))
                                {
                                    return Content("1");
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

        //Get All Region Method...

        public JsonResult DataHandler(DTParameters param, int RegionalHeadType)
        {
            try
            {
                var dtsource = new List<RegionalHeadData>();

                dtsource = ManageRegionalHead.GetRegionalForGrid(RegionalHeadType);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionalHeadData> data = ManageRegionalHead.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegionalHead.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionalHeadData> result = new DTResult<RegionalHeadData>
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
        public int DeleteRegionalHead(int regionalHeadID)
        {
            return ManageRegionalHead.DeleteRegionalHead(regionalHeadID);
        }






        public ActionResult ItemGroup()
        {
            // Load Region Data For City Records ...

            var objReigionalHead = new RegionalHeadData();
            objReigionalHead.Regions = FOS.Setup.ManageRegion.GetItemsForRanges(6);
            objReigionalHead.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRangeType();

            return View(objReigionalHead);
        }


        public JsonResult GetItemsForRegions(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageRegion.GetItemsForRanges(RegionalHeadType);
            return Json(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUpdateItemGroup([Bind(Exclude = "TID,Regions")] RegionalHeadData newRegionalHead)
        {
            Boolean boolFlag = true;
            Boolean PhoneNumberFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newRegionalHead != null)
                    {
                        if (newRegionalHead.ID == 0)
                        {
                            RegionalHeadValidator validator = new RegionalHeadValidator();
                            results = validator.Validate(newRegionalHead);
                            boolFlag = results.IsValid;
                        }

                       

                        if (PhoneNumberFlag)
                        {
                            if (boolFlag)
                            {
                                if (ManageRegionalHead.AddUpdateItemGroup(newRegionalHead))
                                {
                                    return Content("1");
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


        public JsonResult ItemGroupDataHandler(DTParameters param, int RegionalHeadType)
        {
            try
            {
                var dtsource = new List<RegionalHeadData>();

                dtsource = ManageRegionalHead.GetItemGroupForGrid(RegionalHeadType);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionalHeadData> data = ManageRegionalHead.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegionalHead.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionalHeadData> result = new DTResult<RegionalHeadData>
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


        public int DeleteItemGroup(int regionalHeadID)
        {
            return ManageRegionalHead.DeleteItemgroup(regionalHeadID);
        }
    }
}