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
    public class ItemController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int MainCatID)
        {
            try
            {
                if (MainCatID > 0 )
                {
                    var Item = ManageArea.GetItemsForAPI(MainCatID);
                    if (Item != null && Item.Count > 0)
                    {
                        return Ok(new
                        {
                            Items = Item.Where(s => s.IsActive).Select(d => new
                            {
                                d.ItemId,
                                d.ItemName,
                                d.ItemPrice,
                                d.ItemPacking,
                                d.SortOrder
                            }).OrderBy(d => d.SortOrder)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ItemController Get API Failed");
            }
            object[] param = {};
            return Ok(new
            {
                Items = param
            });
        }
    }
}