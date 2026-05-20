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
    public class GetChemicalCustomersForEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int CustomerID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                if (CustomerID > 0)
                {
                    object[] param = { CustomerID };


                    var result = dbContext.Sp_GetChemicalRetailerForEditForBrighto(CustomerID).ToList();

                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                           ChemicalCustomerEdit=result

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
                ChemicalCustomerEdit = paramm
            });

        }
    }
}