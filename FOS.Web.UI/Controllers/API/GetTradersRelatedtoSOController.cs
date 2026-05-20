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
    public class GetTradersRelatedtoSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID, int SegmentID,string DateFrom,string DateTo)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = Convert.ToDateTime(DateFrom);


                DateTime dtFromToday = dtFromTodayUtc.Date;
                var todatedumm= Convert.ToDateTime(DateTo);

                DateTime dtFromTodate = todatedumm.Date;
                DateTime todate = dtFromTodate.AddDays(1);
            

                if (SOID > 0)
                {
                    object[] param = { SOID };

                    var customers = (from hv in dbContext.Tbl_SalesClaimMaster
                                    
                                     join r in dbContext.Retailers
                                         on hv.TradePartyID equals r.ID
                                     where hv.SOID == SOID
                                        && hv.DateSelected >= dtFromToday
                                        && hv.DateSelected <= todate
                                        && hv.IsActive==true
                                     select new
                                     {
                                         ID = r.ID,
                                         Name = r.ShopName
                                     })
                                 .Distinct()
                                 .ToList();

                    customers.Insert(0, new { ID = 0, Name = "All" });
                    if (customers != null && customers.Count > 0)
                    {
                        return Ok(new
                        {
                            TraderRelatedToSO = customers
                        });
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DistributorRelatedToSO GET API Failed");
            }
            object[] paramm = {  };
            return Ok(new
            {
                TraderRelatedToSO = paramm
            });

        }
    }
}