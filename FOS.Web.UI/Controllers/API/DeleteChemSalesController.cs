using FOS.DataLayer;
using FOS.Setup;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class DeleteChemSalesController : ApiController
    {
      

        // GET api/<controller>/5
        FOSDataModel db = new FOSDataModel();

        

        public Result<SuccessResponse> Post(DeleteChemSaleRequest request)
        { 
            try
            {
                var retailers = db.Tbl_ChemSaleMaster.Where(a => a.ID == request.SaleID).FirstOrDefault();
                retailers.IsActive = false;
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Deletion completed successfully.",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Order API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Delete Request Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }


    }

    public class DeleteChemSaleRequest
    {
        public int SaleID { get; set; }
    }
}