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
    public class QRController : Controller
    {

        #region QrActivitys THINGS

        [CustomAuthorize]
        //View
        public ActionResult List()
        {
            // Load RegionalHead Data ...

            var objSaleOffice = new QrActivityData();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType(1);
            objSaleOffice.Cities = FOS.Setup.ManageCity.GetCityList();
            objSaleOffice.Areas = new List<Area>();
            return View(objSaleOffice);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateQrActivity([Bind(Exclude = "TID,RegionalHead")] QrActivityData newSaleOfficer)
        {
            Boolean boolFlag = true;
            Boolean PhoneNumberFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {

                if (newSaleOfficer != null)
                {

                    //if (newSaleOfficer.QrActivityID == 0)
                    //{
                    //    SaleOfficerValidator validator = new SaleOfficerValidator();
                    //    results = validator.Validate(newSaleOfficer);
                    //    boolFlag = results.IsValid;
                    //}

                    //if (newSaleOfficer.Phone1 != null)
                    //    {
                    //        if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumber1Exist(newSaleOfficer.ID, newSaleOfficer.Phone1 == null ? "" : newSaleOfficer.Phone1) == 1)
                    //        {
                    //            return Content("2");
                    //        }
                    //    }

                    //if (newSaleOfficer.Phone2 != null)
                    //    {
                    //        if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumber2Exist(newSaleOfficer.ID, newSaleOfficer.Phone2 == null ? "" : newSaleOfficer.Phone2) == 1)
                    //        {
                    //            return Content("2");
                    //        }
                    //    }

                    //if (newSaleOfficer.Phone1 != null && newSaleOfficer.Phone2 != null)
                    //    {
                    //        if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumberExist(newSaleOfficer.ID, newSaleOfficer.Phone1 == null ? "" : newSaleOfficer.Phone1, newSaleOfficer.Phone2 == null ? "" : newSaleOfficer.Phone2) == 1)
                    //        {
                    //            PhoneNumberFlag = false;
                    //        }
                    //    }

                    if (PhoneNumberFlag)
                    {
                        if (boolFlag)
                        {
                            if (ManageQrActivity.AddUpdateQrActivities(newSaleOfficer))
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

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        public JsonResult GetCityListByRegionHeadID(int ID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(ID);
            return Json(result);
        }

        public JsonResult GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionalHeadAccordingToType(RegionalHeadType);
            return Json(result);
        }

        public JsonResult GetAreaListByCityID(int ID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityID(ID);
            return Json(result);
        }

        public JsonResult GetAreaForSaleOfficerEdit(int ID, int CityID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityIDEdit(ID, CityID);
            return Json(result);
        }

        //Get All Region Method...
       public JsonResult DataHandler(DTParameters param , int RegionalHeadType , int RegionalHeadID)
       {
            try
            {
                var dtsource = new List<QrActivityData>();

                dtsource = ManageQrActivity.GetQrActivityListForGrid(RegionalHeadType , RegionalHeadID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<QrActivityData> data = ManageQrActivity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageQrActivity.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<QrActivityData> result = new DTResult<QrActivityData>
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
        public int DeleteQrActivity(int qrId)
        {
            return ManageQrActivity.DeleteQrActivity(qrId);
        }

        #endregion qrs

        #region QR SO List

        public ActionResult SOList(string qrCode)
        {
            QrSOData model = new QrSOData();
            model.QrCode = qrCode;
            return View(model);
        }
        public JsonResult SOGridList(DTParameters param, string QrCode)
        {
            try
            {
                var dtsource = new List<QrSOData>();

                dtsource = ManageQrActivity.GetQrSOListForGrid(QrCode);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<QrSOData> data = ManageQrActivity.GetResultSO(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageQrActivity.CountSO(param.Search.Value, dtsource, columnSearch);
                DTResult<QrSOData> result = new DTResult<QrSOData>
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

        #endregion
    }
}