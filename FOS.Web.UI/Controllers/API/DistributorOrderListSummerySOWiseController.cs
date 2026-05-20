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
    public class DistributorOrderListSummerySOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (SOID > 0)
                {
                    object[] param = { SOID };

                    DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                    DateTime dtFromToday = dtFromTodayUtc.Date;
                    DateTime dtToToday = dtFromToday.AddDays(1);

                    var result = dbContext.Sp_MyOrderListDistributorSummery1_7(dtFromToday,dtToToday,SOID).ToList();
                    
                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            DistributorOrderListSummery = result
                            
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
                DistributorOrderListSummery = paramm
            });

        }


    }
}