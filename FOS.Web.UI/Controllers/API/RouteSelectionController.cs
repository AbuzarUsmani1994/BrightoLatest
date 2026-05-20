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
    public class RouteSelectionController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(BillDispatchedData rm)
        { // This controller is for retailers orders.

            var JobObj = new Job();
            var RemObj = new TblReminder();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
                var data = db.TBL_RouteSelection.Where(r => r.SOID == rm.SOID && r.CreatedOn >= dtFromToday && r.CreatedOn <= dtToToday && r.IsActive == true).FirstOrDefault();
                if (data == null)
                {
                    TBL_RouteSelection jobDet = new TBL_RouteSelection();

                    jobDet.SOID = rm.SOID;
                    jobDet.RegionID = rm.RegionID;
                    jobDet.CityID = rm.CityID;
                    jobDet.IsActive = true;
                    jobDet.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.TBL_RouteSelection.Add(jobDet);
                    db.SaveChanges();




                }
                else
                {
                    TBL_RouteSelection jobDet = new TBL_RouteSelection();
                    data.IsActive = false;
                    db.SaveChanges();
                    jobDet.SOID = rm.SOID;
                    jobDet.RegionID = rm.CityID;
                    jobDet.CityID = rm.CityID;
                    jobDet.IsActive = true;
                    jobDet.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.TBL_RouteSelection.Add(jobDet);
                    db.SaveChanges();
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Area Selected Successfully",
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
        public class BillDispatchedData
        {

            public int CityID { get; set; }
            public int RegionID { get; set; }
          
            public int SOID { get; set; }

        }


    }
}