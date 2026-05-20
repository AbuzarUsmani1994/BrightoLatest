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
    public class ComplaintController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult GetSOComplaints(int soId)
        {
            try
            {
                var list = db.Complaints.Where(d => d.SaleOfficerId == soId && d.Status == (int)StatusEnum.Pending).ToList();
                if (list != null)
                {
                    return Ok(new
                    {
                        Complaints = list.Select(d => new
                        {
                            d.ComplaintID,
                            d.Title,
                            d.Detail,
                            d.ComplaintType,
                            d.DueDate,
                            //DueDateLong = d.DueDate.Ticks,
                            RetailerName = d.Retailer.Name,
                            d.Priority,
                            d.Status,
                            d.Remarks
                        }).OrderBy(d => d.DueDate)
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Complaints = new { }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetSOComplaints List API Failed");
                return Ok(new
                {
                    Complaints = new { }
                });
            }
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateComplaint(ComplaintsModel model)
        {
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(model.Token))
                {
                    Complaint complaint = db.Complaints.Where(u => u.ComplaintID == model.ComplaintId).FirstOrDefault();

                    if (complaint != null)
                    {
                        complaint.Remarks = model.Remarks;
                        complaint.UpdatedBy = model.UpdatedBy;
                        complaint.RemUpdatedOn = DateTime.Now;

                        // Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = model.Token;
                        tokenDetail.Action = "Update Complaint";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END

                        db.SaveChanges();
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = new CheckInLatLongResp
                            {
                                code = "400",
                                message = "Complaint not found in update complaint API"
                            }
                        });
                    }
                    
                    

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Complaint updated successfully"
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
                            message = "Authentication failed in update complaint API"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Complaint API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Complaint API Failed"
                    }
                });
            }
        }
    }
    public class ComplaintsModel
    {
        public int ComplaintId { get; set; }
        public string Remarks { get; set; }
        public int UpdatedBy { get; set; }
        public string Token { get; set; }
    }
}