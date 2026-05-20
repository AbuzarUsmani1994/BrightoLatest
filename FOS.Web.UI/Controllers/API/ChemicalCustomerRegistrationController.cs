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
    public class ChemicalCustomerRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ChemicalCustomerRegistrationRequest rm)
        {
            Tbl_SchoolException Excep = new Tbl_SchoolException();
            Tbl_ChemCustomerInfo retailerObj = new Tbl_ChemCustomerInfo();
            try
            {
                
                //var rangeid= db.SaleOfficers.Where(x=>x.ID==rm.SalesOfficerID).Select(x=>x.RangeID).FirstOrDefault();

               

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
              
               
                   

                    db.Tbl_ChemCustomerInfo.Add(retailerObj);
                  

             

                    db.SaveChanges();

                 



                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Chemical Customer Registration Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               


            }
            catch (Exception ex)
            {
               

                Log.Instance.Error(ex, "Add Business Affiliates API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Chemical Customer  Registration API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex, 
                    ValidationErrors = null
              
            };

              
              


            }

         

        }

        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }



        public class SuccessResponse
        {

        }
        public class ChemicalCustomerRegistrationRequest
        {
            public ChemicalCustomerRegistrationRequest()
            {
              
            }
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