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
    public class StockTakingController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        {
            //JobsDetail jobDet = new JobsDetail();
            var JobObj = new Tbl_MasterStock();
          //  var RemObj = new TblReminder();
            try
            {
                Dealer ret = db.Dealers.Where(r => r.ID == rm.RetailerID).FirstOrDefault();
                if (ret != null)
                {
                    JobObj.ID = db.Tbl_MasterStock.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    JobObj.SaleOfficerID = rm.SaleOfficerId;
                    JobObj.RegionalHeadID = (int) ret.SaleOfficer.RegionalHeadID;
                    JobObj.DistributorID = rm.RetailerID;
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


                    JobObj.CreatedOn = localTime;
                    JobObj.Createdby = JobObj.SaleOfficerID;
                    JobObj.IsActive = true;
                    JobObj.IsDeleted = false;
                    db.Tbl_MasterStock.Add(JobObj);
                    db.SaveChanges();
                    //db.JobsDetails.Add(jobDet);
                    //db.SaveChanges();
                    //db.TblReminders.Add(RemObj);
                    //db.SaveChanges();

                    if (rm.StockItems != null && rm.StockItems.Count > 0)
                    {

                        foreach (var item in rm.StockItems)
                        {
                            db.tbl_StockDetail.Add(
                                new tbl_StockDetail
                                {
                                    StockMasterID = JobObj.ID,
                                    itemID = item.ItemID,
                                    Quantity = item.Quantity,
                                   // Price = item.Price,
                                    IsActive = true,
                                    IsDeleted = false,
                                    CreatedOn = localTime,
                                    Createdby = rm.SaleOfficerId,
                                    StockTakingTime =localTime,

                                });
                            db.SaveChanges();
                        }
                    }
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Stock Taking Done Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Stock Taking API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Stock Taking API Failed",
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
            public int JobId { get; set; }
            public int RetailerID { get; set; }
            public int SaleOfficerId { get; set; }
            public string ActivityType { get; set; }
            public string PurposeofVisit { get; set; }
            public string ActivityDetails { get; set; }
            public string ReminderCancelStatus { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
            public string NextVisitDate { get; set; }
            public string Priorty { get; set; }
            public string TentativeCloseDate { get; set; }
            public List<StockItemModel> StockItems { get; set; }

        }

        public class StockItemModel
        {
            public int StockID { get; set; }
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            
        }
    }
}