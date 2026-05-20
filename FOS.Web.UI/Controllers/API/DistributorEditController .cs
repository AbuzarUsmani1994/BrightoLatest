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
    public class DistributorEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DistributorEditmodel rm)
        {
            Dealer retailerObj = new Dealer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //ADD New Retailer 
                    retailerObj = db.Dealers.Where(u => u.ID == rm.ID).FirstOrDefault();

                    retailerObj.Name = rm.OwnerName;
                    retailerObj.NewArea = rm.AreaName;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    // Zone ID  is saving in Regions Table bcx in menu region changes to zone
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.RegionID = rm.RegionID;
                
                    retailerObj.Phone1 = rm.CellNo1;
                    retailerObj.Phone2 = rm.CellNo2;
                    retailerObj.Email = rm.Email;

                    retailerObj.Address = rm.Address;
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


                    retailerObj.LastUpdate = localTime;
                  
                    retailerObj.CreatedBy = rm.SalesOfficerID;

                    //db.Retailers.Add(retailerObj);
                    //END

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Add New Retailer";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Distributor Edit Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
                else
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Authentication failed in Distributor Edit API",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Distributor Edit API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Distributor Edit API Failed",
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
        public class DistributorEditmodel
        {
            public int ID { get; set; }
            public string ShopName { get; set; }
            public string OwnerName { get; set; }
            public string CellNo1 { get; set; }
            public string CellNo2 { get; set; }
            public int SalesOfficerID { get; set; }
            public string Email { get; set; }
            public string AreaName { get; set; }
            public int RegionID { get; set; }
            public int CityID { get; set; }
            public int ZoneID { get; set; }
            public int AreaID { get; set; }
            public string Address { get; set; }
            public string Token { get; set; }
       
  
         
            public string Remarks { get; set; }
            public bool IsVerified { get; set; }
        }

        }
    }
