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
    public class ItemController : Controller
    {
       

        public ActionResult MainCategory()
        {
            // Load Region Data For City Records ...
            var objCity = new MainCategories();
      
          // objCity.Regions = FOS.Setup.ManageRegion.GetMainCategory();

            return View();
        }
        public ActionResult AddUpdateMainCategory([Bind(Exclude = "TID")] MainCategories newCity)
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
                            MainCategoryValidator validator = new MainCategoryValidator();
                            results = validator.Validate(newCity);
                            boolFlag = results.IsValid;
                        }

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddUpdateMainCategory(newCity);
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

        public JsonResult MainCategoryDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<MainCategories>();

                dtsource = ManageCity.GetMainCategoryForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<MainCategories> data = ManageCity.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageCity.Count1(param.Search.Value, dtsource, columnSearch);
                DTResult<MainCategories> result = new DTResult<MainCategories>
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

        public JsonResult GetEditMainCategory(int CityID)
        {
            var Response = ManageCity.GetEditMAinCategory(CityID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }


        public int DeleteMainCategory(int cityID)
        {
            return FOS.Setup.ManageCity.DeleteMAinCategory(cityID);
        }

        #region SubCategory

        public ActionResult SubCategory()
        {
            // int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            //List<Region> RegionObj = FOS.Setup.ManageRegion.GetRegionList(RHID);
            // var objRegion = RegionObj.FirstOrDefault();
            List<MainCategories> CityObj = FOS.Setup.ManageCity.GetProductMainCatList();

            //ViewData["CityObj"] = CityObj;

            var objSubCat = new SubCategories();
            ////objArea.Regions = RegionObj;
            objSubCat.mainCat = CityObj;

            return View(objSubCat);
        }

        public JsonResult SubCategoryDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<SubCategories>();

                dtsource = ManageArea.GetSubCatForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SubCategories> data = ManageArea.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count1(param.Search.Value, dtsource, columnSearch);
                DTResult<SubCategories> result = new DTResult<SubCategories>
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

        public ActionResult AddUpdateSubCategory([Bind(Exclude = "TID")] SubCategories newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        SubCatValidator validator = new SubCatValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateSubCAt(newData);
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

        public JsonResult GetSubCatByID(int regionID)
        {
            var result = FOS.Setup.ManageCity.GetSubCatList(regionID);
            return Json(result);
        }

        public JsonResult GetItemsList(int regionID, int SubCatID)
        {
            var result = FOS.Setup.ManageCity.GetItemsList(regionID, SubCatID);
            return Json(result);
        }

        public JsonResult GetSubCatAByID(int regionID, int regionID1)
        {
            var result = FOS.Setup.ManageCity.GetSubCatAList(regionID, regionID1);
            return Json(result);
        }


        public int DeleteSubCat(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteSubCat(areaID);
        }

        #endregion SubCategory


        #region Items

        [CustomAuthorize]
        // View ...
        public ActionResult item()
        {
            ////int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<MainCategories> CityObj = FOS.Setup.ManageCity.GetMainCatList();

            var objRegion = CityObj.FirstOrDefault();
            List<SubCategories> SubCategory = FOS.Setup.ManageCity.GetSubCatList(objRegion.ID);
            //var objSubCat = SubCategory.FirstOrDefault();
            //List <SubCategoryA> SubCatAA= FOS.Setup.ManageCity.GetSubCatAList(objSubCat.ID);
            //SubCatAA.Insert(0, new SubCategoryA
            //{
            //    ID = 0,
            //    SubCategoryAName = "All"
            //});

            var objArea = new Items();
            objArea.Regions = CityObj;
            objArea.SubCategory = SubCategory;
            //objArea.SubCategoryAList = SubCatAA;
            return View(objArea);




           // return View(objSubCat);
        }


        public ActionResult AddUpdateItemsss([Bind(Exclude = "TID")] Items newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ItemId == 0)
                    {
                        ItemValidator validator = new ItemValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateItems(newData);
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
        public JsonResult ItemsDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Items>();

                dtsource = ManageArea.GetItemsForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Items> data = ManageArea.GetResult2(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count2(param.Search.Value, dtsource, columnSearch);
                DTResult<Items> result = new DTResult<Items>
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

        //Delete Region...
        public int DeleteItems(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteItem(areaID);
        }

        #endregion Item


        #region SubCatA

        [CustomAuthorize]
        // View ...
        public ActionResult SubCategoryA()
        {
            ////int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<MainCategories> CityObj = FOS.Setup.ManageCity.GetProductMainCatList();

            var objRegion = CityObj.FirstOrDefault();
            List<SubCategories> SubCategory = FOS.Setup.ManageCity.GetProdSubCatList(objRegion.ID);


            var objArea = new SubCategoryA();
            objArea.Regions = CityObj;
            objArea.SubCategory = SubCategory;

            return View(objArea);




            // return View(objSubCat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateSubCategoryA([Bind(Exclude = "TID")] SubCategoryA newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                   

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateSubCatA(newData);
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
        public JsonResult SubCategoryADataHandler(DTParameters param, Int32 CityID, Int32 SubCat)
        {
            try
            {
                var dtsource = new List<SubCategoryA>();

                dtsource = ManageArea.GetSubCatAForGrid(CityID, SubCat);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SubCategoryA> data = ManageArea.GetResult3(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count3(param.Search.Value, dtsource, columnSearch);
                DTResult<SubCategoryA> result = new DTResult<SubCategoryA>
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

        //public JsonResult GetCityListByRegionID(int RegionID)
        //{
        //    var result = FOS.Setup.ManageCity.GetCityListByRegionID(RegionID);
        //    return Json(result);
        //}

        //Delete Region...
        public int DeleteSubCategoryA(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteSubCatA(areaID);
        }

        #endregion SubCatA



        #region Productcategory

        public ActionResult ProductCategory()
        {
            // Load Region Data For City Records ...
            var objCity = new MainCategories();

            // objCity.Regions = FOS.Setup.ManageRegion.GetMainCategory();

            return View();
        }
        public ActionResult AddUpdateProductCategory([Bind(Exclude = "TID")] MainCategories newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
               
                    if (newCity != null)
                    {
                        

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddUpdateProductCategory(newCity);
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

        public JsonResult ProductCategoryDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<MainCategories>();

                dtsource = ManageCity.GetProductCategoryForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<MainCategories> data = ManageCity.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageCity.Count1(param.Search.Value, dtsource, columnSearch);
                DTResult<MainCategories> result = new DTResult<MainCategories>
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

        public JsonResult GetEditProductCategory(int CityID)
        {
            var Response = ManageCity.GetEditProductCategory(CityID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }


        public int DeleteProductCategory(int cityID)
        {
            return FOS.Setup.ManageCity.DeleteMAinCategory(cityID);
        }
        #endregion






        public JsonResult GetItemsRelatedToJobID(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetItemsAcctoID(JobID);
            return Json(result);
        }


    }
}