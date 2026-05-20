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
    public class CoverageListController : ApiController
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
                    var Url = "http://116.58.33.11:81/";
                    object[] param = { SOID };


                    var result = dbContext.Tbl_MasterCoverage
                          .Where(x => x.SOID == SOID &&
                                 x.CreatedOn >= dtFromToday &&
                                 x.CreatedOn <= dtToToday &&
                                 x.IsActive==true)
                          .Select(x => new
                          {
                              x.ID,
                              x.CustomerID,  // We'll join with customer info
                              x.CreatedOn
                          })
                          .AsEnumerable() // Switch to client evaluation after getting minimal data
                          .Select(x => new
                          {
                              ID = x.ID,
                              CoverageID = x.ID,
                              CustomerName = dbContext.Retailers
                                  .Where(y => y.ID == x.CustomerID)
                                  .Select(y => y.ShopName)
                                  .FirstOrDefault(),
                              CreatedDate = x.CreatedOn,
                              CustomerID = x.CustomerID,
                              TotalLiters = dbContext.Tbl_DetailCoverage
                                  .Where(z => z.CoverageMasterID == x.ID)
                                  .Sum(z => (decimal?)z.Liters) ?? 0  // Handle null sums
                          })
                          .OrderBy(x => x.CustomerName)
                          .ToList();

                    if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                CoverageList = result

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
                CoverageList = paramm
            });

        }


    }
}