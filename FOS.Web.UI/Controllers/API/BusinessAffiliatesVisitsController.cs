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
    public class BusinessAffiliatesVisitsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(BusinessAffiliatesVisitsRequest rm)
        {
           
            Tbl_BusinessAffiliatesVisits retailerObj = new Tbl_BusinessAffiliatesVisits();
            try
            {
                   
                    retailerObj.SOID = rm.SOID;
                   
                    retailerObj.CityID = rm.CityID;
                    retailerObj.RegionID = rm.RegionID;
                retailerObj.BusinessAffiliateID = rm.BusinessAffiliateID;
             
                    retailerObj.VisitDate = rm.VisitDate;
                retailerObj.PurposeOfVisit = rm.PurposeOfVisit;
                retailerObj.TargetAgreement = rm.TargetAgreement;


                if (rm.GroupPicture == "" || rm.GroupPicture == null)
                {
                    retailerObj.GroupPicture = null;
                }
                else
                {
                    retailerObj.GroupPicture = ConvertIntoByte(rm.GroupPicture, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                }

                if (rm.Picture == "" || rm.Picture == null)
                {
                    retailerObj.Picture = null;
                }
                else
                {
                    retailerObj.Picture = ConvertIntoByte1(rm.Picture, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                }


                retailerObj.Remarks = rm.Remarks;
                    retailerObj.IsActive = true;
              
                    retailerObj.CreatedDate = DateTime.UtcNow.AddHours(5);
                   

                    db.Tbl_BusinessAffiliatesVisits.Add(retailerObj);
                  

             

                    db.SaveChanges();

                 



                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Business Affiliates Visit Submitted Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               


            }
            catch (Exception ex)
            {
               

                Log.Instance.Error(ex, "Business Affiliates Visit API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Business Affiliates Visit API Failed",
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

        public string ConvertIntoByte1(string Base64, string DealerName, string SendDateTime, string folderName)
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
        public class BusinessAffiliatesVisitsRequest
        {
            public BusinessAffiliatesVisitsRequest()
            {
              
            }
            public int SOID { get; set; }
           
            public int BusinessAffiliateID { get; set; }
           
         
            public int RegionID { get; set; }
        
            public int CityID { get; set; }
            public DateTime VisitDate { get; set; }

            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        
            public string PurposeOfVisit { get; set; }

            public string TargetAgreement { get; set; }
      
            public string Picture { get; set; }
            public string GroupPicture { get; set; }

            public string Remarks { get; set; }
         

      



        }

     

    }
}