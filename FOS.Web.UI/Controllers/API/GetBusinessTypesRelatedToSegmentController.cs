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
    public class GetBusinessTypesRelatedToSegmentController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SegmentID)
        {
            try
            {
                if (SegmentID > 0)
                {
                    var SubCat = ManageArea.GetBusinessTypesForAPI(SegmentID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            BusinessTypes = SubCat.Where(s => s.IsActive).Select(d => new
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
                Log.Instance.Error(ex, "CitiesController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                BusinessTypes = param
            });
        }


    }
}