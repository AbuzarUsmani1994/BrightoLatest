using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class ChemicalRetailerEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(EditChemicalRetailermodel rm)
        {
            Tbl_ChemCustomerInfo retailerObj = new Tbl_ChemCustomerInfo();
            try
            {
               
                    //ADD New Retailer 
                    retailerObj = db.Tbl_ChemCustomerInfo.Where(u => u.ID == rm.CustomerID).FirstOrDefault();


                retailerObj.BusinessName = rm.BusinessName;
                retailerObj.SalesPerson = db.SaleOfficers.Where(x => x.ID == rm.SalesOfficerID).Select(x => x.Name).FirstOrDefault();

                retailerObj.CityID = rm.CityID;
                retailerObj.Code = rm.CustomerCode;
                retailerObj.ContactName = rm.ContactName;
                retailerObj.Phone = rm.PersonContact;
                retailerObj.ZoneID = rm.RegionID;
                retailerObj.City = db.Cities.Where(x => x.ID == rm.CityID).Select(x => x.Name).FirstOrDefault();
                retailerObj.Region = db.Regions.Where(x => x.ID == rm.RegionID).Select(x => x.Name).FirstOrDefault();



                retailerObj.BusinessLine = rm.BusinessLine;
                retailerObj.IsActive = true;



                //db.Retailers.Add(retailerObj);
                //END

                // Add Token Detail ...
                db.SaveChanges();

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Chemical Customer Edit Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               
               

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Chemical Customer Edit API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Chemical Customer Edit API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }


       

        public class SuccessResponse
        {

        }
        public class EditChemicalRetailermodel
        {
            public int CustomerID { get; set; }
            public string CustomerCode { get; set; }
            public string BusinessName { get; set; }

            public int SalesOfficerID { get; set; }
            public int PersonContact { get; set; }
            public string ContactName { get; set; }
            public int RegionID { get; set; }

            public int CityID { get; set; }


            public string BusinessLine { get; set; }
        }

        }
    }
