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
    public class AllDataDistributorController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int RHeadID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {


                var SoHeirarchy = dbContext.RegionalHeadRegions.Where(x => x.RegionHeadID == RHeadID).ToList();
                List<AllDistributorData> obj = new List<AllDistributorData>();


                foreach (var itm in SoHeirarchy)
                {

                    List<Dealer> itmC = dbContext.Dealers.Where(x => x.RegionID == itm.RegionID).ToList();
                    if (itmC.Count != 0)
                    {
                        foreach (var ele in itmC)

                            if (obj.Count >= 1)
                            {
                                var result = obj.Find(x => x.RegionName == ele.Region.Name);
                                if (result != null)
                                {
                                    result.TotalDistributors++;

                                }
                                else
                                {
                                    obj.Add(new AllDistributorData()
                                    {
                                        //the name of the db column
                                        TotalDistributors = 1,
                                        RegionName = itm.Region.Name,

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
                                obj.Add(new AllDistributorData()
                                {
                                    //the name of the db column
                                    TotalDistributors = 1,
                                    RegionName = itm.Region.Name,

                                });
                            }
                    }

                }

                var data = obj.ToList();

                if (data.Count != 0)
                {
                    return Ok(new
                    {
                        AllDistributorSO = data
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
                AllDistributorSOSO = paramm
            });

        }
    }
}
public class AllDistributorData
{
    public string RegionName { get; set; }
    public int TotalDistributors { get; set; }

}