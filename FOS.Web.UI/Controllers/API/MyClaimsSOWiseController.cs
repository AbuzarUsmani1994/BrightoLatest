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
    public class MyClaimsSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string DateFrom, string DateTo, int SegmentID)
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


                    if (SegmentID == 1)
                    {
                        //var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.SegmentID==1 &&x.IsActive==true && x.Status== "InProgress").Select(x => new
                        //{
                        //    ID=x.ID,
                        //    Name=dbContext.Retailers.Where(y=>y.ID== x.CustomerID).Select(y=>y.ShopName).FirstOrDefault(),
                        //    TotalLiters=x.TotalLiters,
                        //    pic1= Url+x.Picture,
                        //    Date = x.CreatedOn.HasValue ? x.CreatedOn.Value.ToString("yyyy-MM-dd") : null




                        //}).ToList();


                        var result = dbContext.Tbl_SalesClaimMaster
                          .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 1 && x.IsActive == true && x.Status == "InProgress")
                          .AsEnumerable() // Moves execution to memory
                          .Select(x => new
                          {
                              ID = x.ID,
                              Name = dbContext.Retailers
                                      .Where(y => y.ID == x.CustomerID)
                                      .Select(y => y.ShopName)
                                      .FirstOrDefault(),
                              TotalLiters = x.TotalLiters,
                              pic1 = Url + x.Picture,
                              SaleValue=x.SaleValue,
                              Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                      "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive==true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                          })
                           .OrderBy(x => x.Name)
                          .ToList();

                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
                    }

                    if (SegmentID == 2)
                    {
                     
                        //}).ToList();

                        var result = dbContext.Tbl_SalesClaimMaster
                            .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 2 && x.IsActive == true && x.Status == "InProgress")
                            .AsEnumerable() // Moves execution to memory
                            .Select(x => new
                            {
                                ID = x.ID,
                                Name = dbContext.Retailers
                                        .Where(y => y.ID == x.CustomerID)
                                        .Select(y => y.ShopName)
                                        .FirstOrDefault(),
                                TotalLiters = x.TotalLiters,
                                pic1 = Url + x.Picture,
                                SaleValue = x.SaleValue,
                                Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                      "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive == true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                            })
                             .OrderBy(x => x.Name)
                            .ToList();




                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
                    }

                    if (SegmentID == 3)
                    {
                        //var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.SegmentID == 3 && x.IsActive == true && x.Status == "InProgress").Select(x => new
                        //{
                        //    ID = x.ID,
                        //    Name = dbContext.Retailers.Where(y => y.ID == x.CustomerID).Select(y => y.ShopName).FirstOrDefault(),
                        //    TotalLiters = x.TotalLiters,
                        //    pic1 = Url + x.Picture,
                        //    Date = x.CreatedOn.HasValue ? x.CreatedOn.Value.ToString("yyyy-MM-dd") : null



                        //}).ToList();

                        var result = dbContext.Tbl_SalesClaimMaster
                          .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 3 && x.IsActive == true && x.Status == "InProgress")
                          .AsEnumerable() // Moves execution to memory
                          .Select(x => new
                          {
                              ID = x.ID,
                              Name = dbContext.Retailers
                                      .Where(y => y.ID == x.CustomerID)
                                      .Select(y => y.ShopName)
                                      .FirstOrDefault(),
                              TotalLiters = x.TotalLiters,
                              pic1 = Url + x.Picture,
                              SaleValue = x.SaleValue,
                              Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                      "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive == true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                          })
                           .OrderBy(x => x.Name)
                          .ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
                    }


                    if (SegmentID == 5)
                    {
                        //var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.SegmentID == 5 && x.IsActive == true && x.Status == "InProgress").Select(x => new
                        //{
                        //    ID = x.ID,
                        //    Name = dbContext.Retailers.Where(y => y.ID == x.CustomerID).Select(y => y.ShopName).FirstOrDefault(),
                        //    TotalLiters = x.TotalLiters,
                        //    pic1 = Url + x.Picture,
                        //    Date = x.CreatedOn.HasValue ? x.CreatedOn.Value.ToString("yyyy-MM-dd") : null



                        //}).ToList();

                        var result = dbContext.Tbl_SalesClaimMaster
                    .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 5 && x.IsActive == true && x.Status == "InProgress")
                    .AsEnumerable()
                    .Select(x => new
                    {
                        ID = x.ID,
                        Name = dbContext.Retailers
                                .Where(y => y.ID == x.CustomerID)
                                .Select(y => y.ShopName)
                                .FirstOrDefault(),
                        TotalLiters = x.TotalLiters,
                        pic1 = Url + x.Picture,
                        SaleValue = x.SaleValue,
                        Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive == true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                    })
                    .OrderBy(x => x.Name)
                    .ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

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
                ClaimSummery = paramm
            });

        }


    }
}