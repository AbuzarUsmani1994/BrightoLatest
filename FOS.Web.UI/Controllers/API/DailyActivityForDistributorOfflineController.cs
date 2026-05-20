//using FOS.DataLayer;
//using FOS.Web.UI.Common;
//using Shared.Diagnostics.Logging;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Web.Http;

//namespace FOS.Web.UI.Controllers.API
//{
//    public class DailyActivityForDistributorOfflineController : ApiController
//    {
//        FOSDataModel db = new FOSDataModel();

//        public Result<SuccessResponse> Post(DailyActivityRequest rm)
//        { // This controller is for Distributors orders.
//            JobsDetail jobDet = new JobsDetail();
//            var JobObj = new Job();
//            var RemObj = new TblReminder();
//            try
//            {
//                Dealer ret = db.Dealers.Where(r => r.ID == rm.RetailerId).FirstOrDefault();
//                if (ret != null)
//                {
//                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
//                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

//                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
//                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


//                    JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

//                    JobObj.SaleOfficerID = rm.SaleOfficerId;
//                    JobObj.RegionalHeadID = ret.SaleOfficer.RegionalHeadID;
//                    JobObj.RegionalHeadType = ret.SaleOfficer.RegionalHead.Type;
//                    JobObj.Status = true;
//                    JobObj.StartingDate = localTime;
//                    JobObj.DateOfAssign = localTime;
//                    JobObj.CreatedDate = localTime;
//                    JobObj.LastUpdated = localTime;
//                    JobObj.IsActive = true;
//                    JobObj.IsDeleted = false;


//                    //ADD New Job in jobsdetail 
//                    jobDet.JobID = JobObj.ID;
//                    jobDet.RegionalHeadID = JobObj.RegionalHeadID;
//                    jobDet.SalesOficerID = JobObj.SaleOfficerID;
//                    jobDet.RetailerID = 1;
//                    jobDet.ActivityDetails = "Online";
//                    jobDet.DealerID = rm.RetailerId;
//                    jobDet.DiscountPercentage = rm.DiscountPercentage;
//                    jobDet.DiscountedTotal = rm.DiscountedTotal;
//                    jobDet.OrderTotal = rm.OrderTotal;
//                    jobDet.JobDate = localTime;
//                    jobDet.JobType = "Distributor Order";
//                    jobDet.ActivityDetails = "Offline";
//                    jobDet.ActivityType = "Ordered";
//                    jobDet.Status = true;
//                   // jobDet.ActivityType = rm.ActivityType;
//                    jobDet.VisitPurpose = "Ordering";
//                    if (rm.Picture1 == "" || rm.Picture1 == null)
//                    {
//                        jobDet.Picture1 = null;
//                    }
//                    else
//                    {
//                        jobDet.Picture1 = ConvertIntoByte(rm.Picture1, "OrderPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
//                    }

//                    db.Jobs.Add(JobObj);
//                    db.SaveChanges();
//                    db.JobsDetails.Add(jobDet);
//                    db.SaveChanges();

//                    // Here StockItems is the array which is changed From JobItems Array due to Hassan 

//                    if (rm.StockItems != null && rm.StockItems.Count > 0)
//                    {
//                        foreach (var item in rm.StockItems)
//                        {
//                            db.JobItems.Add(
//                                new JobItem
//                                {
//                                    JobID = JobObj.ID,
//                                    ItemID = item.ItemID,
//                                    Quantity = item.Quantity,
//                                    Price = item.Price,
//                                    IsActive = true,
//                                    IsDeleted = false,
//                                    CreatedOn = localTime,
//                                  //  Status="Ordered",
//                                    CreatedBy = rm.SaleOfficerId,

//                                });
//                            db.SaveChanges();
//                        }
//                    }
//                }

//                return new Result<SuccessResponse>
//                {
//                    Data = null,
//                    Message = "Order Done Successfully",
//                    ResultType = ResultType.Success,
//                    Exception = null,
//                    ValidationErrors = null
//                };
//            }
//            catch (Exception ex)
//            {
//                Log.Instance.Error(ex, "Order API Failed");
//                return new Result<SuccessResponse>
//                {
//                    Data = null,
//                    Message = "Order API Failed",
//                    ResultType = ResultType.Exception,
//                    Exception = ex,
//                    ValidationErrors = null
//                };
//            }
//        }

//        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
//        {
//            byte[] bytes = Convert.FromBase64String(Base64);
//            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
//            ms.Write(bytes, 0, bytes.Length);
//            Image image = Image.FromStream(ms, true);
//            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
//            string filestoragename = DealerName + SendDateTime;
//            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
//            image.Save(outputPath, ImageFormat.Jpeg);

//            //string fileName = UserName + ".jpg";
//            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
//            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
//            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
//        }


//        public class SuccessResponse
//        {

//        }
//        public class DailyActivityRequest
//        {
//            public DailyActivityRequest()
//            {
//                StockItems = new List<JobItemModel>();
//            }
//            public int JobId { get; set; }
//            public int RetailerId { get; set; }

//           // public int RetailerID { get; set; }
//            public int SaleOfficerId { get; set; }
//            public string ActivityType { get; set; }
//            public int DiscountPercentage { get; set; }
//            public int OrderTotal { get; set; }
//            public int DiscountedTotal { get; set; }
//            public string PurposeofVisit { get; set; }
//            public string ActivityDetails { get; set; }
//            public string Picture1 { get; set; }
//            public string ReminderCancelStatus { get; set; }

//            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
//            public string NextVisitDate { get; set; }
//            public string Priorty { get; set; }
//            public string TentativeCloseDate { get; set; }
//            public List<JobItemModel> StockItems { get; set; }

//        }

//        public class JobItemModel
//        {
//            public int JobID { get; set; }
//            public int ItemID { get; set; }
//            public decimal Quantity { get; set; }
//            public decimal Price { get; set; }
//        }
//    }
//}



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
    public class DailyActivityForDistributorOfflineController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Job();
            var RemObj = new TblReminder();
            try
            {
                Dealer ret = db.Dealers.Where(r => r.ID == rm.RetailerId).FirstOrDefault();
                var regionalheadid = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerId).Select(x => x.RegionalHeadID).FirstOrDefault();
                if (ret != null)
                {
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                    if (rm.Type == "Ordering")
                    {
                        


                        JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                        JobObj.SaleOfficerID = rm.SaleOfficerId;
                        JobObj.RegionalHeadID = regionalheadid;
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
                        jobDet.RegionalHeadID = regionalheadid;
                        jobDet.SalesOficerID = JobObj.SaleOfficerID;
                        jobDet.DealerID = rm.RetailerId;
                        jobDet.RetailerID = 1;
                        jobDet.DiscountPercentage = rm.DiscountPercentage;
                        jobDet.DiscountedTotal = rm.DiscountedTotal;
                        jobDet.OrderTotal = rm.OrderTotal;
                        jobDet.JobDate = localTime;
                        jobDet.JobType = "Distributor Order";
                        jobDet.FollowupReason = rm.Followupreason;
                        jobDet.VisitPurpose = rm.Type;
                        jobDet.PRemarks = rm.Remarks;
                        jobDet.Status = true;
                        jobDet.ActivityDetails = "Offline";
                        // jobDet.ActivityType = rm.ActivityType;
                        jobDet.VisitPurpose = rm.Type;
                        if (rm.Picture1 == "" || rm.Picture1 == null)
                        {
                            jobDet.Picture1 = null;
                        }
                        else
                        {
                            jobDet.Picture1 = ConvertIntoByte(rm.Picture1, "OrderPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
                        }



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
                                        Quantity = item.Quantity,
                                        Price = item.Price,
                                        IsActive = true,
                                        IsDeleted = false,
                                        CreatedOn = localTime,
                                        CreatedBy = rm.SaleOfficerId,

                                    });
                                db.SaveChanges();
                            }
                        }
                    }
                    else if (rm.Type == "FollowupVisit")
                    {
                        JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                        JobObj.SaleOfficerID = rm.SaleOfficerId;
                        JobObj.RegionalHeadID = regionalheadid;
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
                        jobDet.RegionalHeadID = regionalheadid;
                        jobDet.SalesOficerID = JobObj.SaleOfficerID;
                        jobDet.DealerID = rm.RetailerId;
                        jobDet.RetailerID = 1;

                        jobDet.ActivityDetails = "Offline";
                        jobDet.JobDate = localTime;
                        jobDet.JobType = "Distributor Order";
                        jobDet.Status = true;
                        jobDet.FollowupReason = rm.Followupreason;
                        jobDet.VisitPurpose = rm.Type;
                        jobDet.PRemarks = rm.Remarks;
                        if (rm.Picture1 == "" || rm.Picture1 == null)
                        {
                            jobDet.Picture1 = null;
                        }
                        else
                        {
                            jobDet.Picture1 = ConvertIntoByte(rm.Picture1, "OrderPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
                        }



                        db.Jobs.Add(JobObj);
                        db.SaveChanges();
                        db.JobsDetails.Add(jobDet);
                        db.SaveChanges();
                    }
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
            }
            public int JobId { get; set; }
            public int RetailerId { get; set; }
            public int SaleOfficerId { get; set; }
            public string ActivityType { get; set; }
            public string PurposeofVisit { get; set; }
            public int DiscountPercentage { get; set; }
            public int OrderTotal { get; set; }
            public int DiscountedTotal { get; set; }
            public string ActivityDetails { get; set; }
            public string Picture1 { get; set; }
            public int Followupreason { get; set; }
            public string Type { get; set; }

            public string Remarks { get; set; }
            public string ReminderCancelStatus { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
            public string NextVisitDate { get; set; }
            public string Priorty { get; set; }
            public string TentativeCloseDate { get; set; }
            public List<JobItemModel> StockItems { get; set; }

        }

        public class JobItemModel
        {
            public int JobID { get; set; }
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}