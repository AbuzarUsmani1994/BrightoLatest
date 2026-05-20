using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class PasswordEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(PasswordEditmodel rm)
        {
            SaleOfficer retailerObj = new SaleOfficer();
            try
            {
               
                    //ADD New Retailer 
                    retailerObj = db.SaleOfficers.Where(u => u.ID == rm.SOID).FirstOrDefault();

                    retailerObj.Password = rm.password;
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


                    retailerObj.LastUpdate = localTime;



                  

                    db.SaveChanges();

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Password Edit Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
              
               

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Password Edit API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Password Edit API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }



        public class SuccessResponse
        {

        }
        public class PasswordEditmodel
        {
            public int SOID { get; set; }
            public string Token { get; set; }
            public string password { get; set; }

        }

    }
}
