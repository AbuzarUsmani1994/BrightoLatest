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
    public class GetCurrentRouteOfSOController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (SOID > 0)
                {
                    object[] param = { SOID };

                    // RetailerData cty;

                    var result = dbContext.TBL_RouteSelection.Where(r => r.SOID == SOID && r.CreatedOn >= dtFromToday && r.CreatedOn <= dtToToday && r.IsActive == true).Select(x => new
                    {
                        ID = x.ID,
                        CityID = x.CityID,
                        RegionID = x.RegionID,
                      

                    }).FirstOrDefault();


                    if (result != null)
                    {
                        return Ok(new
                        {
                            RouteInfo = result

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
                RouteInfo = paramm
            });

        }


    }
}