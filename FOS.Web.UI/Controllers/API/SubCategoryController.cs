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
    public class SubCategoryController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int MainCatID)
        {
            try
            {
                if (MainCatID > 0)
                {
                    var SubCat = ManageArea.GetSubCatForAPI(MainCatID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            SubCategory = SubCat.Where(s => s.IsActive).Select(d => new
                            {
                                d.ID,
                                d.SubName
                            }).OrderBy(d => d.SubName)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SubCategoryController GET API Failed");
            }

            return Ok(new
            {
                SubCategory = new { }
            });
        }


    }
}