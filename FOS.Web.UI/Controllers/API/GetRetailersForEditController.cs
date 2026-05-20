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
    public class GetRetailersForEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int DistributorID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (DistributorID > 0)
                {
                    object[] param = { DistributorID };


                    var result = dbContext.Sp_GetRetailerForEditFinalRequest(DistributorID).ToList();

                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                           RetailerEdit=result

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DistributorEdit GET API Failed");
            }
            object[] paramm = {  };
            return Ok(new
            {
                RetailerEdit = paramm
            });

        }
    }
}