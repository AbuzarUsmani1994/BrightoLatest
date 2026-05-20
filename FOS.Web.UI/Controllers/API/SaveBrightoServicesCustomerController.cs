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
using System.Transactions;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SaveBrightoServicesCustomerController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        {
            // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Tbl_FannanCustomer();
          
            try
            {


               
                JobObj.ContactPerson = rm.Name;
                JobObj.SiteCode = rm.RetailerCode;
                JobObj.CNIC = rm.CNIC;
                JobObj.Email = rm.Email;

                JobObj.SiteName = rm.ShopName;
                JobObj.Address = rm.Address;

                JobObj.Phone1 = rm.Phone;
                JobObj.RegionID = rm.RegionID;
                JobObj.IsActive = true;
                JobObj.CreatedOn = DateTime.UtcNow.AddHours(5);




                db.Tbl_FannanCustomer.Add(JobObj);
                db.SaveChanges();







                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Brighto Services Customer Added Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Brighto Services Customer API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Claim Approved API Failed",
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

            public string Name { get; set; }
            public string RetailerCode { get; set; }
            public string CNIC { get; set; }
            public string Email { get; set; }
            public string ShopName { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }

            public int RegionID { get; set; }



        }


    }
}