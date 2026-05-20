using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SubmitCoverageController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

      

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Save Master
                    var jobDet = new Tbl_MasterCoverage
                    {
                        SOID = rm.SOID,
                        CustomerID = rm.CustomerID,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow.AddHours(5)
                    };
                    db.Tbl_MasterCoverage.Add(jobDet);
                    db.SaveChanges();

                    // Verify Master saved
                    if (jobDet.ID == 0)
                        throw new Exception("Master ID not generated!");

                    // 2. Save Details
                    if (rm.BrightoCoverageItems?.Count > 0)
                    {
                        foreach (var item in rm.BrightoCoverageItems)
                        {
                            if (item.ProductID <= 0)
                                throw new Exception($"Invalid ProductID: {item.ProductID}");

                            db.Tbl_DetailCoverage.Add(new Tbl_DetailCoverage
                            {
                                CoverageMasterID = jobDet.ID, // Ensure this is set
                                ProductID = item.ProductID,
                                ProductName = item.ProductName,
                                AreainSqft = item.AreainSqft,
                                AreaInMeters = item.AreaInMeters,
                                DiscountPercent = item.DiscountPercent,
                                Category = item.Category,
                                Drum = item.Drum,
                                Gallon = item.Gallon,
                                Quarter = item.Quarter,
                                NoOfCoats = (int)item.NoOfCoats,
                                Liters = item.Liters,
                                Price = item.Price
                            });
                        }
                        try
                        {
                            db.SaveChanges();
                            Console.WriteLine("Details saved successfully!");
                        }
                        catch (DbUpdateException ex)
                        {
                            Console.WriteLine($"Save failed: {ex.InnerException?.Message}");
                        }
                    }

                    transaction.Commit(); // Explicit commit

                    return new Result<SuccessResponse>
                    {
                        Message = "Saved successfully!",
                        ResultType = ResultType.Success
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Instance.Error(ex, "Save failed!");
                    return new Result<SuccessResponse>
                    {
                        Message = $"Save failed: {ex.Message}",
                        ResultType = ResultType.Exception,
                        Exception = ex
                    };
                }
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
                BrightoCoverageItems = new List<ProductDetail>();
            }
           
            public DateTime OrderDate { get; set; }
            public int SOID { get; set; }
            public int CustomerID { get; set; }
            public decimal TotalQuantity { get; set; }
            public string Remarks { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal TotalOrderValue { get; set; }
            public string Picture { get; set; }
            public List<ProductDetail> BrightoCoverageItems { get; set; }

        }

        public class ProductDetail
        {
          
            public decimal? AreainSqft { get; set; }
            public decimal? AreaInMeters { get; set; }
            public string Category { get; set; }
            public int? ProductID { get; set; }
            public string ProductName { get; set; }
            public int? NoOfCoats { get; set; }
            public decimal? Liters { get; set; }
            public decimal? DiscountPercent { get; set; }
            public decimal? Drum { get; set; }
            public decimal? Gallon { get; set; }
            public decimal? Quarter { get; set; }
            public decimal? Price { get; set; }

        }
    }
}