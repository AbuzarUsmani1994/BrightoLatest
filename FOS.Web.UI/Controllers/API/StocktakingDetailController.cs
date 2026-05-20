using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class StocktakingDetailController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int JobID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (JobID > 0)
                {
                    object[] param = { JobID };
                    
                    
                        var result = dbContext.Sp_StockTakingDetail1_1(JobID).ToList();
                    
                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            StockTakingDetail = result
                            
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                StockTakingDetail = paramm
            });

        }


    }
}