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
    public class GetBusinessExpertiseRealtedToBusinessTypeController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int AffBusinessTypeID)
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

                  

                    var result = dbContext.Tbl_AffiliatesExpertise.Where(x=>x.AffBusinessTypeID== AffBusinessTypeID && x.IsActive==true).OrderBy(x => x.Name).ToList();

                    foreach (var dbCty in result)
                    {
                        cty = new RetailerType();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.Name;
                       

                        CustomerValidate.Add(cty);
                    }

                    if (CustomerValidate != null && CustomerValidate.Count > 0)
                    {
                        return Ok(new
                        {
                            BusinessAffiliatesExpertiseList = CustomerValidate

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
                BusinessAffiliatesExpertiseList = paramm
            });

        }


    }
}