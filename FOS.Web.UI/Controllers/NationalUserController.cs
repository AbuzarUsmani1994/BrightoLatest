using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class NationalUserController : Controller
    {
        FOSDataModel dbContext = new FOSDataModel();

        #region Sale Officer

           [CustomAuthorize]
        //View
        public ActionResult NationalUser()
        {
            // Load RegionalHead Data ...
            var userID = Convert.ToInt32(Session["UserID"]);
            //var objSaleOffice = new NationalUserData();
            //objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            //objSaleOffice.RegionData = ManageRegion.GetRegionDataList();
            //objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            //objSaleOffice.SaleOfficerData = ManageSaleOffice.GetAllSO();
            //return View(objSaleOffice);


            var objSaleOffice = new NationalUserData();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            objSaleOffice.RegionData = ManageRegion.GetRegionDataList(userID);
            objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objSaleOffice.SaleOfficerData = ManageSaleOffice.GetAllSO();
            objSaleOffice.Cities = new List<CityData>();
            objSaleOffice.Areas = new List<Area>();
            return View(objSaleOffice);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSaleOfficersAccess([Bind(Exclude = "TID,RegionalHead")] NationalUserData userData)
        {
            Boolean boolFlag = true;

            ValidationResult results = new ValidationResult();
            try
            {
                if (userData != null)
                {
                    FOSDataModel dbcontext = new FOSDataModel();
                    var salelist = ManageSaleOffice.GetSaleOfficerListRelatedtoregionalHeadID(userData.RegionID);
                    if (salelist.Count > 0)
                    {
                        foreach (var list in salelist)
                        {
                            Tbl_Access acc = new Tbl_Access();
                            acc.RegionID = userData.RegionID;
                            acc.SaleOfficerID = list.ID;
                            acc.ReportedDown = list.ID;
                            acc.RepotedUP = userData.SOID;
                            acc.CreatedOn = DateTime.Now;
                            acc.Status = true;
                            acc.IsDeleted = false;
                            dbcontext.Tbl_Access.Add(acc);
                            dbcontext.SaveChanges();

                        }
                    }
                }
                return Content("1");

            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }


        }
        public JsonResult GetSOListByRegionalHeadID(int RegionalHeadID)
        {
            var result = ManageSaleOffice.GetSOByRegionalHeadId(RegionalHeadID);
            return Json(result);
        }
        public JsonResult NationalUserAccessDataHandler(DTParameters param, Int32 CityID)
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
        public JsonResult GetCityListByRegionHeadID(int ID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(ID);
            return Json(result);
        }
        public JsonResult GetSalesOfficer(int RegionID)
        {

            using (FOSDataModel model = new FOSDataModel())
            {
                var result = model.spGetSaleOfficer(RegionID).ToList();

                return Json(result);
            }

        }
        public JsonResult GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionalHeadAccordingToType(RegionalHeadType);
            return Json(result);
        }
        public JsonResult GetSORegions(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionsofSO(RegionalHeadType);

            return Json(result);
        }
        public JsonResult GetSOByRegionalHeadId(int RegionalHeadId)
        {
            var result = FOS.Setup.ManageSaleOffice.GetSOByRegionalHeadId(RegionalHeadId);
            return Json(result);
        }


        public JsonResult GetRetailersBySOID(int soId)
        {
            var result = FOS.Setup.ManageRetailer.GetRetailerBySOID(soId);
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
        public JsonResult DataHandler(DTParameters param, int RegionalHeadType, int RegionalHeadID)
        {
            try
            {
                var dtsource = new List<SaleOfficerData>();

                int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                if (regionalheadID == 0)
                {
                    dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType, RegionalHeadID);
                }
                else
                {
                    RegionalHeadID = regionalheadID;
                    dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType, RegionalHeadID);
                }

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SaleOfficerData> data = ManageSaleOffice.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageSaleOffice.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<SaleOfficerData> result = new DTResult<SaleOfficerData>
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
        public int DeleteSaleOfficer(int saleOfficerID)
        {
            return ManageSaleOffice.DeleteSaleOfficer(saleOfficerID);
        }
        public int DeleteAccess(int ID)
        {
            return FOS.Setup.ManageArea.DeleteNationalUserAccess(ID);




        }
        #endregion Sale Officer


        #region Transfer Orders

        [CustomAuthorize]
        //View
        public ActionResult TransferOrders()
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


            objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.Banks = ManageRetailer.GetBanks();

            return View(objRetailer);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransferOrders([Bind(Exclude = "TID,RegionalHead")] RetailerData userData)
        {

            ValidationResult results = new ValidationResult();
            try
            {
                if (userData != null)
                {
                    JobsDetail AreaObj = new JobsDetail();
                    FOSDataModel dbcontext = new FOSDataModel();
                    var salelist = ManageSaleOffice.GetOrdersForRetailer(userData.TransferFrom);
                    if (salelist.Count > 0)
                    {
                        foreach (var item in salelist)
                        {
                            
                            AreaObj = dbContext.JobsDetails.Where(u => u.ID == item.ID).FirstOrDefault();
                            AreaObj.RetailerID= userData.TransferTo;
                           
                        }
                        dbContext.SaveChanges();
                        
                       

                    }
                   
                }
                else
                {
                    return Content("2");

                }
                return Content("1");

            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }


            //}
            //public JsonResult GetSOListByRegionalHeadID(int RegionalHeadID)
            //{
            //    var result = ManageSaleOffice.GetSOByRegionalHeadId(RegionalHeadID);
            //    return Json(result);
            //}
            //public JsonResult NationalUserAccessDataHandler(DTParameters param, Int32 CityID)
            //{
            //    try
            //    {
            //        var dtsource = new List<Tbl_AccessModel>();

            //        dtsource = ManageArea.GetAccessForGrid(CityID);

            //        List<String> columnSearch = new List<string>();

            //        foreach (var col in param.Columns)
            //        {
            //            columnSearch.Add(col.Search.Value);
            //        }

            //        List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
            //        int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
            //        DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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
            //public JsonResult GetCityListByRegionHeadID(int ID)
            //{
            //    var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(ID);
            //    return Json(result);
            //}
            //public JsonResult GetSalesOfficer(int RegionID)
            //{

            //    using (FOSDataModel model = new FOSDataModel())
            //    {
            //        var result = model.spGetSaleOfficer(RegionID).ToList();

            //        return Json(result);
            //    }

            //}
            //public JsonResult GetRegionalHeadAccordingToType(int RegionalHeadType)
            //{
            //    var result = FOS.Setup.ManageSaleOffice.GetRegionalHeadAccordingToType(RegionalHeadType);
            //    return Json(result);
            //}
            //public JsonResult GetSORegions(int RegionalHeadType)
            //{
            //    var result = FOS.Setup.ManageSaleOffice.GetRegionsofSO(RegionalHeadType);

            //    return Json(result);
            //}
            //public JsonResult GetSOByRegionalHeadId(int RegionalHeadId)
            //{
            //    var result = FOS.Setup.ManageSaleOffice.GetSOByRegionalHeadId(RegionalHeadId);
            //    return Json(result);
            //}


            //public JsonResult GetRetailersBySOID(int soId)
            //{
            //    var result = FOS.Setup.ManageRetailer.GetRetailerBySOID(soId);
            //    return Json(result);
            //}
            //public JsonResult GetAreaListByCityID(int ID)
            //{
            //    var result = FOS.Setup.ManageArea.GetAreaListByCityID(ID);
            //    return Json(result);
            //}

            //public JsonResult GetAreaForSaleOfficerEdit(int ID, int CityID)
            //{
            //    var result = FOS.Setup.ManageArea.GetAreaListByCityIDEdit(ID, CityID);
            //    return Json(result);
            //}

            ////Get All Region Method...
            //public JsonResult DataHandler(DTParameters param, int RegionalHeadType, int RegionalHeadID)
            //{
            //    try
            //    {
            //        var dtsource = new List<SaleOfficerData>();

            //        int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

            //        if (regionalheadID == 0)
            //        {
            //            dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType, RegionalHeadID);
            //        }
            //        else
            //        {
            //            RegionalHeadID = regionalheadID;
            //            dtsource = ManageSaleOffice.GetSaleOfficerListForGrid(RegionalHeadType, RegionalHeadID);
            //        }

            //        List<String> columnSearch = new List<string>();

            //        foreach (var col in param.Columns)
            //        {
            //            columnSearch.Add(col.Search.Value);
            //        }

            //        List<SaleOfficerData> data = ManageSaleOffice.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
            //        int count = ManageSaleOffice.Count(param.Search.Value, dtsource, columnSearch);
            //        DTResult<SaleOfficerData> result = new DTResult<SaleOfficerData>
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
            //public int DeleteSaleOfficer(int saleOfficerID)
            //{
            //    return ManageSaleOffice.DeleteSaleOfficer(saleOfficerID);
            //}
            //public int DeleteAccess(int ID)
            //{
            //    return FOS.Setup.ManageArea.DeleteNationalUserAccess(ID);




            //}
            #endregion TransferOrders


        }

        #region Upload StockPosition

        public ActionResult UploadStock()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
         
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
            });

            List<RetailerData> RetailerObj = new List<RetailerData>();

            if (SaleOfficerObj == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(SaleOfficerObj.Select(s => s.ID).FirstOrDefault());
                ViewData["RetailerObj"] = RetailerObj;
            }



            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListByRegionID(regionalHeadData.FirstOrDefault().ID);
            objJob.Range = FOS.Setup.ManageRegion.GetRanges();

            return View(objJob);
        }


        [HttpPost]
        public ActionResult UploadStockFile(HttpPostedFileBase postedFile,int RangeID,string StartingDate1, string StartingDate2)
        {
            List<position> list = new List<position>();
            position comlist;
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

               // Insert records to database table.
                StockPosition entities = new StockPosition();
                foreach (DataRow row in dt.Rows)
                {
                    StockPosition position = new StockPosition();
                    position.ItemID =Convert.ToInt32(row.ItemArray[0]);
                    position.RegionID = Convert.ToInt32(row.ItemArray[1]);
                    position.CityID = Convert.ToInt32(row.ItemArray[2]);
                    position.Quantity = Convert.ToInt32(row.ItemArray[3]);
                    position.RangeID = RangeID;
                    position.StartDate = Convert.ToDateTime(StartingDate1);
                    position.EndDate = Convert.ToDateTime(StartingDate2);
                    dbContext.StockPositions.Add(position);
                    dbContext.SaveChanges();
                }
             
            }

            return Content("1");
        }
        #endregion Upload StockPosition


        #region Upload StockInvoice

        public ActionResult UploadInvoice()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);

            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
            });

            List<RetailerData> RetailerObj = new List<RetailerData>();

            if (SaleOfficerObj == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(SaleOfficerObj.Select(s => s.ID).FirstOrDefault());
                ViewData["RetailerObj"] = RetailerObj;
            }



            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListByRegionID(regionalHeadData.FirstOrDefault().ID);
            objJob.Range = FOS.Setup.ManageRegion.GetRanges();

            return View(objJob);
        }


        [HttpPost]
        public ActionResult UploadInvoiceFile(HttpPostedFileBase postedFile, int RangeID, string StartingDate1, string StartingDate2)
        {
            List<position> list = new List<position>();
            position comlist;
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                StockInvoice entities = new StockInvoice();
                foreach (DataRow row in dt.Rows)
                {
                    StockInvoice position = new StockInvoice();
                    position.ItemID = Convert.ToInt32(row.ItemArray[0]);
                    position.RegionID = Convert.ToInt32(row.ItemArray[1]);
                    position.CityID = Convert.ToInt32(row.ItemArray[2]);
                    position.Quantity = Convert.ToInt32(row.ItemArray[3]);
                    position.RangeID = RangeID;
                    position.StartDate = Convert.ToDateTime(StartingDate1);
                    position.EndDate = Convert.ToDateTime(StartingDate2);
                    dbContext.StockInvoices.Add(position);
                    dbContext.SaveChanges();
                }

            }

            return Content("1");
        }
        #endregion Upload StockPosition
    }



    public class position
    {
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> RegionID { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> RangeID { get; set; }
    }
}