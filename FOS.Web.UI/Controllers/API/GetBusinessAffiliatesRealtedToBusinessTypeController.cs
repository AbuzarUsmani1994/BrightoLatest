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
    public class GetBusinessAffiliatesRealtedToBusinessTypeController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int AffBusinessTypeID,int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<RetailerType> CustomerValidate = new List<RetailerType>();
                RetailerType cty;
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (AffBusinessTypeID > 0)
                {
                    object[] param = { AffBusinessTypeID };

                  

                    var result = dbContext.Tbl_BusinessAffiliates.Where(x=>x.AffBusinessTypeID== AffBusinessTypeID && x.SOID==SOID && x.IsActive==true).OrderBy(x => x.BusinessName).ToList();

                    foreach (var dbCty in result)
                    {
                        cty = new RetailerType();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.BusinessName;
                       

                        CustomerValidate.Add(cty);
                    }

                    if (CustomerValidate != null && CustomerValidate.Count > 0)
                    {
                        return Ok(new
                        {
                            BusinessAffiliatesList = CustomerValidate

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                BusinessAffiliatesList = paramm
            });

        }


    }
}