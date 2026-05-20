using FOS.DataLayer;
using FOS.Setup;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class ApproveClaimsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ClaimID , int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
               
                if (ClaimID > 0)
                {
                    object[] param = { ClaimID };



                    var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.ID == ClaimID).FirstOrDefault();
                    if (result != null) 
                    {

                        result.Status = "Approved";
                        result.ApprovedBy=SOID;
                        dbContext.SaveChanges();
                    }

                    return Ok(new
                    {
                        Message = "Claim approved successfully.",
                        ResultType = ResultType.Success,
                       
                    });





                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ApproveClaimsController GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                ClaimSummery = paramm
            });

        }


    }
}