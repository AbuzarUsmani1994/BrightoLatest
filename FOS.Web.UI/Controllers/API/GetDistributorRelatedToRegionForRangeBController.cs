using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetDistributorRelatedToRegionForRangeBController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int RegionID, int CityID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<MyList> list = new List<MyList>();
                MyList comlist;
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (RegionID > 0 && CityID>0)
                {
                    object[] param = { RegionID };


                    var result = db.DealerRanges.Where(x => x.RegionID == RegionID  && x.RangeID == 7).Select(x => x.DealerID).ToList();

                    foreach (var item in result)
                    {
                        comlist = new MyList();

                        comlist.ID = item;
                        comlist.ShopName = dbContext.Dealers.Where(x => x.ID == item).Select(x => x.ShopName).FirstOrDefault();
                        list.Add(comlist);
                    }




                    //var result = dbContext.Sp_GetDistributorRelatedtoRegion1_1(RegionID).ToList();


                    if (list != null && list.Count > 0)
                    {
                        return Ok(new
                        {
                            Distributor = list

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                Distributor = paramm
            });

        }


    }

   
}