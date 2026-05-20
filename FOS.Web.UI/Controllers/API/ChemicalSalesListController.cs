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
    public class ChemicalSalesListController : ApiController
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


                  

                        var result = dbContext.Tbl_ChemSaleMaster
                          .Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.IsActive == true)
                          .AsEnumerable() // Moves execution to memory
                          .Select(x => new
                          {
                              ID = x.ID,
                              CustomerName = dbContext.Tbl_ChemCustomerInfo
                                      .Where(y => y.ID == x.CustomerID)
                                      .Select(y => y.BusinessName)
                                      .FirstOrDefault(),
                                      OrderDate=x.OrderDate,
                                      TotalQuantity=x.TotalQuantity,
                                      TotalPrice=x.TotalPrice,

                              TotalOrderValue = x.TotalOrderValue,
                              Picture = string.IsNullOrEmpty(x.Picture)
                                      ? Url + "\\Images\\brighto.jpeg"
                                      : Url + x.Picture,
                              Remarks =x.Remarks,
                              
                          })
                           .OrderBy(x => x.CustomerName)
                          .ToList();

                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ChemicalSaleList = result

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
                ChemicalSaleList = paramm
            });

        }


    }
}