using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class GetCompetitorNamesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var list = db.Tbl_CompititorList
                    .Where(x => x.IsActive == true)
                    .OrderBy(x => x.sortOrder)
                    .ThenBy(x => x.Name)
                    .Select(x => new { x.ID, x.Name })
                    .ToList();

                return Ok(new { Data = list, Message = "Success", ResultType = "Success" });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetCompetitorNames Failed");
                return Ok(new { Data = (object)null, Message = "Failed to load competitor names", ResultType = "Exception" });
            }
        }
    }
}
