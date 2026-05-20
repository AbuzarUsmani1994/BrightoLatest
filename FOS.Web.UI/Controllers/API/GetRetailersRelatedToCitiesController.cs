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
    public class GetRetailersRelatedToCitiesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int RegionID, int CityID, int SelectedSOID)
        {
            try
            {
                if (RegionID > 0)
                {
                    var SubCat = ManageArea.GetRetailersForAPI(RegionID,CityID,6, SelectedSOID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            Retailers = SubCat.Where(s => s.IsActive).Select(d => new
                            {
                               d.ID,
                                d.ShopName,
                                d.NewOrOld,
                                d.Quota,
                                d.address,
                                d.ShopType,
                                d.OwnerName,
                                d.Phone,
                                d.Latitude,
                                d.Longitude,
                                d.AreaName
                            }).OrderBy(d => d.ShopName)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "CitiesController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                Retailers = param
            });
        }


    }
}