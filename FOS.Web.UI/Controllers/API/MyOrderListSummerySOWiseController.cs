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
    public class MyOrderListSummerySOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string Date)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //var SOType= db.SaleOfficers.Where(x=>x.ID==SOID).Select(x=>x.so)


                DateTime dtFromTodayUtc = Convert.ToDateTime(Date);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                if (SOID > 0)
                {
                    object[] param = { SOID };
                    
                    
                        var result = dbContext.Sp_MyOrderListSummery1_6(SOID,dtFromToday,dtToToday).ToList();
                    
                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            MyOrderListSummery = result
                            
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
                MyOrderListSummery = paramm
            });

        }


    }
}