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
    public class RetailerTransferController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(RetailerTransferRequest rm)
        {
            Tbl_SchoolException Excep = new Tbl_SchoolException();
            Retailer retailerObj = new Retailer();
            try
            {

                List<int> TagIds = rm.RetailerID.Split(',').Select(int.Parse).ToList();

                foreach (var item in TagIds)
                {
                    //ADD New Retailer 
                    retailerObj = db.Retailers.Where(u => u.ID == item).FirstOrDefault();
                    retailerObj.RegionID = rm.RegionID;
                    retailerObj.CityID = rm.TransferTOCityID;
                    retailerObj.AreaID = rm.TransferTOCityID;
                    db.SaveChanges();
                }

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Retailer Transfer Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               


            }
            catch (Exception ex)
            {
                


                Log.Instance.Error(ex, "Add Retailer API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Retailer Transfer API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex, 
                    ValidationErrors = null
              
            };

              
              


            }

         

        }



        public class SuccessResponse
        {

        }
        public class RetailerTransferRequest
        {
            public RetailerTransferRequest()
            {
               
            }
            public string RetailerID { get; set; }
            public int RegionID { get; set; }
            public int TransferTOCityID { get; set; }

        }

       


    }
}