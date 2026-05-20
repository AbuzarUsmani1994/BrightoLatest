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
    public class CoorporateVisitOldController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(CoorporateVisitRequest rm)
        {
            Tbl_SchoolException Excep = new Tbl_SchoolException();
            Tbl_CorporateVisits retailerObj = new Tbl_CorporateVisits();
            try
            {
                
                //var rangeid= db.SaleOfficers.Where(x=>x.ID==rm.SalesOfficerID).Select(x=>x.RangeID).FirstOrDefault();

                //ADD New Retailer 
               // retailerObj.ID = db.Retailers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    retailerObj.CustomerID = rm.CustomerID;
                    retailerObj.ProjectTitle = rm.ProjectTitle;

               

                    retailerObj.IndustryID = rm.IndustryID;
                   
              
                    retailerObj.OEMName = rm.OEMName;
                    retailerObj.ProjectEstimatedValue = rm.ProjectEstimatedValue;
                    retailerObj.InfluencerName = rm.InfluencerName;
                    // Zone ID  is saving in Regions Table bcx in menu region changes to zone
                    retailerObj.InfluencerNumber =rm.InfluencerNumber;
                    retailerObj.ArchitectName = rm.ArchitectName;
                retailerObj.ArchitectNumber = rm.ArchitectNumber;
                    retailerObj.BuilderName = rm.BuilderName;
                    retailerObj.BuilderNumber = rm.BuilderNumber;
                    retailerObj.PaintContractorName = rm.PaintContractorName;
                    retailerObj.PaintContractorNumber = rm.PaintContractorNumber;
                retailerObj.SOID = rm.SOID;
                retailerObj.IsActive = true;
                retailerObj.NextVisitDate = DateTime.UtcNow.AddHours(5);
                retailerObj.ProcurementManager = rm.ProcurementManager;
                    retailerObj.ProcurementManagerNo = rm.ProcurementManagerNo;
                    retailerObj.AccountsManager = rm.AccountsManager;
                    retailerObj.AccountsManagerNo = rm.AccountsManagerNo;
                retailerObj.Latitude = rm.Latitude;
                retailerObj.OrderVolume = rm.OrderVolume;
                retailerObj.Longitude = rm.Longitude;
                retailerObj.CompetitorName = rm.CompititorName;
                retailerObj.CompetitorNo = rm.CompititorNo;
               
                retailerObj.CreatedAt = DateTime.UtcNow.AddHours(5);
                
                    retailerObj.Remarks = rm.Remarks;
                 

                    db.Tbl_CorporateVisits.Add(retailerObj);
                   

                    db.SaveChanges();

                 



                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Visit Submitted Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               


            }
            catch (Exception ex)
            {
                Tbl_SchoolException tokenDetail = new Tbl_SchoolException();
                //tokenDetail.SaleOfficerID = rm.SalesOfficerID;
                tokenDetail.ExceptionMessage = ex.Message;
                if (ex.InnerException != null)

                    tokenDetail.InnerException = ex.InnerException.ToString();

                else

                    tokenDetail.InnerException = ("Inner Exception is Empty:");

                //tokenDetail.Createdby = rm.SalesOfficerID;
                tokenDetail.CreatedOn = DateTime.Now;
                db.Tbl_SchoolException.Add(tokenDetail);
                db.SaveChanges();


                Log.Instance.Error(ex, "Add Retailer API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Retailer Registration API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex, 
                    ValidationErrors = null
              
            };

              
              


            }

         

        }

        //public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        //{
        //    byte[] bytes = Convert.FromBase64String(Base64);
        //    MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
        //    ms.Write(bytes, 0, bytes.Length);
        //    Image image = Image.FromStream(ms, true);
        //    //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
        //    string filestoragename = DealerName + SendDateTime;
        //    string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
        //    image.Save(outputPath, ImageFormat.Jpeg);

        //    //string fileName = UserName + ".jpg";
        //    //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
        //    //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
        //    return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        //}



        public class SuccessResponse
        {

        }
        public class CoorporateVisitRequest
        {
            
            public int CustomerID { get; set; }
            public string ProjectTitle { get; set; }
            public string OEMName{ get; set; }
            public int IndustryID { get; set; }
           
            public decimal ProjectEstimatedValue{ get; set; }
            public int SOID { get; set; }
            public int OrderVolume { get; set; }
            public string InfluencerName{ get; set; }
            public string InfluencerNumber{ get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string ArchitectName{ get; set; }
            public string ArchitectNumber{ get; set; }
            public string BuilderName{ get; set; }
            public DateTime NextVisitDate { get; set; }

            public string BuilderNumber{ get; set; }
          
            public string PaintContractorName{ get; set; }
            public string PaintContractorNumber{ get; set; }
            public string ProcurementManager{ get; set; }
            public string ProcurementManagerNo{ get; set; }
            
            public string AccountsManager{ get; set; }
         
            public string AccountsManagerNo{ get; set; }
            public string CompititorName { get; set; }

            public string CompititorNo { get; set; }
            public string Remarks { get; set; }

            // public List<CompititorInfoModel> CompititorInformation { get; set; }



        }

        public class CompititorInfoModel
        {
            public int SaleOfficerID { get; set; }
            public int RetailerID { get; set; }
            public int SylabusID { get; set; }
        }


    }
}