using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class AllPurposeVisitOldController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Tbl_AllPurposeVisits();
            //var RemObj = new TblReminder();
            try
            {
                Retailer ret = db.Retailers.Where(r => r.ID == rm.CustomerID).FirstOrDefault();
                if (ret != null)
                {
                    //var regionalHeadID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerId).Select(x => x.RegionalHeadID).FirstOrDefault();
                  


                   // JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    JobObj.CustomerID = rm.CustomerID;
                   

                    JobObj.OtherText = rm.OtherRemarks;
                    JobObj.Remarks = rm.Remarks;
                    JobObj.SOID = rm.SOID;
                    JobObj.Latitude = rm.Latitude;
                    JobObj.Longitude = rm.Longitude;
                    JobObj.PurposeOfVisitID = rm.PurposeOfVisitID;
                    JobObj.NextVisitDate = rm.NextVisitDate;
                    JobObj.IsActive = true;
                    JobObj.CreatedOn = DateTime.UtcNow.AddHours(5);
                  


                


                    db.Tbl_AllPurposeVisits.Add(JobObj);
                    db.SaveChanges();
                   
                   

             
                    
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "All Purpose Visit Done Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Order API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "All Purpose Visit API Failed",
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
        public class DailyActivityRequest
        {
           
            public int CustomerID { get; set; }
            public int SOID { get; set; }
            public int PurposeOfVisitID { get; set; }
            public string OtherRemarks { get; set; }
         
            public DateTime NextVisitDate { get; set; }

            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        

            public string Remarks { get; set; }
        
          
          

        }

       
    }
}