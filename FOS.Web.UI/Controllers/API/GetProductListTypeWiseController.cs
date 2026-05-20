using CrystalDecisions.Shared.Json;
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
    public class GetProductListTypeWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(string ApplicationType)
        {
            try
            {
                if (ApplicationType != null)
                {
                  
                    var SubCat = db.Tbl_ProductDetail.Where(x=>x.ApplicationType== ApplicationType && x.IsActive==true).ToList();
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            CoverageProducts = SubCat.Select(d => new
                            {
                                d.ID,
                                d.Product_Desc,
                                d.Qtr_Price,
                                d.Qtr_UoM,
                                d.Gallon_Price,
                                d.Gallon_UoM,
                                d.Incentive_PKR,
                                d.Drum_Price,
                                d.Drum_UoM,
                                d.Sortorder,
                                d.Tier_ID,
                                d.Tier_Desc,
                                d.UOM,
                                d.Uom_ID,
                                d.Uom_Name,
                                d.TaxableCost,
                                d.Coverage,
                                d.ApplicationType
                            }).OrderBy(d => d.Product_Desc)
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
                CoverageProducts = new { }
            });
        }


    }
}