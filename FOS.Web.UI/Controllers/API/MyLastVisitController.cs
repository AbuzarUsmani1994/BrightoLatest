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
    public class MyLastVisitController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int CustomerID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                var SegmentType = db.Retailers.Where(x => x.ID == CustomerID).Select(x => x.SegmentTypeID).FirstOrDefault();


                //DateTime dtFromTodayUtc = Convert.ToDateTime(Date);

                //DateTime dtFromToday = dtFromTodayUtc.Date;
                //DateTime dtToToday = dtFromToday.AddDays(1);

                if (CustomerID > 0)
                {
                    object[] param = { CustomerID };

                    if (SegmentType == 2)
                    {
                        var result = dbContext.Sp_MyHousinglastVisit(CustomerID).ToList();

                        if (result != null)
                        {
                            return Ok(new
                            {
                                MyLastVisit = result

                            });
                        }
                    }
                    else 
                    {
                        var result = dbContext.Sp_MyCoorporatelastVisit(CustomerID).ToList();

                        if (result != null)
                        {
                            return Ok(new
                            {
                                MyLastVisit = result

                            });
                        }

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
                MyLastVisit = paramm
            });

        }


    }
}