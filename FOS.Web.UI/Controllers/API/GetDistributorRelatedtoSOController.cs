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
    public class GetDistributorRelatedtoSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID, int RangeID,string DateFrom,string DateTo)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = Convert.ToDateTime(DateFrom);


                DateTime dtFromToday = dtFromTodayUtc.Date;
                var todatedumm= Convert.ToDateTime(DateTo);

                DateTime dtFromTodate = todatedumm.Date;
                DateTime todate = dtFromTodate.AddDays(1);
               // DateTime fromdate = todate.AddDays(-10);

                if (SOID > 0)
                {
                    object[] param = { SOID };


                        var result = dbContext.sp_GetDistributorListInDSR(SOID, dtFromToday, todate).ToList();

                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                DistributorRelatedToSO = result

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
                DistributorRelatedToSO = paramm
            });

        }
    }
}