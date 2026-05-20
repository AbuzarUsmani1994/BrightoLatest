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
    public class AllDataRetailerController : ApiController
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


                List<AllRetailerData> obj = new List<AllRetailerData>();


                foreach (var itm in list)
                {

                    List<Retailer> itmC = dbContext.Retailers.Where(x => x.SaleOfficerID == itm.SaleOfficerID).ToList();
                    if (itmC.Count != 0)
                    {
                        foreach (var ele in itmC)

                            if (obj.Count >= 1)
                            {
                                foreach (var result in obj)
                                {
                                    result.TotalRetailers++;
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
                                obj.Add(new AllRetailerData()
                                {
                                    //the name of the db column
                                    TotalRetailers = 1,


                                });
                            }
                    }

                }

                var data = obj.ToList();

                if (data.Count != 0)
                {
                    return Ok(new
                    {
                        AllRetailerSO = data
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
                AllRetailerSOSO = paramm
            });

        }
    }
}
public class AllRetailerData
{

    public int TotalRetailers { get; set; }

}