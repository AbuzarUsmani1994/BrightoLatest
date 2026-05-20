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
    public class DeleteHousingImagesController : ApiController
    {
      

     
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DeleteHousingImages request)
        { 
            try
            {
                var retailers = db.Tbl_HousingVisits.Where(a => a.ID == request.VisitID).FirstOrDefault();

                if ((request.IsPicture1Delete==true))
                {
                    retailers.Picture1 = null;
                    
                }
                if ((request.IsPicture2Delete == true))
                {
                    retailers.Picture2 = null;

                }
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Picture Deleted Successfully",
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
    public class DeleteHousingImages
    {
        public int VisitID { get; set; }
        public bool IsPicture1Delete {  get; set; }
        public bool IsPicture2Delete { get; set; }
    }
}