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
    public class SubmitChemSalesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            Tbl_ChemSaleMaster jobDet = new Tbl_ChemSaleMaster();
            var JobObj = new Tbl_ChemSaleDetail();


            //var Lastdata = db.JobsDetails.Where(x => x.RetailerID == rm.RetailerId).OrderByDescending(x => x.ID).FirstOrDefault();
            try
            {
                

                jobDet.SOID = rm.SOID;
                jobDet.OrderDate = rm.OrderDate;
                jobDet.CustomerID = rm.CustomerID;
                jobDet.IsActive = true;
                jobDet.CreatedOn = DateTime.UtcNow.AddHours(5);
                jobDet.TotalQuantity = rm.TotalQuantity;
                jobDet.TotalPrice = rm.TotalPrice;
                jobDet.TotalOrderValue = rm.TotalOrderValue;
                jobDet.Remarks = rm.Remarks;

                if (rm.Picture == "" || rm.Picture == null)
                {
                    jobDet.Picture = null;
                }
                else
                {
                    jobDet.Picture = ConvertIntoByte(rm.Picture, "OrderPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
                }




                db.Tbl_ChemSaleMaster.Add(jobDet);
                db.SaveChanges();


                // Here StockItems is the array which is changed From JobItems Array due to Hassan 

                if (rm.BrightoChemicalItems != null && rm.BrightoChemicalItems.Count > 0)
                {
                    foreach (var item in rm.BrightoChemicalItems)
                    {
                        db.Tbl_ChemSaleDetail.Add(
                            new Tbl_ChemSaleDetail
                            {
                                ChemMasterID = jobDet.ID,
                                ProductID = item.ProductID,
                                Quantity = item.Quantity,
                                OrderDate = jobDet.OrderDate,
                                Cost=item.Cost,
                                IsActive = true,
                                TotalValue = item.TotalValue,
                                CreatedOn = DateTime.UtcNow.AddHours(5),
                            });
                    }
                    // Moved outside the loop to save all records at once
                    db.SaveChanges();
                }







                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Request processed successfully",
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
                BrightoChemicalItems = new List<JobItemModel>();
            }
           
            public DateTime OrderDate { get; set; }
            public int SOID { get; set; }
            public int CustomerID { get; set; }
            public decimal TotalQuantity { get; set; }
           
            public string Remarks { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal TotalOrderValue { get; set; }
            public string Picture { get; set; }
            public List<JobItemModel> BrightoChemicalItems { get; set; }

        }

        public class JobItemModel
        {
            
            public int ProductID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Cost { get; set; }
            public decimal TotalValue { get; set; }

        }
    }
}