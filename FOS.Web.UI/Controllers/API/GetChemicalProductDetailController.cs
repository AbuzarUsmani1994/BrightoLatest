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
    public class GetChemicalProductDetailSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(string ProductDesc)
        {
            try
            {
                if (ProductDesc!=null)
                {
                  
                    var SubCat = db.Tbl_BrightoChemicalProducts.Where(x=>x.ProductDesc==ProductDesc && x.IsActive==true).ToList();
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            BrightoChemicalProducts = SubCat.Select(d => new
                            {
                                d.ID,
                                d.BrandID,
                                d.BrandDesc,
                                d.ProductID,
                                d.ProductCode,
                                d.ProductDesc,
                                d.TierID,
                                d.TierDesc,
                                d.SubProductDesc,
                                d.SubProductID,
                                d.PackingID,
                                d.PackingDesc,
                                d.UOM,
                                d.UomID,
                                d.UomName,
                                d.TaxableCost
                            }).OrderBy(d => d.SubProductDesc)
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
                BrightoChemicalProducts = new { }
            });
        }


    }
}