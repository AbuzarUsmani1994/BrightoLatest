using FOS.Setup;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FOS.DataLayer;
using FluentValidation.Results;
using FOS.Web.UI.Models;
using FOS.Web.UI.Common.CustomAttributes;


using FOS.Web.UI.DataSets;
using CrystalDecisions.CrystalReports.Engine;
using Shared.Diagnostics.Logging;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace FOS.Web.UI.Controllers
{
    public class PainterController : Controller
    {

        // View ...
        [CustomAuthorize]
        public ActionResult PainterTest()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerList();
            var objSalesOfficer = SaleOfficerObj.FirstOrDefault();

            List<RetailerData> RetailerObj = new List<RetailerData>();


            if (objSalesOfficer == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(objSalesOfficer.ID);
                ViewData["RetailerObj"] = RetailerObj;
            }
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            var ObjPainter = new PainterAssociationData();
            ObjPainter.RegionalHeads = regionalHeadData;
            ObjPainter.SalesOfficer = SaleOfficerObj;
            ObjPainter.Retailers = RetailerObj;
            ObjPainter.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();


            return View(ObjPainter);
        }

        [HttpGet]
        public ActionResult ReportsInput()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();//.Select(c => new SelectListItem
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            //List<PainterCityNamesData> customer = new List<PainterCityNamesData>();
            //var customer = objPainter.GetCityList(0).Select(c => new SelectListItem
            //{
            //    Text = c.CityName,
            //    Value = c.ID.ToString()
            //});
            var customer = objPainter.GetCityList(1);
            obj.PainterCityNames = customer;
            //{
            //    Text = c.TeritoryName,
            //    Value = c.ID,
            //});
            return View(obj);
        }

        [HttpPost]
        public ActionResult ReportsInput(ReportsInputData model)
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();//.Select(c => new SelectListItem
            //{
            //    Text = c.TeritoryName,
            //    Value = c.ID,
            //});
            return View(obj);
        }

        public ActionResult CityDateWiseRetailerReport(string StartingDate, string EndingDate, string TID, string FOSID, string message)//string message)//, string[] sPainters
        {
            int[] arr;
            string[] mul=null;
            try
            {
                mul = message.Split(',');
            }
            catch (Exception exe)
            { 
            }
            SqlDataAdapter sda;
            ManageRetailer objRetailers = new ManageRetailer();
            //string StartingDate = "";
            //string EndingDate = "";
            DateTime Start = Convert.ToDateTime(StartingDate);
            DateTime End = Convert.ToDateTime(EndingDate);
            var rlist = objRetailers.GetRetailersForExportinExcelReportC(Start, End, TID, FOSID, mul);
            rptDataSet obj = new rptDataSet();
            DataRow dtrow;
            DataTable dtNewTable;
            dtNewTable = new DataTable();
            DataColumn dcol5 = new DataColumn("RetailerID", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol5);
            DataColumn dcol3 = new DataColumn("retailername", typeof(System.String));
            dtNewTable.Columns.Add(dcol3);
            DataColumn dcol0 = new DataColumn("saleofficername", typeof(System.String));
            dtNewTable.Columns.Add(dcol0);
            DataColumn dcol2 = new DataColumn("shopname", typeof(System.String));
            dtNewTable.Columns.Add(dcol2);
            DataColumn dcol4 = new DataColumn("cityname", typeof(System.String));
            dtNewTable.Columns.Add(dcol4);
            DataColumn dcol7 = new DataColumn("retailertype", typeof(System.String));
            dtNewTable.Columns.Add(dcol7);
            DataColumn dcol6 = new DataColumn("spreviousorder1kg", typeof(System.Int32));
            dtNewTable.Columns.Add(dcol6);
            DataColumn dcol8 = new DataColumn("visiteddate", typeof(System.DateTime));
            dtNewTable.Columns.Add(dcol8);
            foreach (var rowfiles in rlist)
            {
                dtrow = dtNewTable.NewRow();
                dtrow[0] = Convert.ToInt32(rowfiles.RetailerID);
                dtrow[1] = rowfiles.retailername;
                dtrow[2] = rowfiles.saleofficername;
                dtrow[3] = rowfiles.shopname;
                dtrow[4] = rowfiles.cityname;
                dtrow[5] = rowfiles.retailertype;
                dtrow[6] = Convert.ToInt32(rowfiles.spreviousorder1kg);
                dtrow[7] = Convert.ToDateTime(rowfiles.visiteddate);
                dtNewTable.Rows.Add(dtrow);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/RetailerReport1.rpt")));
            rd.SetDataSource(dtNewTable);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            string type = "xls";
            if (type == "pdf")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Retailers.pdf");
            }
            else
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/ms-excel", string.Format("CityDateWiseRetailerReport_{0}.xls", DateTime.Now.ToShortDateString()));
            }

            return null;
            //return View();
        }

        public JsonResult getData(string cities)
        {
            string[] mul = cities.Split(',');
            return null;
        }

        public ActionResult getCities(string TID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(TID))
            {
                tid = Convert.ToInt32(TID);
            }
            ManagePainter objPainter = new ManagePainter();
            //var customer = objPainter.GetCityList(tid).Select(c => new SelectListItem
            //{
            //    Text = c.CityName,
            //    Value = c.ID.ToString()
            //});

            //return Json(customer);
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
               var customer = objPainter.GetCityList(tid);
                foreach (var p in customer)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected == false) ? "" : "checked") + " id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.CityName + "</span><p style='font-size:9px;' id='cityName'>" + p.CityName + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        public ActionResult getFosNames(string TID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(TID))
            {
                tid = Convert.ToInt32(TID);
            }
            ManagePainter objPainter = new ManagePainter();
            var customers = objPainter.getFosLov(tid);
            var customer = objPainter.getFosLov(tid).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID
            });
            return Json(customer);
        }

        public ActionResult getCitiess(string TID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(TID))
            {
                tid = Convert.ToInt32(TID);
            }
            ManagePainter objPainter = new ManagePainter();
            var customer = objPainter.GetCityList(tid);
            

            return Json(customer);
        }

        public JsonResult LoadCitiesEdit()
        {
            string Reponse = "";
            List<int> painterIds;
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                painterIds = dbContext.Cities.Select(r => r.ID).ToList();

                //var painters = dbContext.CityPaintersByRetailer(RetailerID, CityName);
                var painters = (from r in dbContext.Cities
                                select new
                                    {
                                        RetailerID=r.ID,
                                        CityName=r.Name,
                                    }).ToList();

                //foreach (var p in painters)
                //{
                //    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.RetailerID ?? false) ? "checked" : "") + " id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.Name + "</span><p style='font-size:9px;' id='cityName'>" + p.City + "</p></div>";
                //}
                return Json(new { painters });
            }

            //return Json(new { Response = Reponse, PainterIDs = painterIds });
            return Json(new { Response = Reponse, PainterIDs = painterIds });

        }

        [CustomAuthorize]
        public ActionResult Painter()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerList();
            var objSalesOfficer = SaleOfficerObj.FirstOrDefault();

            List<RetailerData> RetailerObj = new List<RetailerData>();


            if (objSalesOfficer == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(objSalesOfficer.ID);
                ViewData["RetailerObj"] = RetailerObj;
            }
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            var ObjPainter = new PainterAssociationData();
            ObjPainter.RegionalHeads = regionalHeadData;
            ObjPainter.SalesOfficer = SaleOfficerObj;
            ObjPainter.Retailers = RetailerObj;
            ObjPainter.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();


            return View(ObjPainter);
        }

        public ActionResult LoadPainters(string CityName)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<PainterAssociationData> PainterObject = new List<PainterAssociationData>();


                //PainterObject = dbContext.vPainters.OrderBy(p => p.City).Where(p => p.Name != null && p.Registered == true && p.CNIC != null && p.City == CityName)
                //    .Select(p =>
                //         new PainterAssociationData
                //         {
                //             ID = p.ID,
                //             PainterName = p.Name,
                //             City = p.City
                //         }).ToList();

                foreach (var p in PainterObject)
                {

                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='painter" + p.ID + "' data-id='" + p.ID + "' class='' name='painter[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.PainterName + "</span><p style='font-size:9px;'> " + p.City + "</p></div>";
                }
                
            }

            return Json(new { Response = Reponse });
        }

        public JsonResult LoadPaintersEdit(int RegionalHeadID, int SaleOfficerID, int RetailerID, string CityName)
        {
            string Reponse = "";
            List<int> painterIds;
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                  painterIds = dbContext.RetailerPainters.Where(r => r.RetailerID == RetailerID).Select(r => r.PainterID).ToList(); 
                  
                  var painters = dbContext.CityPaintersByRetailer(RetailerID, CityName);
                  
                  foreach (var p in painters)
                  {
                      Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected ?? false ) ? "checked": "") +" id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.Name + "</span><p style='font-size:9px;' id='cityName'>" + p.City + "</p></div>";
                  }

            }

            return Json(new { Response = Reponse, PainterIDs = painterIds });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateRetailerPaintersAssociation([Bind(Exclude = "TID")] PainterAssociationData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();


            try
            {
                if (newData != null)
                {
                    if (newData.SelectedPainters == null)
                    {
                        boolFlag = false;
                    }

                    if (boolFlag)
                    {

                        Res = ManagePainter.AddUpdatePaintersRelatedToRetailers(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
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
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }


        //Delete RetailerPaintersAssociation Method...
        public int DeleteRetailerPaintersAssociation(int ID)
        {
            return ManagePainter.DeleteRetailerPaintersAssociation(ID);
        }


        //Get All Region Method...
        public JsonResult DataHandler(DTParameters param, int RegionalHeadID, int SaleOfficerID)
        {
            try
            {

                var dtsource = new List<PainterAssociationData>();

                int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                //if (regionalheadID == 0)
                //{
                dtsource = ManagePainter.GetAllPaintersRelatedToRetailersListForGrid(RegionalHeadID, SaleOfficerID);
                //}
                //else
                //{
                //    dtsource = ManageJobs.GetJobsForGrid(regionalheadID, SaleOfficerID);
                //}
                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<PainterAssociationData> data = ManagePainter.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManagePainter.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<PainterAssociationData> result = new DTResult<PainterAssociationData>
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


        public JsonResult LoadRetailersRelatedToSaleOfficer(int SaleOfficerID)
        {
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                return Json(dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID).Select(r =>
                        new RetailerData
                        {
                            ID = r.ID,
                            Name = r.Name,
                            CityID = r.CityID,
                            CItyName = dbContext.Cities.Where(c => c.ID == r.CityID).Select(c => c.Name).FirstOrDefault()
                        }).ToList());
            }
        }

        public JsonResult PaintersInfo()
        {
            FOSDataModel dbContext = new FOSDataModel();
            PainterInfoData info = new PainterInfoData();

            info.TotalNoOfPainters = 0;// dbContext.vPainters.Where(p => p.Registered == true).Count();
            info.Cities = 0;// dbContext.vPainters.Where(p => p.City != null && p.City != "").GroupBy(p => p.City).Count();
            return Json(new { TotalNoOfPainters = info.TotalNoOfPainters, UsedPainters = info.UsedPainters, FreePainters = info.FreePainters, Cities = info.Cities });
        }

    }

    public class PainterInfoData {
        public int TotalNoOfPainters { get; set; }
        public int UsedPainters { get; set; }
        public int FreePainters { get; set; }
        public int Cities { get; set; }
    }







}