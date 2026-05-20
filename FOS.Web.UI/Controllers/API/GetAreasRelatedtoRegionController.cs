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
    public class GetAreasRelatedtoRegionController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int RegionID, int CityID)
        {
            try
            {
                if (RegionID > 0 && CityID > 0)
                {
                    var Item = ManageArea.GetAreasForAPI(RegionID, CityID);
                    if (Item != null && Item.Count > 0)
                    {
                        return Ok(new
                        {
                            Areas = Item.Where(s => s.IsActive).Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "AreaController Get API Failed");
            }

            object[] param = {};
            return Ok(new
            {
                Areas = param
            });
        }
    }
}