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
    public class DeleteClaimController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(int ClaimID)
        {
            try
            {
                // Fetch the claim record first
                var claim = db.Tbl_SalesClaimMaster.Where(a => a.ID == ClaimID).FirstOrDefault();

                if (claim.ClaimManagerLatestStatus == "Approved")
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Claim is Approved. It cant be deleted",
                        ResultType = ResultType.Warning,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                else if (claim == null)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Claim not found",
                        ResultType = ResultType.Warning,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                // Date validation logic for deletion
                //string validationError;
                //if (!IsDateValidForDeletion(Convert.ToDateTime(claim.DateSelected), out validationError))
                //{
                //    return new Result<SuccessResponse>
                //    {
                //        Data = null,
                //        Message = validationError,
                //        ResultType = ResultType.Warning,
                //        Exception = null,
                //        ValidationErrors = null
                //    };
                //}

                // Proceed with deletion if validation passes
                claim.IsActive = false;
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Claim Deleted Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Delete Claim API Failed");
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

        private bool IsDateValidForDeletion(DateTime selectedDate, out string errorMessage)
        {
            DateTime currentDate = DateTime.Now;
            errorMessage = null;

            // Check if selected date is in previous month
            if (selectedDate.Year < currentDate.Year ||
                (selectedDate.Year == currentDate.Year && selectedDate.Month < currentDate.Month))
            {
                // For previous month data, only allow deletion if current date is on or before 10th of current month
                if (currentDate.Day > 10)
                {
                    errorMessage = "Previous month claims can only be deleted until the 10th of current month";
                    return false;
                }
            }
            // Check if selected date is in future
            else if (selectedDate > currentDate)
            {
                errorMessage = "Future claims cannot be deleted";
                return false;
            }

            // Current month data is always allowed for deletion
            return true;
        }

        public class SuccessResponse
        {
        }
    }
}