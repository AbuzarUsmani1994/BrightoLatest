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
    public class MyOrderListSummeryController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int RegionID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (RegionID > 0)
                {
                    object[] param = { RegionID };
                    
                    
                        var result = dbContext.Sp_MyOrderListSummery1_3(RegionID).ToList();
                    
                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            MyOrderListSummery = result
                            
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
                MyOrderListSummery = paramm
            });

        }


    }
}