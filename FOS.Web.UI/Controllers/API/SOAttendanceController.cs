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
    public class SOAttendanceController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            SOAttendance JobObj = new SOAttendance();
          
            try
            {

                JobObj.SOID = rm.SOID;
                JobObj.RegionID = rm.RegionID;
                JobObj.CityID = rm.CityID;
               // JobObj.Type = rm.Type;
                if (rm.Type == "Market start")
                {
                    JobObj.Type = rm.Type;
                    JobObj.MarketStartLat = rm.Latitude;
                    JobObj.MarketStartLong = rm.Longitude;
                    JobObj.MarketStartLatlong = rm.Latitude + "," + rm.Longitude;
                }

                else if (rm.Type == "Market close")
                {
                    JobObj.Type = rm.Type;
                    JobObj.MarketCloseLat = rm.Latitude;
                    JobObj.MarketCloseLong = rm.Longitude;
                    JobObj.MarketCloseLatlong = rm.Latitude + "," + rm.Longitude;
                }
                else
                {
                    JobObj.Type = rm.Type;

                }
                JobObj.DealerID = rm.DistributorID;
                JobObj.SelectedSOID = rm.SelectedSOID;
                JobObj.SegmentTypeID = rm.SegmentType;

                JobObj.RetailerID = rm.RetailerID;
                
                JobObj.CreatedAt = DateTime.UtcNow.AddHours(5);
                db.SOAttendances.Add(JobObj);
                db.SaveChanges();
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Attendence Punched Successfully",
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
                    Message = "Attendence Punched Failed",
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
       
            public int SOID { get; set; }
            public int SelectedSOID { get; set; }
            public int RetailerID { get; set; }
            public int RegionID { get; set; }
            public int SegmentType { get; set; }
            public int CityID { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public int DistributorID { get; set; }
            public string Type { get; set; }
        
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