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
    public class GetDistributorsRelatedToSOSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            try
            {
                if (SOID > 0)
                {
                  
                    var SubCat = new CommonController().DistributorRrelatedToSoForCheckin(SOID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            Customers = SubCat.Where(s => s.ISActive).Select(d => new
                            {
                                d.ID,
                                d.ShopName
                            }).OrderBy(d => d.ShopName)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SubCategoryController GET API Failed");
            }

            return Ok(new
            {
                Customers = new { }
            });
        }


    }
}