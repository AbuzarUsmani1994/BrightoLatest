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
    public class GetOrdersEditController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int JobID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (JobID > 0)
                {
                    object[] param = { JobID };

                    // RetailerData cty;

                    var result = dbContext.Sp_GetOrdersEdit(JobID).ToList();


                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            OrderDetail = result

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "StockDetail GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                OrderDetail = paramm
            });

        }


    }
}