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
    public class StockTakingEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        {
            var JobObj = new Tbl_MasterStock();

            try
            {
                var Result2 = db.tbl_StockDetail.Where(x => x.StockMasterID == rm.JobID).ToList();

                foreach (var item in Result2)
                {
                    db.tbl_StockDetail.Remove(item);
                    db.SaveChanges();
                }

                var Result1 = db.Tbl_MasterStock.Where(x => x.ID == rm.JobID).FirstOrDefault();
                db.Tbl_MasterStock.Remove(Result1);
                db.SaveChanges();

                JobObj.ID = db.Tbl_MasterStock.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                JobObj.SaleOfficerID = rm.SaleOfficerId;
                JobObj.RegionalHeadID = rm.RegionalHeadID;
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
                                StockTakingTime = localTime,

                            });
                        db.SaveChanges();
                    }
                }


                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Stock Taking Edit Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Stock Taking Edit Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Stock Taking Edit Failed",
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
            public List<StockItemModel> StockItems { get; set; }

        }

        public class StockItemModel
        {
     
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            
        }
    }
}