using FOS.DataLayer;
using FOS.Setup;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class DeleteBusinessAffiliatesListController : ApiController
    {
      

     
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(BusinessAffiliatesVisitRequest request)
        { 
            try
            {
                var retailers = db.Tbl_BusinessAffiliatesVisits.Where(a => a.ID == request.BusinessAffiliatesVisitID).FirstOrDefault();
                retailers.IsActive = false;
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Business Affiliates Visit Deleted Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Business Affiliates Visit API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Business Affiliates Visit Delete Request Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }


    }
    public class BusinessAffiliatesVisitRequest
    {
        public int BusinessAffiliatesVisitID { get; set; }
    }
}