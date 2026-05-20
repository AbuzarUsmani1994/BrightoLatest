using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class GetQualityReviewAgainstComplaintController : ApiController
    {
        FOSDataModel db = new FOSDataModel();
        public IHttpActionResult Get(int ComplaintID)
        {
            try
            {
                var result = db.Sp_GetComplaintVerifiedorNorbyQualtiy(ComplaintID).ToList();
                if (result.Count > 0)
                {
                    return Ok(new
                    {
                        Review = result
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Review = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Review = ex.Message.ToString()
                });
                throw;
            }
        }
    }
}
