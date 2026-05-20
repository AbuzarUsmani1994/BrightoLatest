using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FOS.DataLayer;
using FOS.AdminPanel;
using System.IO;
using FOS.Web.UI.Models;
using FOS.Shared;
using FluentValidation.Results;
using System.Web.Security;
using FOS.Web.UI.Common;
using FOS.Web.UI.Common.CustomAttributes;
using Shared.Diagnostics.Logging;
using FOS.Setup;

namespace FOS.Web.UI.Controllers
{
    public class SalesController : Controller
    {

        FOSDataModel db = new FOSDataModel();


        //public ActionResult ManualSales()
        //{
        //    List<DealerData> DealersObj = FOS.Setup.ManageDealer.GetAllDealersList();
        //    int DID = DealersObj.Select(d => d.ID).FirstOrDefault();

        //    List<RetailerData> RetailersObj = FOS.Setup.ManageRetailer.GetDealerRetailers(DID);

        //    var ObjJob = new FOS.Shared.JobsDetailData();
        //    ObjJob.Dealers = DealersObj;
        //    ObjJob.Retailers = RetailersObj;

        //    return View(ObjJob);
        //}

        //public JsonResult GetDealerRetailers(int DealerID)
        //{
        //    var Result = FOS.Setup.ManageRetailer.GetDealerRetailers(DealerID);
        //    return Json(Result);
        //}


        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddShopVisit(JobsDetailData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();


            try
            {
                if (newData != null)
                {

                    if (boolFlag)
                    {

                        Res = ManageSales.AddShopVisit(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }

    }

}
