using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class QrController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult GetQrActivities(int soId)
        {
            try
            {
                var list = db.QrActivities.Where(d => d.Status == (int)StatusEnum.Pending).ToList();
                if (list != null)
                {
                    return Ok(new
                    {
                        QrActivities = list.Select(d => new
                        {
                            d.QrID,
                            d.Title,
                            d.QrCode,
                            CityName = d.City.Name,
                            d.Detail,
                            d.DueDate,
                            //DueDateLong = d.DueDate.Ticks,
                            d.Priority,
                            d.Status
                        }).OrderBy(d => d.DueDate)
                    });
                }
                else
                {
                    return Ok(new
                    {
                        QrActivities = new { }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetQrActivities List API Failed");
                return Ok(new
                {
                    QrActivities = new { }
                });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult AddSOQrDetail(QrModel model)
        {
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(model.Token))
                {
                    QrSODetail soDet = new QrSODetail();

                    soDet.SaleOfficerId = model.SaleOfficerId;
                    soDet.RetailerID = model.RetailerID;
                    soDet.QrCode = model.QrCode;
                    soDet.Remarks = model.Remarks;
                    soDet.CreatedOn = DateTime.Now;
                    soDet.CreatedBy = model.CreatedBy;

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = model.Token;
                    tokenDetail.Action = "Add SO QR Details";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "SO QR added successfully"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in add SO QR API"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SO QR Deail Add API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "SO QR Add API Failed"
                    }
                });
            }
        }
    }
    public class QrModel
    {
        public int QrId { get; set; }
        public int SaleOfficerId { get; set; }
        public int RetailerID { get; set; }
        public string QrCode { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public string Token { get; set; }
    }
}