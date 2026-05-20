using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetApprovalsListController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ClaimID,int LoginSOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<ApprovalsList> CustomerValidate = new List<ApprovalsList>();
                ApprovalsList cty;
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (ClaimID > 0)
                {
                    object[] param = { ClaimID };

                  

                    var result = dbContext.Tbl_ClaimsApproval.Where(x=>x.ClaimID==ClaimID && x.IsActive==true ).OrderBy(x => x.ApprovedAt).ToList();

                    foreach (var dbCty in result)
                    {
                        cty = new ApprovalsList();
                        cty.ID = dbCty.ID;
                        cty.Remarks = dbCty.Remarks;
                        cty.ApprovedStatus = dbCty.ApprovedStatus;
                        cty.ApprovedAt = dbCty.ApprovedAt.HasValue? dbCty.ApprovedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
                        cty.SOName = dbContext.SaleOfficers.Where(x=>x.ID==dbCty.ApprovedBy).Select(x=>x.Name).FirstOrDefault();

                        cty.CanDelete = dbCty.ApprovedBy == LoginSOID ? true : false;

                        CustomerValidate.Add(cty);
                    }

                    if (CustomerValidate != null && CustomerValidate.Count > 0)
                    {
                        return Ok(new
                        {
                            ApprovalList = CustomerValidate

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ApprovalList = paramm
            });

        }


    }
}