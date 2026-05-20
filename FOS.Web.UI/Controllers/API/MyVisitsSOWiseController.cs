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
    public class MyVisitsSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string Date, int SegmentID)
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
                    var Url = "http://116.58.33.11:81/";
                    object[] param = { SOID };


                    if (SegmentID == 1)
                    {
                        var result = dbContext.Tbl_TradeVisitsFinal.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromToday && x.CreatedAt <= dtToToday && x.IsActive==true).Select(x => new
                        {
                            ID=x.ID,
                            Name=x.Retailer.ShopName,
                            OrderValue=x.OrderValue,
                            Remarks=x.Remarks,
                            Picture1 = Url + "\\Images\\brighto.jpeg",
                            Picture2 = Url + "\\Images\\brighto.jpeg",


                        }).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                VisitSummery = result

                            });
                        }
                    }

                    if (SegmentID == 2)
                    {
                        var result = dbContext.Tbl_HousingVisits.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromToday && x.CreatedAt <= dtToToday && x.IsActive == true).Select(x => new
                        {
                            ID = x.ID,
                            Name = x.Retailer.ShopName,
                           ApproxCoveredArea=x.ApproxCoveredArea,
                            Remarks = x.Remarks,
                            Picture1= Url + x.Picture1,
                            Picture2= Url + x.Picture2,


                        }).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                VisitSummery = result

                            });
                        }
                    }

                    if (SegmentID == 3)
                    {
                        var result = dbContext.Tbl_CorporateVisits.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromToday && x.CreatedAt <= dtToToday && x.IsActive == true).Select(x => new
                        {
                            ID = x.ID,
                            Name = x.Retailer.ShopName,
                            ProjectTitle = x.ProjectTitle,
                            Remarks = x.Remarks,
                            Picture1 = Url + "\\Images\\brighto.jpeg",
                            Picture2 = Url + "\\Images\\brighto.jpeg",

                        }).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                VisitSummery = result

                            });
                        }
                    }


                    if (SegmentID == 5)
                    {
                        var result = (from visit in dbContext.Tbl_AllPurposeVisits
                                      join retailer in dbContext.Retailers on visit.CustomerID equals retailer.ID
                                      where visit.SOID == SOID
                                            && visit.CreatedOn >= dtFromToday
                                            && visit.CreatedOn <= dtToToday
                                            && visit.IsActive==true
                                      select new
                                      {
                                          ID = visit.ID,
                                          Name = retailer.ShopName,
                                          Remarks = visit.Remarks,
                                          Picture1 = Url + "\\Images\\brighto.jpeg",
                                          Picture2 = Url + "\\Images\\brighto.jpeg",
                                      }).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                VisitSummery = result

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
                MyOrderListSummery = paramm
            });

        }


    }
}