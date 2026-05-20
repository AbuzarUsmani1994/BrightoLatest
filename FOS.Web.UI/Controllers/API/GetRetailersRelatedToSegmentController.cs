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
    public class GetRetailersRelatedToSegmentController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SegmentID,int RegionID,int CityID,int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
                CustomersForCheckin cty;
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (SegmentID > 0)
                {
                    object[] param = { SegmentID };

                  

                    var result = dbContext.Retailers.Where(x=>x.SegmentTypeID==SegmentID && x.RegionID== RegionID && x.CityID==CityID && x.SaleOfficerID==SOID && x.IsActive==true).OrderBy(x => x.ShopName).ToList();

                    foreach (var dbCty in result)
                    {
                        cty = new CustomersForCheckin();
                        cty.ID = dbCty.ID;
                        cty.ShopName = dbCty.ShopName;
                        cty.OwnerName = dbCty.Name;
                        cty.OwnerNumber = dbCty.Phone1;
                        cty.Address = dbCty.Address;
                        cty.CustomerType = dbCty.CustomerType;
                     

                        CustomerValidate.Add(cty);
                    }

                    if (CustomerValidate != null && CustomerValidate.Count > 0)
                    {
                        return Ok(new
                        {
                            Retailers = CustomerValidate

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
                Retailers = paramm
            });

        }


    }
}