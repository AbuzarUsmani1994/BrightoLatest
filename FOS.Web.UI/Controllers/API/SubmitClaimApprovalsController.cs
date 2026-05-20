using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SubmitClaimApprovalsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Tbl_ClaimsApproval();
            //var RemObj = new TblReminder();
            try
            {
                
                   
                   // JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    JobObj.ClaimID = rm.ClaimID;
                    

                    JobObj.ApprovedAt = DateTime.UtcNow.AddHours(5);
                    JobObj.Remarks = rm.Remarks;
                    JobObj.ApprovedBy = rm.SOID;
                    JobObj.ApprovedStatus = rm.ApprovedStatus;
                JobObj.IsActive = true;
                JobObj.CreatedOn = DateTime.UtcNow.AddHours(5);
                  


                    //if (rm.Picture == "" || rm.Picture == null)
                    //{
                    //    JobObj.Picture = null;
                    //}
                    //else
                    //{
                    //    JobObj.Picture = ConvertIntoByte(rm.Picture, "TradeVisitPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
                    //}
                


                    db.Tbl_ClaimsApproval.Add(JobObj);
                    db.SaveChanges();
                   
                   

             
                    
              

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Claim Updated Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Claim Approved API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Claim Approved API Failed",
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
           
            public int ClaimID { get; set; }
            public int SOID { get; set; }

            public string ApprovedStatus { get; set; }



            public string Remarks { get; set; }
           
          
          

        }

       
    }
}