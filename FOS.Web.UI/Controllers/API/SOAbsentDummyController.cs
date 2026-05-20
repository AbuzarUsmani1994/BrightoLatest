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
    public class SOAbsentDummyController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get()
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                //DateTime dtFromToday = dtFromTodayUtc.Date;
                //DateTime dtToToday = dtFromToday.AddDays(1);


                DateTime dtFromToday = Convert.ToDateTime("2019-04-01");
                DateTime dtToToday = Convert.ToDateTime("2019-04-30");

                // object[] param = { SOID };

                var result = dbContext.Sp_SalesOfficerAllOrders1_1(dtFromToday, dtToToday,0,0).ToList();

                var soID = result.Select(s => s.SaleOfficerID);


                var FULLSO = dbContext.SaleOfficers.Where(t => t.IsActive).ToList();

                var final = FULLSO.Where(p => !result.Any(p2 => p2.SaleOfficerID == p.ID)).Select(i => new
                {

                    i.ID,
                    i.Name



                });




                if (final != null)
                {
                    return Ok(new
                    {
                        AbsentSO = final

                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                AbsentSO = paramm
            });

        }


    }
}