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

    public class DashboardDataController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID, int TypeID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (SOID > 0)
                {
                    object[] param = { SOID };
                    DateTime now = DateTime.UtcNow.AddHours(5);
                    var startDate = new DateTime(now.Year, now.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var today = DateTime.Today;
                    var month = new DateTime(today.Year, today.Month, 1);
                    var first = month.AddMonths(-1);
                    var last = month.AddDays(-1);
                   // var rangeid = db.SaleOfficers.Where(x => x.ID == SOID).Select(x => x.RangeID).FirstOrDefault();

                    var result = dbContext.sp_GetDashboardData(startDate, endDate, SOID, first, last, 6,TypeID).ToList();

                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            FinalDashboardData = result

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }

            return Ok(new
            {
                FinalDashboardData = new { }
            });

        }


    }
}