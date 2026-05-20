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
    public class GetSalesClaimDeleteController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

       
        public Result<SuccessResponse> Get(int ClaimID)
        {
            try
            {
                // Find the sales claim
                var retailerObj = db.Tbl_SalesClaimMaster.FirstOrDefault(u => u.ID == ClaimID);

                // Check if record exists
                if (retailerObj == null)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Sales Claim not found",
                        ResultType = ResultType.Warning,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                // Check if claim is approved
                if (retailerObj.ClaimManagerLatestStatus == "Approved")
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Claim is Approved. It cannot be deleted",
                        ResultType = ResultType.Warning,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                // Soft delete the record
                retailerObj.IsActive = false;
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Sales Claim Deleted Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Delete Sales Claim API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Delete Sales Claim API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        // Dispose the database context to free resources
        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}