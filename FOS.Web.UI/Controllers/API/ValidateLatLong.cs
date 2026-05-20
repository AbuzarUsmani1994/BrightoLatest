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
    public class ValidateLatLongController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<LoginResponse> Post(CheckInLatLongRequest req)
        {
            if (req.ID > 0 && !string.IsNullOrEmpty(req.Latitude.ToString()) && !string.IsNullOrEmpty(req.Longitude.ToString()))
            {
                object[] param = { req.Latitude, req.Longitude, req.ID };
                bool flag = false;
                decimal distance = 0;
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var result = dbContext.Database.SqlQuery<SpReturn>("exec spValidateCheckInLatLong @latitude, @longitude, @shopId",
                                    new SqlParameter("latitude", param[0]),
                                    new SqlParameter("longitude", param[1]),
                                    new SqlParameter("shopId", param[2])).ToList();
                    if (result.Count() > 0)
                    {
                        distance = result.FirstOrDefault().distance;
                        flag = true;
                    }
                }
                if (flag)
                {

                    return new Result<LoginResponse>
                    {
                        Data = null,
                        Message = "CheckIn successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
                else
                {

                    return new Result<LoginResponse>
                    {
                        Data = null,
                        Message = "CheckIn is not successful",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
            }
            else
            {
                return new Result<LoginResponse>
                {
                    Data = new LoginResponse(),
                    Message = "Please provide Respective LatLong",
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