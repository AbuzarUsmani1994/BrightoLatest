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
    public class SubmitSiteAllocationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            Tbl_FannanCustomerAssign jobDet = new Tbl_FannanCustomerAssign();
            


            //var Lastdata = db.JobsDetails.Where(x => x.RetailerID == rm.RetailerId).OrderByDescending(x => x.ID).FirstOrDefault();
            try
            {
                

                jobDet.SOID = rm.SOID;
                
                jobDet.CustomerID = rm.CustomerID;
                jobDet.IsActive = true;
                jobDet.CreatedOn = DateTime.UtcNow.AddHours(5);
               

             



                db.Tbl_FannanCustomerAssign.Add(jobDet);
                db.SaveChanges();







                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Customer Assigned Successfully",
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



        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
           
           
           
            public int SOID { get; set; }
            public int CustomerID { get; set; }
          

        }

    }
}