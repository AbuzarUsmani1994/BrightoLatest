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
using System.Text;
using System.Web;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class UpdateComplaintController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ComplaintsUpdateDto rm)
        {
            // Early validation - return fast if no data
            if (rm == null)
            {
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Invalid request data",
                    ResultType = ResultType.Failure,
                    Exception = null,
                    ValidationErrors = null
                };
            }

            // Check if there's actually any data to update
            if (rm.ComplaintStatusID == null &&
                rm.ProgressStatusID == null &&
                string.IsNullOrEmpty(rm.Picture) &&
                string.IsNullOrEmpty(rm.VoiceNote) &&
                string.IsNullOrEmpty(rm.Video))
            {
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "No data provided for update",
                    ResultType = ResultType.Failure,
                    Exception = null,
                    ValidationErrors = null
                };
            }

            Tbl_ComplaintUpdate jobDet = new Tbl_ComplaintUpdate();

            try
            {
                // Get complaint header - use async or more efficient query
                var ComplaintHeader = db.Tbl_ComplaintUpdate
                    .AsNoTracking() // Important: Read-only for better performance
                    .Where(x => x.ComplaintID == rm.ComplaintID)
                    .OrderByDescending(x => x.LastUpdatedAt) // Get latest
                    .FirstOrDefault();

                if (ComplaintHeader == null)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Complaint not found",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                // Set basic complaint details
                jobDet.ComplaintID = ComplaintHeader.ComplaintID;
                jobDet.ComplaintNumber = ComplaintHeader.ComplaintNumber;
                jobDet.LauncedTime = ComplaintHeader.LauncedTime;
                jobDet.LaunchedBy = ComplaintHeader.LaunchedBy;
                jobDet.ComplaintStatusID = rm.ComplaintStatusID ?? ComplaintHeader.ComplaintStatusID;
                jobDet.ProgressStatusID = rm.ProgressStatusID ?? ComplaintHeader.ProgressStatusID;
                jobDet.LastUpdatedAt = DateTime.UtcNow.AddHours(3);
                jobDet.Remarks = ComplaintHeader.Remarks;
                // Process files only if they are provided
                var processingTasks = new List<System.Threading.Tasks.Task>();

                if (rm.Picture == "" || rm.Picture == null)
                {
                    jobDet.Picture = null;
                }

                else
                {
                    try
                    {
                        jobDet.Picture = FileHandler.ConvertImage(rm.Picture, "ComplaintPictures",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintPictures");
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error(ex, "Failed to process Picture");
                        jobDet.Picture = null;
                    }
                }

                if (rm.VoiceNote == "" || rm.VoiceNote == null)
                {
                    jobDet.Audio = null;
                }
                // Process Voice Note - only if provided

                else
                {
                    try
                    {
                        Log.Instance.Info($"Processing AAC voice note, length: {rm.VoiceNote.Length} characters");
                        jobDet.Audio = VoiceNoteProcessor.ProcessAndSaveVoiceNote(rm.VoiceNote, "ComplaintAudio",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintAudio");
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error(ex, "Failed to process AAC VoiceNote");
                        jobDet.Audio = null;
                    }

                }

                if (rm.Video == "" || rm.Video == null)
                {
                    jobDet.Video = null;
                }
                // Process Video - only if provided

                else
                {
                    try
                    {
                        jobDet.Video = FileHandler.ConvertVideo(rm.Video, "ComplaintVideos",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintVideos");
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error(ex, "Failed to process Video");
                        jobDet.Video = null;
                    }
                }



                // Save main complaint record
                db.Tbl_ComplaintUpdate.Add(jobDet);
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint Updated Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Complaint API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        // OPTIMIZED VoiceNoteProcessor - Reduced logging and validation
        public static class VoiceNoteProcessor
        {
            public static string ProcessAndSaveVoiceNote(string voiceData, string dealerName, string sendDateTime, string folderName)
            {
                if (string.IsNullOrEmpty(voiceData) || voiceData.Length < 100) // Quick validation
                    return null;

                try
                {
                    byte[] aacBytes = ExtractAacAudioBytes(voiceData);
                    if (aacBytes == null || aacBytes.Length == 0)
                        return null;

                    string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
                    string outputPath = HttpContext.Current.Server.MapPath($@"~/Audio/{folderName}/{fileStorageName}.aac");

                    // Ensure directory exists
                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    File.WriteAllBytes(outputPath, aacBytes);
                    return File.Exists(outputPath) ? $"/Audio/{folderName}/{fileStorageName}.aac" : null;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Voice note processing failed");
                    return null;
                }
            }

            private static byte[] ExtractAacAudioBytes(string voiceData)
            {
                try
                {
                    if (string.IsNullOrEmpty(voiceData))
                        return null;

                    // Quick extraction without excessive logging
                    if (voiceData.StartsWith("data:audio/"))
                    {
                        var parts = voiceData.Split(',');
                        if (parts.Length > 1 && IsValidBase64(parts[1]))
                            return Convert.FromBase64String(parts[1]);
                    }

                    if (voiceData.StartsWith("//"))
                    {
                        string cleanData = voiceData.Substring(2);
                        if (IsValidBase64(cleanData))
                            return Convert.FromBase64String(cleanData);
                        return Encoding.UTF8.GetBytes(cleanData);
                    }

                    if (IsValidBase64(voiceData))
                        return Convert.FromBase64String(voiceData);

                    return Encoding.UTF8.GetBytes(voiceData);
                }
                catch
                {
                    return null;
                }
            }

            private static bool IsValidBase64(string base64String)
            {
                if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0)
                    return false;

                try
                {
                    // Test only first 100 chars for performance
                    int testLength = Math.Min(base64String.Length, 100);
                    testLength = testLength - (testLength % 4); // Ensure multiple of 4

                    if (testLength > 0)
                        Convert.FromBase64String(base64String.Substring(0, testLength));

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // OPTIMIZED FileHandler
        public static class FileHandler
        {
            public static string ConvertImage(string base64, string dealerName, string sendDateTime, string folderName)
            {
                if (string.IsNullOrEmpty(base64) || base64.Length < 100)
                    return null;

                try
                {
                    string cleanBase64 = base64.StartsWith("data:") ? base64.Split(',')[1] : base64;
                    byte[] bytes = Convert.FromBase64String(cleanBase64);

                    using (var ms = new MemoryStream(bytes))
                    using (var image = Image.FromStream(ms))
                    {
                        string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
                        string outputPath = HttpContext.Current.Server.MapPath($@"~/Images/{folderName}/{fileStorageName}.jpg");

                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                        image.Save(outputPath, ImageFormat.Jpeg);

                        return $"/Images/{folderName}/{fileStorageName}.jpg";
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Image conversion failed");
                    return null;
                }
            }

            public static string ConvertVideo(string base64, string dealerName, string sendDateTime, string folderName)
            {
                if (string.IsNullOrEmpty(base64) || base64.Length < 100)
                    return null;

                try
                {
                    string cleanBase64 = base64.StartsWith("data:") ? base64.Split(',')[1] : base64;
                    byte[] bytes = Convert.FromBase64String(cleanBase64);

                    string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
                    string outputPath = HttpContext.Current.Server.MapPath($@"~/Videos/{folderName}/{fileStorageName}.mp4");

                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    File.WriteAllBytes(outputPath, bytes);

                    return $"/Videos/{folderName}/{fileStorageName}.mp4";
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Video conversion failed");
                    return null;
                }
            }
        }

        // DTO Classes remain same
        public class SuccessResponse { }

        public class ComplaintsUpdateDto
        {
            public int? ComplaintStatusID { get; set; }
            public int? ComplaintID { get; set; }
            public int? ProgressStatusID { get; set; }
            public string Remarks { get; set; }
            public string Picture { get; set; }
            public string VoiceNote { get; set; }
            public string Video { get; set; }
        }
    }
}
