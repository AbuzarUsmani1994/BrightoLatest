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
    public class GetRetailerRelatedtoSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (ID > 0)
                {
                    object[] param = { ID };


                    var result = dbContext.Sp_GetRetailerForEditFinalRequest(ID).ToList();

                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            RetailersForEdit = result

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "RetailersForEdit GET API Failed");
            }
            object[] paramm = {  };
            return Ok(new
            {
                RetailersForEdit = paramm
            });

        }
    }
}