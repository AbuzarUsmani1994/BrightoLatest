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
    public class SOPresentController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get()
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {

              


                var result = dbContext.Sp_PresentSOPieGraphFinal().ToList();

                if (result != null && result.Count > 0)
                {
                    return Ok(new
                    {
                        PresentSO = result

                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                PresentSO = paramm
            });

        }


    }
}