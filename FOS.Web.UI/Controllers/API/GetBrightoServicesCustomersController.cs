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
    public class GetBrightoServicesCustomersController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int RegionID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<RetailerType> CustomerValidate = new List<RetailerType>();
                RetailerType cty;
                //List<RetailerData> MAinCat = new List<RetailerData>();
               

                  

                    var result = dbContext.Tbl_FannanCustomer.Where(x=> x.IsActive==true && x.RegionID==RegionID ).OrderBy(x => x.SiteName).ToList();

                    foreach (var dbCty in result)
                    {
                        cty = new RetailerType();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.SiteName + ' '+ dbCty.SiteCode;
                        

                        CustomerValidate.Add(cty);
                    }

                    if (CustomerValidate != null && CustomerValidate.Count > 0)
                    {
                        return Ok(new
                        {
                            BrightoServicesCustomerList = CustomerValidate

                        });
                    }

              

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                BrightoServicesCustomerList = paramm
            });

        }


    }
}