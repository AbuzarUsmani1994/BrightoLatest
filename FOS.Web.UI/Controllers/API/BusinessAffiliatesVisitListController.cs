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
    public class BusinessAffiliatesVisitListController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string DateFrom, string DateTo)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //var SOType= db.SaleOfficers.Where(x=>x.ID==SOID).Select(x=>x.so)


                DateTime dtFromTodayUtc = Convert.ToDateTime(DateFrom);

                DateTime dtFromToday = dtFromTodayUtc.Date;

                DateTime dttoTodayUtc = Convert.ToDateTime(DateTo);
                DateTime dtToToday = dttoTodayUtc.AddDays(1);

                if (SOID > 0)
                {
                  
                    object[] param = { SOID };


                  

                        var result = dbContext.Tbl_BusinessAffiliatesVisits
                          .Where(x => x.SOID == SOID && x.VisitDate >= dtFromToday && x.VisitDate <= dtToToday && x.IsActive == true)
                          .AsEnumerable() // Moves execution to memory
                          .Select(x => new
                          {
                              ID = x.ID,
                              CustomerName = dbContext.Tbl_BusinessAffiliates
                                      .Where(y => y.ID == x.BusinessAffiliateID)
                                      .Select(y => y.BusinessName)
                                      .FirstOrDefault(),
                                      VisitDate = x.VisitDate,
                                      SubmittedDate = x.CreatedDate,
                                 
                              Remarks =x.Remarks,
                              
                          })
                           .OrderBy(x => x.CustomerName)
                          .ToList();

                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                BusinessAffiliatesVisitsList = result

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
                BusinessAffiliatesVisitsList = paramm
            });

        }


    }
}