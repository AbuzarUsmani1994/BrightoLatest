using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class ResetLatLongForDistributorsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<LoginResponse> Post(CheckInLatLongRequest req)
        {
            Dealer Ret = new Dealer();
            if (req.ID > 0 && !string.IsNullOrEmpty(req.Latitude.ToString()) && !string.IsNullOrEmpty(req.Longitude.ToString()))
            {
                Ret = db.Dealers.Where(u => u.ID == req.ID).FirstOrDefault();
                Ret.Latitude = req.Latitude;
                Ret.Longitude = req.Longitude;
                Ret.Location= req.Latitude + "," + req.Longitude;
                db.SaveChanges();

                return new Result<LoginResponse>
                {
                    Data = null,
                    Message = "Lat Long Reset successful",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            else
            {
                return new Result<LoginResponse>
                {
                    Data = new LoginResponse(),
                    Message = "Lat Long Reset unsuccessful",
                    ResultType = ResultType.Failure,
                    Exception = null,
                    ValidationErrors = null
                };
            }
        }


        public class CheckInLatLongRequest
        {
            public int ID { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public decimal Location { get; set; }
        }

    }
}