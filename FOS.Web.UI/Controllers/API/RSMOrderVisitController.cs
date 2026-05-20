using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class RSMOrderVisitController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Job();
            var RemObj = new TblReminder();
            var kpiReport = new KPIReport();

            var Lastdata = db.Retailers.Where(x => x.ID == rm.RetailerId).OrderByDescending(x => x.ID).FirstOrDefault();
            try
            {
                var regionalHeadID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerId).Select(x => x.RegionalHeadID).FirstOrDefault();
                Retailer ret = db.Retailers.Where(r => r.ID == rm.RetailerId).FirstOrDefault();
                if (ret != null)
                {
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                   
                    JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    JobObj.SaleOfficerID = rm.SaleOfficerId;
                    JobObj.RegionalHeadID = regionalHeadID;
                    JobObj.RegionalHeadType = ret.SaleOfficer.RegionalHead.Type;
                    JobObj.Status = true;
                    JobObj.StartingDate = localTime;
                    JobObj.DateOfAssign = localTime;
                    JobObj.CreatedDate = localTime;
                    JobObj.LastUpdated = localTime;
                    JobObj.IsActive = true;
                    JobObj.IsDeleted = false;


                    //ADD New Job in jobsdetail 
                    jobDet.JobID = JobObj.ID;
                    jobDet.RegionalHeadID = JobObj.RegionalHeadID;
                    jobDet.SalesOficerID = JobObj.SaleOfficerID;
                    jobDet.RetailerID = rm.RetailerId;
                    jobDet.ActivityDetails = "Online";
                    jobDet.JobDate = localTime;
                    jobDet.ReasonForNoSaleID = rm.ReasonForNoSaleID;
                    jobDet.VisitTypeID = rm.VisitTypeID;
                    jobDet.DealerID = Lastdata.DealerID;
                    //jobDet.DiscountPercentage = rm.DiscountPercentage;
                    //jobDet.DiscountedTotal = rm.DiscountedTotal;
                    //jobDet.OrderTotal = rm.OrderTotal;
                    jobDet.JobType = "RSM Visit";
                    jobDet.Status = true;
                        //if (Lastdata != null)
                        //{
                        //    jobDet.Lastvisitdate = Lastdata.JobDate;
                        //    jobDet.LastvisitType = Lastdata.VisitPurpose;
                        //}
                        //else
                        //{
                        //    jobDet.Lastvisitdate = DateTime.UtcNow.AddHours(5);
                        //    jobDet.LastvisitType = rm.Type;

                        //}
                        // jobDet.ActivityType = rm.ActivityType;
                        jobDet.VisitPurpose = "Ordering";
                        //jobDet.Dispatchstatus = "Ordered";
                  



                    db.Jobs.Add(JobObj);
                    db.SaveChanges();
                    db.JobsDetails.Add(jobDet);
                    db.SaveChanges();


                    // Here StockItems is the array which is changed From JobItems Array due to Hassan 

                    if (rm.StockItems != null && rm.StockItems.Count > 0)
                    {
                        foreach (var item in rm.StockItems)
                        {
                            db.JobItems.Add(
                                new JobItem
                                {
                                    JobID = JobObj.ID,
                                    ItemID = item.ItemID,
                                    Quantity = 1,
                                    Price = item.Price,
                                    Carton=item.Carton,
                                    Danda=item.Danda,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedOn = localTime,
                                    CreatedBy = rm.SaleOfficerId,
                                    FinalValue=item.TotalCartonOrder,

                                });
                            db.SaveChanges();
                        }
                    }

                    //if (rm.CurrentStock != null && rm.CurrentStock.Count > 0)
                    //{
                    //    foreach (var item in rm.CurrentStock)
                    //    {
                    //        db.TBL_CurrentStock.Add(
                    //            new TBL_CurrentStock
                    //            {
                    //                JobID = JobObj.ID,
                    //                ItemID = item.ItemID,
                    //                Danda = item.Danda,
                    //                Carton = item.Carton,
                    //                IsActive = true,
                    //                Price=item.Price,
                    //                CreatedOn = localTime
                                    

                    //            });
                    //        db.SaveChanges();
                    //    }
                    //}

                    if (rm.Brandrate != null && rm.Brandrate.Count > 0)
                    {
                        foreach (var item in rm.Brandrate)
                        {
                            db.Tbl_OurBrandRate.Add(
                                new Tbl_OurBrandRate
                                {
                                   JobID = JobObj.ID,
                                    ItemID = item.ItemID,
                                    PRDanda = item.PRDanda,
                                    PRCarton = item.PRCarton,
                                    PRGhatta = item.PRGhatta,
                                    SRCarton = item.SRCarton,
                                    SRDanda = item.SRDanda,
                                    SRGhatta = item.SRGhatta,
                                    PurchaseFinalValue=item.PurchaseTotalCartonBrand,
                                    SaleFinalValue=item.SaleTotalCartonBrand,
                                   ProdAvailability=item.ProductAvailability,

                                    CreatedOn = localTime


                                });
                            db.SaveChanges();
                        }
                    }

                    if (rm.CompititorBrand != null && rm.CompititorBrand.Count > 0)
                    {
                        foreach (var item in rm.CompititorBrand)
                        {
                            db.Tbl_CompititorBrand.Add(
                                new Tbl_CompititorBrand
                                {
                                    JobID = JobObj.ID,
                                    ItemID = item.ItemID,
                                    PRDanda = item.PRDanda,
                                    PRCarton = item.PRCarton,
                                    PRGhatta = item.PRGhatta,
                                    SRCarton = item.SRCarton,
                                    SRDanda = item.SRDanda,
                                    SRGhatta = item.SRGhatta,
                                    PurchaseFinalValue=item.PurchaseTotalCartonCompititor,
                                    SaleFinalValue=item.SaleTotalCartonCompititor,
                                    Brandname=item.BrandName,
                                    ProdAvailability = item.ProductAvailability,
                                    CreatedOn = localTime,
                                    Size=item.Size
                                    

                                });
                            db.SaveChanges();
                        }
                    }

                    //DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                    //    DateTime dtFromToday = dtFromTodayUtc.Date;
                    //    DateTime dtToToday = dtFromToday.AddDays(1);
                    //    var data = db.KPIReports.Where(x => x.SOID == rm.SaleOfficerId && x.Date >= dtFromToday && x.EndDate <= dtToToday).FirstOrDefault();

                    //    if (data == null)
                    //    {
                    //        kpiReport.SOID = rm.SaleOfficerId;
                    //        kpiReport.ProductiveVisits = 1;
                    //        kpiReport.TotalVisits = 1;
                    //        kpiReport.Date = localTime;
                    //        kpiReport.EndDate = localTime;
                    //        db.KPIReports.Add(kpiReport);
                    //        db.SaveChanges();

                    //    }
                    //    else
                    //    {
                    //        kpiReport = db.KPIReports.Where(u => u.SOID == rm.SaleOfficerId && u.Date >= dtFromToday && u.EndDate <= dtToToday).FirstOrDefault();

                    //        kpiReport.ProductiveVisits = kpiReport.ProductiveVisits + 1;
                    //        kpiReport.TotalVisits = kpiReport.TotalVisits + 1;
                    //        kpiReport.EndDate = localTime;

                    //        db.SaveChanges();
                    //    }




                }

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Order Done Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Order API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Order API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            

        }
        }

        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }


        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
            public DailyActivityRequest()
            {
                StockItems = new List<JobItemModel>();
                CurrentStock = new List<CurrentStockModel>();
                Brandrate = new List<BrandRateModel>();
                CompititorBrand = new List<CompititorBrandModel>();
            }
            public int JobId { get; set; }
            public int RetailerId { get; set; }
            public int SaleOfficerId { get; set; }
            public int Followupreason { get; set; }
            public string Type { get; set; }
            public int ReasonForNoSaleID { get; set; }
            public int VisitTypeID { get; set; }
            public string Remarks { get; set; }
            public int DiscountPercentage { get; set; }
            public int OrderTotal { get; set; }
            public int DiscountedTotal { get; set; }
            public string PurposeofVisit { get; set; }
            public string ActivityDetails { get; set; }
            public string Picture1 { get; set; }
            public string ReminderCancelStatus { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
            public string NextVisitDate { get; set; }
            public string Priorty { get; set; }
            public string TentativeCloseDate { get; set; }
            public List<JobItemModel> StockItems { get; set; }
            public List<CurrentStockModel> CurrentStock { get; set; }
            public List<BrandRateModel> Brandrate { get; set; }
            public List<CompititorBrandModel> CompititorBrand { get; set; }
        }

        public class JobItemModel
        {
            public int JobID { get; set; }
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal? TotalCartonOrder { get; set; }
            public int? Carton { get; set; }
            public int? Danda { get; set; }
        }
        public class CurrentStockModel
        {
            public decimal Price { get; set; }
            public int ItemID { get; set; }
            public decimal? TotalCartonStock { get; set; }
            public int? Carton { get; set; }
            public int? Danda { get; set; }
        }
        public class BrandRateModel
        {

            public int ItemID { get; set; }
            public int? PRCarton { get; set; }
            public int? PRDanda { get; set; }
            public int? PRGhatta { get; set; }
            public decimal? PurchaseTotalCartonBrand { get; set; }
            public decimal? SaleTotalCartonBrand { get; set; }
            public int? SRCarton { get; set; }
            public int? SRDanda { get; set; }
            public int? SRGhatta { get; set; }

            public string ProductAvailability { get; set; }
        }

        public class CompititorBrandModel
        {
            public string BrandName { get; set; }
            public string Size { get; set; }
            public int ItemID { get; set; }
            public int? PRCarton { get; set; }
            public int? PRDanda { get; set; }
            public decimal? PurchaseTotalCartonCompititor { get; set; }
            public decimal? SaleTotalCartonCompititor { get; set; }
            public int? PRGhatta { get; set; }
            public int? SRCarton { get; set; }
            public int? SRDanda { get; set; }
            public int? SRGhatta { get; set; }
            public string ProductAvailability { get; set; }
        }
    }
}