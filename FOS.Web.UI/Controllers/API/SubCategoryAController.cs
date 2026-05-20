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
    public class SubCategoryAController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int MainCatID, int SubCatID)
        {
            try
            {
                if (MainCatID > 0 && SubCatID > 0)
                {
                    var Item = ManageArea.GetSubCatAForAPI(MainCatID, SubCatID);
                    if (Item != null && Item.Count > 0)
                    {
                        return Ok(new
                        {
                            SubCategoryA = Item.Where(s => s.IsActive).Select(d => new
                            {
                                d.SubCategoryAID,
                                d.SubCategoryAName
                             
                            }).OrderBy(d => d.SubCategoryAName)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ItemController Get API Failed");
            }

            return Ok(new
            {
                SubCategoryA = new { }
            });
        }
    }
}