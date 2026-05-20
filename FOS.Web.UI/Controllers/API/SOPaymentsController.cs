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
    public class SOPaymentsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            TBL_Payments JobObj = new TBL_Payments();
          
            try
            {

                JobObj.SOID = rm.SOID;
                JobObj.ZoneID = rm.RegionID;
                JobObj.CityID = rm.CityID;
                JobObj.Lattitude = rm.Latitude;
                JobObj.Longitude = rm.Longitude;

                JobObj.PaymentMode = rm.PaymentType;
                JobObj.Amount = rm.Amount;
                JobObj.Remarks = rm.Remarks;
                JobObj.SegmentTypeid = rm.SegmentTypeID;

                if (rm.Picture == "" || rm.Picture == null)
                {
                    JobObj.Picture = null;
                }
                else
                {
                    JobObj.Picture = ConvertIntoByte(rm.Picture, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "Payments");
                }

                JobObj.CustomerID = rm.RetailerID;
                
                JobObj.CreatedAt = DateTime.UtcNow.AddHours(5);
                db.TBL_Payments.Add(JobObj);
                db.SaveChanges();
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Payments Saved Successfully",
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
                    Message = "Payments Saved  Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

    


        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
       
            public int SOID { get; set; }
           
            public int RetailerID { get; set; }
            public int RegionID { get; set; }
            public int SegmentTypeID { get; set; }
            public int CityID { get; set; }

            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public decimal Amount { get; set; }
            public string PaymentType { get; set; }

            public string Remarks { get; set; }

            public string Picture { get; set; }

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

    }
}