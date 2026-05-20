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
    public class RSMSummaryController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            Tbl_RSMSummary jobDet = new Tbl_RSMSummary();
          
            var RemObj = new TblReminder();
            

           // var Lastdata = db.JobsDetails.Where(x => x.RetailerID == rm.RetailerId).OrderByDescending(x => x.ID).FirstOrDefault();
            try
            {
              
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                   
                   


                    //ADD New Job in jobsdetail 
                    jobDet.CityID = rm.CityID;
                    jobDet.DistributorID = rm.DistributorID;
                    jobDet.SOID = rm.SOID;
                    jobDet.VisitObjectives = rm.VisitObjectives;
                jobDet.DistributorPerformance = rm.DistributorPerformance;
                jobDet.SOPerformance = rm.SOPerformance;
                jobDet.MarketGap = rm.MarketGap;
                jobDet.Comments = rm.Comments;



               
                  
                    db.Tbl_RSMSummary.Add(jobDet);
                    db.SaveChanges();


                // Here StockItems is the array which is changed From JobItems Array due to Hassan 

                string[] values = rm.RSMCheckListIDS.Split(',');
                foreach (var item in values)
                {
                    var JobObj = new Tbl_RSMCheckList();
                    JobObj.CheckListID = Convert.ToInt32(item);
                    JobObj.SummaryID = jobDet.ID;
                    JobObj.CreatedOn = localTime;
                    db.Tbl_RSMCheckList.Add(JobObj);
                    db.SaveChanges();

                }






                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "RSM Summary Added Successfully",
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
                    Message = "Order API Failed",
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
            public DailyActivityRequest()
            {
                StockItems = new List<JobItemModel>();
                CurrentStock = new List<CurrentStockModel>();
                Brandrate = new List<BrandRateModel>();
                CompititorBrand = new List<CompititorBrandModel>();
            }
            public int JobId { get; set; }
            public int RetailerId { get; set; }
            public int SaleOfficerId { get; set; }
            public int Followupreason { get; set; }
            public string Type { get; set; }
            public int ReasonForNoSaleID { get; set; }
            public int VisitTypeID { get; set; }
            public string Remarks { get; set; }
            public int DiscountPercentage { get; set; }
            public int OrderTotal { get; set; }
            public int? CityID { get; set; }
            public int? DistributorID { get; set; }
            public int? SOID { get; set; }
            public string VisitObjectives { get; set; }
            public string DistributorPerformance { get; set; }
            public string SOPerformance { get; set; }
            public string MarketGap { get; set; }
            public string Comments { get; set; }

            public string RSMCheckListIDS { get; set; }
            public int DiscountedTotal { get; set; }
            public string PurposeofVisit { get; set; }
            public string ActivityDetails { get; set; }
            public string Picture1 { get; set; }
            public string ReminderCancelStatus { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
            public string NextVisitDate { get; set; }
            public string Priorty { get; set; }
            public string TentativeCloseDate { get; set; }
            public List<JobItemModel> StockItems { get; set; }
            public List<CurrentStockModel> CurrentStock { get; set; }
            public List<BrandRateModel> Brandrate { get; set; }
            public List<CompititorBrandModel> CompititorBrand { get; set; }
        }

        public class JobItemModel
        {
            public int JobID { get; set; }
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public int? Carton { get; set; }
            public int? Danda { get; set; }
        }
        public class CurrentStockModel
        {
            public decimal Price { get; set; }
            public int ItemID { get; set; }
            public int? Carton { get; set; }
            public int? Danda { get; set; }
        }
        public class BrandRateModel
        {

            public int ItemID { get; set; }
            public int? PRCarton { get; set; }
            public int? PRDanda { get; set; }
            public int? PRGhatta { get; set; }
            public int? SRCarton { get; set; }
            public int? SRDanda { get; set; }
            public int? SRGhatta { get; set; }

            public string ProductAvailability { get; set; }
        }

        public class CompititorBrandModel
        {
            public string BrandName { get; set; }
            public int ItemID { get; set; }
            public int? PRCarton { get; set; }
            public int? PRDanda { get; set; }
            public int? PRGhatta { get; set; }
            public int? SRCarton { get; set; }
            public int? SRDanda { get; set; }
            public int? SRGhatta { get; set; }
            public string ProductAvailability { get; set; }
        }
    }
}