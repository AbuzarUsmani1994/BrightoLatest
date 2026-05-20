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
    public class DeleteApprovalListController : ApiController
    {
      

     
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DeleteApprovalRequest request)
        { 
            try
            {
                var retailers = db.Tbl_ClaimsApproval.Where(a => a.ID == request.ClaimApprovalID).FirstOrDefault();
                retailers.IsActive = false;
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Approval Deleted Successfully",
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
                    Message = "Delete Request Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }


    }
    public class DeleteApprovalRequest
    {
        public int ClaimApprovalID { get; set; }
    }
}