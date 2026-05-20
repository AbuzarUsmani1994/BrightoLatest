using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class MonthlyTotalOrdersController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {


                var SoHeirarchy = dbContext.Sp_PresentSOHeirarchy1_1(SOID).ToList();

                int Saleofficerid = 0;
                foreach (var item in SoHeirarchy)
                {
                    Saleofficerid = item.SaleOfficerID;
                }
                // List<Tbl_Access> list = null;
                var list = dbContext.Tbl_Access.Where(x => x.RepotedUP == Saleofficerid).ToList();
                //DateTime now = DateTime.Now;
                //var startDate = new DateTime(now.Year, now.Month, 1);
                //var endDate = startDate.AddMonths(1).AddDays(-1);
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);
                var startDate = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
              
                DateTime dtFromToday = startDate.Date;
                DateTime dtToToday = endDate.AddDays(1);

                List<MonthData> obj = new List<MonthData>();
                

                foreach (var itm in list)
                {
                    int a = 0;
                    int b = 0;
                    List<JobsDetail> itmC = dbContext.JobsDetails.Where(x => x.JobDate >= dtFromToday && x.JobDate <= dtToToday && x.SalesOficerID == itm.SaleOfficerID ).ToList();
                    if (itmC.Count != 0)
                    {
                        foreach (var ele in itmC)
                            if(ele.JobType== "Retailer Order")
                            {
                                if (obj.Count >= 1)
                                {
                                    var result = obj.Find(x => x.SaleOfficerID == ele.SalesOficerID);
                                    if (result != null)
                                    {
                                        result.TotalRetailerOrder++;   

                                    }
                                    else
                                    {
                                        obj.Add(new MonthData()
                                        {
                                            //the name of the db column
                                            TotalRetailerOrder = 1,
                                            SaleOfficerID = itm.SaleOfficerID,

                                        });

                                    }
                                    //if ((obj.Contains((Saleofficerid.CompareTo)))==ele.SalesOficerID)
                                    //foreach (var dt in obj)
                                    //{
                                    //    if (dt.SaleOfficerID == ele.SalesOficerID)
                                    //    {
                                    //        dt.TotalRetailerOrder++;
                                    //    }
                                    //    else
                                    //    {
                                           

                                    //    }
                                    //}

                                }
                                else
                                {
                                    obj.Add(new MonthData()
                                    {
                                        //the name of the db column
                                        TotalRetailerOrder = 1,
                                        SaleOfficerID = itm.SaleOfficerID,

                                    });
                                }
                            }
                       else if (ele.JobType == "Distributor Order")
                        {
                            if (obj.Count >= 1)
                            {
                                var result = obj.Find(x => x.SaleOfficerID == ele.SalesOficerID);
                                if (result != null)
                                {
                                    result.TotalDistributorOrder++;

                                }
                                else
                                {
                                    obj.Add(new MonthData()
                                    {
                                        //the name of the db column
                                        TotalDistributorOrder = 1,
                                        SaleOfficerID = itm.SaleOfficerID,

                                    });

                                }
                                //if ((obj.Contains((Saleofficerid.CompareTo)))==ele.SalesOficerID)
                                //foreach (var dt in obj)
                                //{
                                //    if (dt.SaleOfficerID == ele.SalesOficerID)
                                //    {
                                //        dt.TotalRetailerOrder++;
                                //    }
                                //    else
                                //    {


                                //    }
                                //}

                            }
                            else
                            {
                                obj.Add(new MonthData()
                                {
                                    //the name of the db column
                                    TotalDistributorOrder = 1,
                                    SaleOfficerID = itm.SaleOfficerID,

                                });
                            }
                        }

                    }
                }
                var data = obj.ToList();

                if (data.Count != 0)
                {
                    return Ok(new
                    {
                        AbsentSO = data
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
public class MonthData
{
    public int SaleOfficerID { get; set; }
    public int TotalRetailerOrder { get; set; }
    public int TotalDistributorOrder { get; set; }
}


 