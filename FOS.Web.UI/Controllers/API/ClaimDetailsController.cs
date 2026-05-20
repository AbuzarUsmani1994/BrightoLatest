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
    public class ClaimDetailsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ClaimID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //var SOType= db.SaleOfficers.Where(x=>x.ID==SOID).Select(x=>x.so)


                //DateTime dtFromTodayUtc = Convert.ToDateTime(DateFrom);

                //DateTime dtFromToday = dtFromTodayUtc.Date;

                //DateTime dttoTodayUtc = Convert.ToDateTime(DateTo);
                //DateTime dtToToday = dttoTodayUtc.AddDays(1);

                if (ClaimID > 0)
                {
                    object[] param = { ClaimID };


                   
                        var result = dbContext.Tbl_ClaimDetail.Where(x => x.ClaimMasterID == ClaimID ).Select(x => new
                        {
                            ID=x.ID,
                            ClaimID = x.ClaimMasterID,
                            ProductID=x.ProductID,
                            ProductName=dbContext.Tbl_ProductDetail.Where(y=>y.ID== x.ProductID).Select(y=>y.Product_Desc).FirstOrDefault(),
                            Drum=x.Drum,
                            Gallon=x.Gallon,
                            Quarter=x.Quarter,
                            SaleLineValue=x.SaleLineValue,
                            CreatedDate=x.CreatedOn
                            


                        }).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimDetail = result

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
                ClaimDetail = paramm
            });

        }


    }
}