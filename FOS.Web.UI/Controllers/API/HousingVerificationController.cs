using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class HousingVerificationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Tbl_HousingSiteVerification();
            var RemObj = new Tbl_HousingVerificationOffTakeFrom();
            try
            {
               //Retailer ret = db.Retailers.Where(r => r.ID == rm.CustomerID).FirstOrDefault();
              


                    JobObj.CustomerID = rm.CustomerID;
                    JobObj.Remarks = rm.Remarks;
                    JobObj.SiteStatus = rm.SiteStatus;
                    JobObj.SiteStatusVerification = rm.SiteStatusVerification;
                    JobObj.PlotSize = rm.PlotSize;
                JobObj.PlotType = rm.PlotType;
                JobObj.SOMonthlyVisits = rm.SOMonthlyVisit;
                    //JobObj.OffTakeFromOthers = rm.OffTakeFromOthers;
                    JobObj.CompititorName = rm.CompititorName;
                    JobObj.SiteContactInfo = rm.SiteContactInformation;
                    JobObj.SiteAddressinfo = rm.SiteAddressInformation;
                    JobObj.EstimatedValue = rm.EstimatedValue;
                    JobObj.EstimatedVolume = rm.EstimatedVolume;
                    
                    JobObj.SOID = rm.SOID;
                    JobObj.IsActive = true;
                    JobObj.CreatedAt = DateTime.UtcNow.AddHours(5);
                if (rm.Picture == "" || rm.Picture == null)
                {
                    JobObj.Picture = null;
                }
                else
                {
                    JobObj.Picture = ConvertIntoByte(rm.Picture, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                }


                db.Tbl_HousingSiteVerification.Add(JobObj);
                    db.SaveChanges();

                    if (rm.HousingOffTakeFromList != null)
                    {



                        List<string> tokens = rm.HousingOffTakeFromList.Split(',').ToList();

                        foreach (var item in tokens)
                        {
                            db.Tbl_HousingVerificationOffTakeFrom.Add(
                                new Tbl_HousingVerificationOffTakeFrom
                                {
                                   
                                    HousingVerificationID = JobObj.ID,
                                    TradeCustomerID = Convert.ToInt32(item),
                                    CreatedAt = DateTime.UtcNow.AddHours(5)


                                });
                            
                        }

                    db.SaveChanges();

                }
               

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Visit Verification Submitted Successfully",
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
                    Message = "Housing Visit API Failed",
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
            public string SiteStatus { get; set; }
            public string SiteStatusVerification { get; set; }
            public string SOMonthlyVisit { get; set; }
            public string PlotSize { get; set; }
            public string PlotType { get; set; }
            public int SOID { get; set; }
            public int OrderVolume { get; set; }
            
            public string CompititorName { get; set; }
            
            public string OffTakeFromOthers { get; set; }
           
            public string SiteContactInformation { get; set; }
            public string SiteAddressInformation { get; set; }
            public long EstimatedVolume { get; set; }
            public decimal EstimatedValue { get; set; }
            public string Picture { get; set; }
            public string Remarks { get; set; }
            public string HousingOffTakeFromList { get; set; }

           // public List<JobItemModel> HousingVisitsTradeList { get; set; }

        }
        public  class HousingVerificationOffTakeFrom
        {
          
            public Nullable<int> HousingVerificationID { get; set; }
            public Nullable<int> TradeCustomerID { get; set; }
            public Nullable<System.DateTime> CreatedAt { get; set; }
        }

        public class JobItemModel
        {
            public int HousingVisitID { get; set; }
            public int CustomerID { get; set; }
           
        }
    }
}