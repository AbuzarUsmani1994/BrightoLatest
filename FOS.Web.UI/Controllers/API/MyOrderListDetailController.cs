using FOS.DataLayer;
using FOS.Setup;
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
    public class MyOrderListDetailController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int JobID)
        {
            List<MMCFollowUpReasonDetail> MAinCat = new List<MMCFollowUpReasonDetail>();
            MMCFollowUpReasonDetail cty;
            List<Sp_MyOrderListDetail_Result> result = new List<Sp_MyOrderListDetail_Result>();
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (JobID > 0)
                {
                    object[] param = { JobID };

                    var Reason = db.JobsDetails.Where(x => x.JobID == JobID).FirstOrDefault();
                    if (Reason.VisitPurpose == "Ordering" && Reason.ReasonForNoSaleID==0)
                    {


                         result = dbContext.Sp_MyOrderListDetail(JobID).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                MyOrderListDetail = result

                            });
                        }
                    }
                    else
                    {
                        result = dbContext.Sp_MyOrderListDetail(JobID).ToList();

                        if (result != null && result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                cty = new MMCFollowUpReasonDetail();

                                cty.ItemName = item.ItemName;
                                cty.StockQuantity = item.StockQuantity;
                                cty.OrderQuantity = item.OrderQuantity;
                                cty.VisitDate = item.VisitDate;
                                var ID= db.JobsDetails.Where(x => x.JobID == JobID).Select(x => x.ReasonForNoSaleID).FirstOrDefault();
                                cty.ReasonForNoSaleID = ID;
                                cty.ReasonForNoSale = db.Tbl_NoSaleReason.Where(x => x.ID == ID).Select(x => x.Name).FirstOrDefault();
                                MAinCat.Add(cty);
                            }
                            
                        }
                        else
                        {
                            cty = new MMCFollowUpReasonDetail();

                            
                            var ID = db.JobsDetails.Where(x => x.JobID == JobID).FirstOrDefault();
                            cty.ReasonForNoSaleID = ID.ReasonForNoSaleID;
                            cty.VisitDate = ID.Lastvisitdate;
                            cty.ReasonForNoSale = db.Tbl_NoSaleReason.Where(x => x.ID == ID.ReasonForNoSaleID).Select(x => x.Name).FirstOrDefault();
                            MAinCat.Add(cty);

                        }

                        return Ok(new
                        {
                            MyOrderListDetail = MAinCat

                        });

                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                MyOrderListDetail = paramm
            });

        }


    }
}