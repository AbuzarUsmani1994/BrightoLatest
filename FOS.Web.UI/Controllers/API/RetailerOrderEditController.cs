using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using System.Web.Services.Protocols;

namespace FOS.Web.UI.Controllers.API
{
    public class RetailerOrderEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        {
            var JobObj = new Job();
            var JobDetail= new JobsDetail();
            var JobItem = new JobItem();

            try
            {
                var Type = db.JobsDetails.Where(x => x.JobID == rm.JobID).FirstOrDefault();
                if (Type.Dispatchstatus == "Ordered")
                {

                    var Result1 = db.JobItems.Where(x => x.JobID == rm.JobID).ToList();

                    foreach (var item in Result1)
                    {
                        db.JobItems.Remove(item);
                        db.SaveChanges();
                    }

                    //var Result2 = db.JobsDetails.Where(x => x.JobID == rm.JobID).FirstOrDefault();
                    ////var date = Result2.JobDate;
                    //db.JobsDetails.Remove(Result2);
                    //db.SaveChanges();


                    //var Result3 = db.Jobs.Where(x => x.ID == rm.JobID).FirstOrDefault();
                    //db.Jobs.Remove(Result3);
                    //db.SaveChanges();

                    //    JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    //    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    //    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    //    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    //    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                    //    JobObj.SaleOfficerID = rm.SaleOfficerId;

                    //      JobObj.RegionalHeadID = rm.RegionalHeadID;
                    //   // JobObj.DistributorID = rm.ShopID;



                    //    JobObj.CreatedDate = localTime;
                    //   // JobOb = JobObj.SaleOfficerID;
                    //    JobObj.IsActive = true;
                    //    JobObj.IsDeleted = false;


                    //    JobDetail.JobID = JobObj.ID;
                    //    JobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                    //    JobDetail.SalesOficerID = JobObj.SaleOfficerID;
                    //    JobDetail.RetailerID = rm.RetailerID;
                    //    JobDetail.JobDate = localTime;
                    //    JobDetail.DiscountPercentage = rm.DiscountPercentage;
                    //    JobDetail.DiscountedTotal = rm.DiscountedTotal;
                    //    JobDetail.OrderTotal = rm.OrderTotal;
                    //JobDetail.ActivityDetails = "Online";
                    //JobDetail.JobType = "Retailer Order";
                    //    JobDetail.Status = true;
                    //    JobDetail.VisitPurpose = "Ordering";


                    //    db.Jobs.Add(JobObj);
                    //    db.SaveChanges();
                    //    db.JobsDetails.Add(JobDetail);
                    //    db.SaveChanges();
                    List<StockItemModel> CustomerValidate = new List<StockItemModel>();
                    StockItemModel cty;
                    if (rm.StockItems != null && rm.StockItems.Count > 0)
                    {
                        var filteredList = rm.StockItems.GroupBy(e => e.ItemID).Select(g =>
                        {
                            var item = g.First();
                            return new StockItemModel
                            {
                                ItemID = item.ItemID,
                                Quantity = g.Sum(e => e.Quantity),

                                Price = item.Price
                            };
                        }).ToList();





                        foreach (var item in filteredList)
                        {
                            db.JobItems.Add(
                                new JobItem
                                {
                                    JobID = rm.JobID,
                                    ItemID = item.ItemID,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedOn = DateTime.UtcNow.AddHours(5),
                                    CreatedBy = rm.SaleOfficerId,

                                });
                            db.SaveChanges();
                        }
                    }


                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Retailer Order Edit Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
                else
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Retailer Order Edit Failed As It Is Invoiced by Dealer",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Retailer Order Edit Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Retailer Order Edit Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
            public DailyActivityRequest()
            {
                StockItems = new List<StockItemModel>();
            }
            public int JobID { get; set; }

            public int RetailerID { get; set; }
            public int SaleOfficerId { get; set; }
            public int RegionalHeadID { get; set; }
            public int DiscountPercentage { get; set; }
            public int OrderTotal { get; set; }
            public int DiscountedTotal { get; set; }
            public List<StockItemModel> StockItems { get; set; }

        }

        public class StockItemModel
        {
     
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }

        }
    }
}