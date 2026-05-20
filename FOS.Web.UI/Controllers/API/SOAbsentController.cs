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
    public class SOAbsentController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get()
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {

                // object[] param = { SOID };

                var result = dbContext.Sp_PresentSOPieGraphFinal().ToList();

                var soID = result.Select(s => s.SaleOfficerID);


                var FULLSO = dbContext.SaleOfficers.Where(t => t.IsActive).ToList();

                var final = FULLSO.Where(p => !result.Any(p2 => p2.SaleOfficerID == p.ID)).Select(i => new
                {

                    i.ID,
                    i.Name
                  


                });




                if (final != null)
                {
                    return Ok(new
                    {
                        AbsentSO = final

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
                AbsentSO = paramm
            });

        }


    }
}