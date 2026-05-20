using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class MyComplaintsSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID, string DateFrom, string DateTo)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                // Validate input parameters
                if (SOID <= 0)
                {
                    return BadRequest("Invalid SOID");
                }

                if (string.IsNullOrEmpty(DateFrom) || string.IsNullOrEmpty(DateTo))
                {
                    return BadRequest("DateFrom and DateTo are required");
                }

                DateTime dtFromToday = Convert.ToDateTime(DateFrom).Date;
                DateTime dtToToday = Convert.ToDateTime(DateTo).Date.AddDays(1); // Include entire end date

                // ADD DEBUG LOGGING
                Log.Instance.Info($"API Called - SOID: {SOID}, DateFrom: {dtFromToday}, DateTo: {dtToToday}");

                if (SOID > 0)
                {
                    // DEBUG: Check raw data first
                    var rawComplaintUpdates = dbContext.Tbl_ComplaintUpdate
                        .Where(x => x.LaunchedBy == SOID &&
                                    x.LastUpdatedAt >= dtFromToday &&
                                    x.LastUpdatedAt <= dtToToday)
                        .ToList();

                    Log.Instance.Info($"Raw complaint updates found: {rawComplaintUpdates.Count}");

                    // First, get the latest complaint updates - FIXED NULL CHECK
                    var latestComplaintUpdates = dbContext.Tbl_ComplaintUpdate
                        .Where(x => x.LaunchedBy == SOID &&
                                    x.LastUpdatedAt >= dtFromToday &&
                                    x.LastUpdatedAt <= dtToToday)
                        .GroupBy(x => x.ComplaintNumber)
                        .Select(g => g.OrderByDescending(x => x.LastUpdatedAt).FirstOrDefault())
                        .Where(x => x != null) // CRITICAL: Filter out nulls
                        .ToList();

                    Log.Instance.Info($"Latest complaint updates after grouping: {latestComplaintUpdates.Count}");

                    // DEBUG: Check Tbl_SaveComplaints data
                    var complaintIds = latestComplaintUpdates.Select(x => x.ComplaintID).ToList();
                    var saveComplaints = dbContext.Tbl_SaveComplaints
                        .Where(x => complaintIds.Contains(x.ID))
                        .ToList();

                    Log.Instance.Info($"Matching Tbl_SaveComplaints found: {saveComplaints.Count}");

                    // Then join with SaveComplaints to get customer data - FIXED JOIN
                    var result = (from update in latestComplaintUpdates
                                  join complaint in dbContext.Tbl_SaveComplaints
                                  on update.ComplaintID equals complaint.ID // Verify this is correct join
                                  select new
                                  {
                                      ComplaintID = update.ComplaintID,
                                      ComplaintNumber = update.ComplaintNumber,
                                      LaunchedAt = complaint.CreatedOn,
                                      LaunchedBy = dbContext.SaleOfficers
                                                  .Where(r => r.ID == complaint.SOID)
                                                  .Select(r => r.Name)
                                                  .FirstOrDefault() ?? "N/A",
                                      ComplaintStatus = complaint.Status,
                                      ProgressStatus = update.ProgressStatusID,
                                      LastUpdatedAt = update.LastUpdatedAt,
                                      Picture = update.Picture,
                                      Audio = update.Audio,
                                      Video = update.Video,

                                      CustomerName = dbContext.Retailers
                                                  .Where(r => r.ID == complaint.CustomerID)
                                                  .Select(r => r.ShopName)
                                                  .FirstOrDefault() ?? "N/A"
                                  }).ToList();

                    Log.Instance.Info($"Final result count: {result?.Count ?? 0}");

                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {

                            ComplaintsSummery = result,

                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Success = true,
                            ComplaintsSummery = new object[0],
                            Message = "No complaints found for the given criteria",
                            DebugInfo = new
                            {
                                RawUpdatesCount = rawComplaintUpdates.Count,
                                LatestUpdatesCount = latestComplaintUpdates.Count,
                                SaveComplaintsCount = saveComplaints.Count
                            }
                        });
                    }
                }
            }
            catch (FormatException ex)
            {
                Log.Instance.Error(ex, "Invalid date format provided");
                return BadRequest("Invalid date format. Please use valid dates.");
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyComplaintsSOWiseController GET API Failed");
                return InternalServerError(ex);
            }

            return Ok(new
            {
                Success = true,
                ComplaintsSummery = new object[0],
                Message = "No data found"
            });
        }
    }
}
