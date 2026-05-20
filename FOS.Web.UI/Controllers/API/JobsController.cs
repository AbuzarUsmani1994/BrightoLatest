using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class JobsController : ApiController
    {

        public class JobCompleteData
        {
            public int JobID { get; set; }
            public int JobsDetailID { get; set; }
            public int DealerID { get; set; }
            public int SalesOfficerID { get; set; }
            public int RetailerID { get; set; }
            public bool Mapple { get; set; }
            public bool DG { get; set; }
            public bool BestWay { get; set; }
            public bool Lucky { get; set; }
            public bool Other { get; set; }
            public string Major { get; set; }
            public float MapplePrice { get; set; }
            public string Display { get; set; }
            public string Token { get; set; }
        }

        public class CalViewData
        {
            public Nullable<System.DateTime> MasterDate { get; set; }
            public Nullable<int> JobID { get; set; }
            public string JobTitle { get; set; }
            public Nullable<int> JobDetailID { get; set; }
            public Nullable<int> DealerID { get; set; }
            public Nullable<int> SalesOfficerID { get; set; }
            public Nullable<int> RetailerID { get; set; }
            public string Status { get; set; }
            public string AllowCheckin { get; set; }
            public string SaleOfficerName { get; set; }
            public Nullable<int> VisitPlanType { get; set; }
            public Nullable<System.DateTime> DateOfAssign { get; set; }
            public string DealerName { get; set; }
            public string RetailerName { get; set; }
            public string CityName { get; set; }
            public string AreaName { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string Address { get; set; }
            public string ShopName { get; set; }
            public string Location { get; set; }
            public string Target { get; set; }
            public Nullable<int> PainterID { get; set; }
        }

        FOSDataModel db = new FOSDataModel();


        [Route("api/getjobs/{id}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetJobsData(int id)
        {

            try
            {
                string today = DateTime.Now.DayOfWeek.ToString();
                List<int> weeklyTypes = new List<int>() { 2, 3 };
                //var job = db.Jobs.Where(s => s.SaleOfficerID == id && s.Status == false).ToList();
                var job = db.Jobs.Where(s => s.SaleOfficerID == id
                    && ((weeklyTypes.Contains(s.VisitPlanType ?? 0) && s.VisitPlanDays.Contains(today)) || (s.VisitPlanType == 1 && s.StartingDate <= DateTime.Now))
                    && s.Status == false).ToList();


                if (job != null)
                {

                    return Ok(new
                    {
                        Jobs = job.Select(d => new FOS.Shared.JobsData
                        {
                            ID = d.ID,

                            //SalesOfficer ...
                            SaleOfficerID = d.SaleOfficerID,
                            SaleOfficerName = d.SaleOfficer.Name,
                            DealerID = (int)d.JobsDetails.Where(j => j.JobID == d.ID && j.SalesOficerID == id).Select(j => j.DealerID).FirstOrDefault(),
                            DealerName = d.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault(),
                            //CityID = d.JobsDetails.Where(jd => jd.SalesOficerID == id).Select(jd => jd.Retailer.CityID).FirstOrDefault(),
                            //CItyName = d.JobsDetails.Where(jd => jd.SalesOficerID == id).Select(jd => jd.Retailer.Dealer.City.Name).FirstOrDefault(), 
                            //VisitPlanID = d.VisitPlan.ID,
                            //AreaID = FOS.Setup.ManageSaleOffice.GetSaleOfficerAreaID(d.SaleOfficerID).ToString(),
                            //AreaName = FOS.Setup.ManageSaleOffice.GetSaleOfficerAreaName(d.SaleOfficerID),
                            DateOfAssign = d.DateOfAssign.ToString(),
                            //DateOfExecution = d.DateOfExecution.ToString(),

                            Status = d.Status,
                            //Retailers = getJobRetailers(d.ID)

                        })
                    });



                }

                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Jobs List API Failed");
                throw;
            }




        }

        [Route("api/job/{soid}/{sdate}/{edate}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetJobs(int soId, string sDate, string eDate)
        {


            List<CalViewData> lst = new List<CalViewData>();


            DateTime startDate = sDate == "2000-01-01" ? DateTime.Today.AddDays(-4) : DateTime.ParseExact(sDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDate = eDate == "2000-01-01" ? DateTime.Today.AddDays(3) : DateTime.ParseExact(eDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                lst = db.JobsDetails.OrderBy(j => j.JobDate).Where(j =>
                    j.SalesOficerID == soId
                    && j.Job.IsActive == true && j.Job.IsDeleted == false
                    && j.JobDate >= startDate && j.JobDate <= endDate
                    && j.Status == false).Select(j => new CalViewData
                    {
                        JobID = j.Job.ID,
                        JobTitle = j.Job.JobTitle,
                        VisitPlanType = j.Job.VisitPlanType,
                        DateOfAssign = j.Job.DateOfAssign,
                        JobDetailID = j.ID,
                        MasterDate = j.JobDate,
                        SalesOfficerID = j.SalesOficerID,
                        SaleOfficerName = j.Job.SaleOfficer.Name,
                        DealerID = j.DealerID,
                        DealerName = j.Dealer.Name,
                        RetailerID = j.RetailerID,
                        PainterID = j.PainterID,
                        RetailerName = j.Job.VisitType == "Shop" ? j.Retailer.Name : "",//db.vPainters.Where(p => p.ID == j.PainterID).Select(p => p.Name).FirstOrDefault(),
                        ShopName = j.Retailer.ShopName,
                        Address = j.Retailer.Address,
                        Location = j.Retailer.Location,
                        Phone1 = j.Retailer.Phone1,
                        Phone2 = j.Retailer.Phone2,
                        AreaName = j.Area.Name,
                        CityName = j.Area.City.Name,
                        Target = j.Job.VisitType == "Shop" ? "R" : "P",
                        AllowCheckin = "Y",//(DateTime)j.JobDate == DateTime.Today ? "Y" : "N",
                        Status = j.Status == true ? "DONE" : "PENDING"
                    }).ToList();


                //lst = db.CalView_All(startDate, endDate, soId).ToList();

                if (lst != null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        if (item.MasterDate >= DateTime.Now.AddDays(-4) && item.MasterDate <= DateTime.Now.AddDays(3))
                        {
                            item.AllowCheckin = "Y";
                        }
                    }
                    return Ok(lst);
                }
                else
                    return Ok(new List<CalViewData>());
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get jobs API Failed");
                //throw new WebException("Error: Sales Officer Login API Failed", ex); 
                return Ok(new List<CalViewData>());
            }
        }

        [Route("api/getretailercount/{id}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public int GetRetailerCount(int id)
        {
            int Res = 0;
            Res = db.JobsDetails.Where(s => s.JobID == s.Job.ID && s.Job.SaleOfficerID == id && s.Status == false && s.Job.IsActive == true).Select(s => s.RetailerID).Count();
            return Res;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public bool JobCompletion(JobCompleteData jobComplete)
        {
            bool Res = true;

            JobsHistory jobInquiry = new JobsHistory();

            var job = db.Jobs.Where(j => j.ID == jobComplete.JobID).FirstOrDefault();
            JobsDetail jobsDetail = job.JobsDetails.Where(s => s.RetailerID == jobComplete.RetailerID).FirstOrDefault();

            try
            {

                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(jobComplete.Token))
                {

                    //Add Record To jobsDetail Table ...
                    jobsDetail.ID = jobComplete.JobsDetailID;
                    jobsDetail.JobID = jobComplete.JobID;
                    jobsDetail.DealerID = jobComplete.DealerID;
                    jobsDetail.SalesOficerID = jobComplete.SalesOfficerID;
                    jobsDetail.RetailerID = jobComplete.RetailerID;

                    jobsDetail.Status = true;
                    jobsDetail.DateComplete = DateTime.Now;
                    //END

                    //Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = jobComplete.Token;
                    tokenDetail.Action = "Complete A Job";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();
                    Res = true;
                }
                else
                {
                    Res = false;
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Job Completion API Failed");
                Res = false;
            }

            return Res;
        }

        //public List<RetailerData> getJobRetailers(int ID)
        //{

        //    var result = (
        //        // instance from context
        //        from a in db.Jobs
        //            // instance from navigation property
        //        from b in a.JobsDetails
        //            //join to bring useful data
        //        join c in db.Retailers on b.RetailerID equals c.ID
        //        where a.ID == ID && b.Status == false
        //        select new RetailerData
        //        {
        //            ID = c.ID,
        //            Name = c.Name,
        //            DealerID = c.DealerID,
        //            DealerName = c.Dealer.Name,
        //            SaleOfficerID = c.SaleOfficerID,
        //            SaleOfficerName = c.SaleOfficer.Name,
        //            JobDetailID = b.ID,
        //            CityID = c.CityID,
        //            CItyName = c.Dealer.City.Name,
        //            Address = c.Address,
        //            ShopName = c.ShopName,
        //            Location = c.Location,
        //            LocationMargin = c.LocationMargin,
        //            Phone1 = c.Phone1,
        //            Phone2 = c.Phone2,
        //        }).ToList();


        //    return result;
        //}


        [Route("api/getplansdetail/{SaleOfficerID}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public CompletePlanData CompleteJobData(int SaleOfficerID)
        {
            CompletePlanData planData = new CompletePlanData();

            try
            {
                planData.MonthlyVisit = db.JobsDetails.Where(j => j.Job.VisitPlanType == 1).Select(j => j.JobID).Count();
                planData.WeeklyVisit = db.JobsDetails.Where(j => j.Job.VisitPlanType == 2).Select(j => j.JobID).Count();
                planData.DailyVisit = db.JobsDetails.Where(j => j.Job.VisitPlanType == 3).Select(j => j.JobID).Count();
            }
            catch (Exception)
            {
                throw;
            }
            return planData;
        }


        [Route("api/getundonejobs/{SaleOfficerID}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public int UnDoneJobData(int SaleOfficerID)
        {
            int Res = 0;
            try
            {
                Res = db.JobsDetails.Where(j => j.SalesOficerID == SaleOfficerID && j.Status == false).Count();
            }
            catch (Exception)
            {
                Res = -1;
            }
            return Res;
        }

        [Route("api/getdonejobs/{SaleOfficerID}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public int DoneJobData(int SaleOfficerID)
        {
            int Res = 0;
            try
            {
                Res = db.JobsDetails.Where(j => j.SalesOficerID == SaleOfficerID && j.Status == true).Count();
            }
            catch (Exception)
            {
                Res = -1;
            }
            return Res;
        }

        public IHttpActionResult GetSOJobsCount(int soId, string type)
        {
            if (soId > 0 && type != null)
            {
                if (type.Equals("all"))
                {
                    return Ok(new
                    {
                        JobsCount = db.JobsDetails.Where(j => j.SalesOficerID == soId && j.Status == false && j.Job.IsDeleted == false && j.Job.IsActive == true).Count()
                    });
                }
                else if (type.Equals("today"))
                {
                    return Ok(new
                    {
                        JobsCount = db.JobsDetails.Where(j => j.SalesOficerID == soId && j.Status == false && j.Job.IsDeleted == false && j.Job.IsActive == true && j.JobDate == DateTime.Today).Count()
                    });
                }
            }

            return Ok(new
            {
                JobsCount = 0
            });
        }

        [HttpGet]
        public IHttpActionResult GetSONotifications(int soId, int regionalHeadId)
        {
            int jobCountToday = 0;
            int jobCountSevenDays = 0;
            int todoCount = 0;
            int complaintCount = 0;
            //int qrCount = 0;
            DateTime stDate = DateTime.Today.AddDays(-4);
            DateTime endDate = DateTime.Today.AddDays(3);
            if (soId > 0 && regionalHeadId > 0)
            {
                jobCountToday = db.JobsDetails.Where(j => j.SalesOficerID == soId && j.Status == false && j.Job.IsDeleted == false && j.Job.IsActive == true && j.JobDate == DateTime.Today).Count();
                jobCountSevenDays = db.JobsDetails.Where(j => j.SalesOficerID == soId && j.Status == false
                                                        && j.Job.IsDeleted == false && j.Job.IsActive == true
                                                        && j.JobDate >= stDate && j.JobDate <= endDate).Count();
                todoCount = db.Todoes.Where(t => t.Status == (int)StatusEnum.Pending && t.RegionalHeadId == regionalHeadId).Count();
                complaintCount = db.Complaints.Where(t => t.Status == (int)StatusEnum.Pending && t.SaleOfficerId == soId).Count();
                //qrCount = db.QrActivities.Where(t => t.Status == (int)StatusEnum.Pending).Count();
            }

            return Ok(new
            {
                JobsCountToday = jobCountToday,
                JobsCountSevenDays = jobCountSevenDays,
                TodosCount = todoCount,
                ComplaintsCount = complaintCount,
                //QRCount = qrCount,
                HBDCount = 0
            });
        }

        [HttpGet]
        public IHttpActionResult GetRetailerLastOrder(int retailerId)
        {
            if (retailerId > 0)
            {
                var jobDetail = db.JobsDetails.Where(j => j.RetailerID == retailerId && j.Status.Value && j.DateComplete.HasValue).OrderByDescending(p => p.DateComplete).FirstOrDefault();

                if (jobDetail != null)
                {
                    return Ok(new
                    {
                        Previous1KG = jobDetail.SNewQuantity1KG,
                        Previous5KG = jobDetail.SNewQuantity5KG
                    });
                }
            }

            return Ok(new
            {
                Previous1KG = 0,
                Previous5KG = 0,
            });

        }

        [Route("api/getalljobs/{SaleOfficerID}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult DonAndUnDoneJobData(int SaleOfficerID)
        {
            List<JobsDetailData> jobsDetail = new List<JobsDetailData>();
            return Ok(new
            {

                Jobs = db.JobsDetails
                            .Where(j => j.SalesOficerID == SaleOfficerID && j.Job.JobsHistories.Where(jh => jh.SaleOfficerID == SaleOfficerID && jh.Status == false).Count() >= 0)
                            .Select(j => new JobsDetailData
                            {
                                ID = j.JobID,
                                JobTitle = j.Job.JobTitle,
                                SaleOfficerID = (int)j.SalesOficerID,
                                SaleOfficerName = j.Job.RegionalHead.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.Name).FirstOrDefault(),
                                VisitDate = j.Job.LastProcessed,
                                Status = j.Status,
                            })
                            .ToList()
            });

        }


        public class TodayJob
        {
            public List<RetailerData> TodayJobs { get; set; }
        }




        
        #region WALLCOAT JOB COMPLETION APIS


        public class ShopJobData
        {
            public int JobID { get; set; }
            public int JobsDetailID { get; set; }
            public int DealerID { get; set; }
            public int SalesOfficerID { get; set; }
            public int RetailerID { get; set; }
            public bool SAvailable { get; set; }
            public int SQuantity1KG { get; set; }
            public int SQuantity5KG { get; set; }
            public bool SNewOrder { get; set; }
            public int SNewQuantity1KG { get; set; }
            public int SNewQuantity5KG { get; set; }
            public bool SPreviousOrderDelievered { get; set; }
            public int SPreviousOrder1KG { get; set; }
            public int SPreviousOrder5KG { get; set; }
            public bool SPOSMaterialAvailable { get; set; }
            public string SImage { get; set; }
            public string SNote { get; set; }
            public bool SUblAcctOpened { get; set; }
            public bool SBrochureAvailable { get; set; }
            public bool SSmsCardAvailable { get; set; }
            public bool SShadeCardAvailable { get; set; }
            public bool SDisplay { get; set; }
            public bool SWhite40KgAvailable { get; set; }
            public string SMSPIN { get; set; }
            public string Token { get; set; }
            public bool SendUpdatedSMS { get; set; }
        }

        private string CallSmsApi(string phoneNo, string msgText)
        {
            string URL = "http://api.bizsms.pk/web-ported-to-sms.aspx";

            string urlParameters = "?username=mapleaf@bizsms.pk&pass=mapl3w23f&text="
                + msgText + "&masking=MLCF&destinationnum="
                + phoneNo + "&language=English";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                return "1";
            }
            else
            {
                return "Response StatusCode: " + (int)response.StatusCode + " :: ReasonPhrase :: " + response.ReasonPhrase;
            }
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        [HttpGet]
        public IHttpActionResult SendSMS(int soId, int retailerId, string oneKgQty, string fiveKgQty)
        {
            try
            {
                var retailer = db.Retailers.Where(p => p.ID == retailerId).FirstOrDefault();
                string phoneNo = "";
                string phone2 = "";
                if (retailer != null)
                {
                    if (!string.IsNullOrEmpty(retailer.Phone1))
                    {
                        phoneNo = retailer.Phone1.Trim();
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = new CheckInLatLongResp
                            {
                                code = "400",
                                message = "SMS cannot be send successfully, phone 1 not found"
                            }
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "SMS cannot be send successfully, retailer not found"
                        }
                    });
                }

                var smsPin = "" + GenerateRandomNo();
                var msgText = "Thank you for submitting your order of 5Kg Packs = " + fiveKgQty +
                    " Kg, 1Kg Packs = " + oneKgQty +
                    " Kg. Order Verification Code = " + smsPin;
                var error = "";
                var result = CallSmsApi(phoneNo, msgText);

                if (retailer != null)
                {
                    phone2 = retailer.Phone2;
                    if (phone2 != null && phone2.Trim().Length > 0)
                    {
                        CallSmsApi(phone2, msgText);
                    }
                }

                if (!result.Equals("1"))
                {
                    error = result;
                }

                SMSLog sms = new SMSLog
                {
                    PhoneNo = phoneNo,
                    RetailerID = retailerId,
                    SaleOfficerID = soId,
                    CreatedOn = DateTime.Now,
                    SmsPin = smsPin,
                    ErrorDetail = error,
                    Status = error.Equals("") ? 1 : 0
                };

                db.SMSLogs.Add(sms);
                db.SaveChanges();

                if (result.Equals("1"))
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "SMS send successfully"
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
                            message = "SMS cannot be send successfully"
                        }
                    });
                }
            }
            catch
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "SMS cannot be send successfully, something went wrong"
                    }
                });
            }
        }

        [HttpGet]
        public IHttpActionResult SMSPinVerification(int soId, int retailerId, string smsPin)
        {
            var retailer = db.Retailers.Where(p => p.ID == retailerId).FirstOrDefault();
            string phoneNo = "";
            if (retailer != null)
            {
                phoneNo = retailer.Phone1.Trim();
            }


            if (!string.IsNullOrEmpty(smsPin) && !string.IsNullOrEmpty(phoneNo))
            {
                if (IsSMSPINValid(smsPin, soId, retailerId, phoneNo))
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Valid SMS PIN"
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
                            message = "Invalid SMS PIN"
                        }
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "SMS PIN or Phone No is not provided"
                    }
                });
            }

        }

        private bool IsSMSPINValid(string pin, int soId, int retailerId, string phoneNo)
        {
            DateTime today = DateTime.Now.AddHours(-1);
            var smsLog = db.SMSLogs.Where(p => p.RetailerID == retailerId
                            && p.SaleOfficerID == soId && p.PhoneNo.Equals(phoneNo)
                            && p.CreatedOn >= today
                            && p.Status == (int)StatusEnum.Completed).OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            if (smsLog != null && smsLog.SmsPin.Equals(pin))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult OfflineShopJobCompletion(ShopJobData offlineJob)
        {
            var JobObj = new Job();
            if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(offlineJob.Token))
            {
                try
                {
                    Retailer ret = db.Retailers.Where(r => r.ID == offlineJob.RetailerID).FirstOrDefault();
                    if (ret != null)
                    {
                        JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        JobObj.JobTitle = "";
                        JobObj.SaleOfficerID = offlineJob.SalesOfficerID;
                        JobObj.RegionalHeadID = ret.SaleOfficer.RegionalHeadID;
                        JobObj.RetailerType = ret.RetailerType;
                        JobObj.RegionalHeadType = ret.SaleOfficer.RegionalHead.Type;
                        JobObj.VisitType = "Shop";
                        JobObj.Status = true;
                        JobObj.JobType = "A";
                        JobObj.VisitPlanType = 7;

                        JobObj.StartingDate = DateTime.Now;
                        JobObj.DateOfAssign = DateTime.Now;
                        JobObj.CreatedDate = DateTime.Now;
                        JobObj.LastUpdated = DateTime.Now;
                        JobObj.IsActive = true;
                        JobObj.IsDeleted = false;

                        JobsDetail jobDetail = new JobsDetail();
                        jobDetail.JobID = JobObj.ID;
                       // jobDetail.DealerID = ret.DealerID;
                        jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                        jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                        jobDetail.RetailerID = offlineJob.RetailerID;

                        jobDetail.JobDate = DateTime.Today;
                        jobDetail.Status = true;
                        jobDetail.VisitPlanType = 7;

                        jobDetail.SAvailable = offlineJob.SAvailable;
                        jobDetail.SQuantity1KG = offlineJob.SQuantity1KG;
                        jobDetail.SQuantity5KG = offlineJob.SQuantity5KG;
                        jobDetail.SNewOrder = offlineJob.SNewOrder;
                        jobDetail.SNewQuantity1KG = offlineJob.SNewQuantity1KG;
                        jobDetail.SNewQuantity5KG = offlineJob.SNewQuantity5KG;
                        jobDetail.SPreviousOrderDelievered = offlineJob.SPreviousOrderDelievered;
                        jobDetail.SPreviousOrder1KG = offlineJob.SPreviousOrder1KG;
                        jobDetail.SPreviousOrder5KG = offlineJob.SPreviousOrder5KG;
                        jobDetail.SPOSMaterialAvailable = offlineJob.SPOSMaterialAvailable;

                        jobDetail.SUblAcctOpened = offlineJob.SUblAcctOpened;
                        jobDetail.SBrochureAvailable = offlineJob.SBrochureAvailable;
                        jobDetail.SShadeCardAvailable = offlineJob.SShadeCardAvailable;
                        jobDetail.SSmsCardAvailable = offlineJob.SSmsCardAvailable;
                        jobDetail.SDisplay = offlineJob.SDisplay;
                        jobDetail.SWhite40KgAvailable = offlineJob.SWhite40KgAvailable;

                        if (offlineJob.SImage == "" || offlineJob.SImage == null)
                        {
                            jobDetail.SImage = null;
                        }
                        else
                        {
                            jobDetail.SImage = ConvertIntoByte(offlineJob.SImage, offlineJob.RetailerID, "ret", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""));
                        }
                        jobDetail.SNote = offlineJob.SNote;
                        jobDetail.Status = true;
                        JobObj.Status = true;
                        jobDetail.DateComplete = DateTime.Now;
                        //END

                        //Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = offlineJob.Token;
                        tokenDetail.Action = "Complete A Retailer offline Job";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END
                        db.Jobs.Add(JobObj);
                        db.SaveChanges();
                        jobDetail.JobID = JobObj.ID;
                        db.JobsDetails.Add(jobDetail);
                        db.SaveChanges();

                        if (offlineJob.SendUpdatedSMS)
                        {
                            try
                            {
                                SendSMSUpdated(jobDetail.SalesOficerID.Value, jobDetail.RetailerID.Value, offlineJob.SNewQuantity1KG + "", offlineJob.SNewQuantity5KG + "");
                            }
                            catch { }
                        }

                        return Ok(new
                        {
                            status = new CheckInLatLongResp
                            {
                                code = "200",
                                message = "Order submitted successfully"
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
                                message = "Please provide retailer id"
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Offline Shop Job Completion API Failed");
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Offline Shop Job Completion API Failed"
                        }
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Please provide valid token to complete offline job form"
                    }
                });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult ShopJobCompletion(ShopJobData jobComplete)
        {
            var job = db.Jobs.Where(j => j.ID == jobComplete.JobID).FirstOrDefault();
            if (job != null && job.JobsDetails != null)
            {
                JobsDetail jobsDetail = job.JobsDetails.Where(j => j.ID == jobComplete.JobsDetailID).FirstOrDefault();

                if (jobsDetail != null)
                {
                    try
                    {
                        if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(jobComplete.Token))
                        {
                            bool smsFlag = false;
                            if (jobComplete.SNewOrder)
                            {
                                smsFlag = true;//IsSMSPINValid(jobComplete.SMSPIN, jobsDetail.SalesOficerID.Value, jobsDetail.RetailerID.Value, jobsDetail.Retailer.Phone1);
                            }
                            else
                            {
                                // so it means user is not saving NEW ORDER - in this case SMS verification is not required.
                                smsFlag = true;
                            }
                            if (smsFlag)
                            {
                                jobsDetail.SAvailable = jobComplete.SAvailable;
                                jobsDetail.SQuantity1KG = jobComplete.SQuantity1KG;
                                jobsDetail.SQuantity5KG = jobComplete.SQuantity5KG;
                                jobsDetail.SNewOrder = jobComplete.SNewOrder;
                                jobsDetail.SNewQuantity1KG = jobComplete.SNewQuantity1KG;
                                jobsDetail.SNewQuantity5KG = jobComplete.SNewQuantity5KG;
                                jobsDetail.SPreviousOrderDelievered = jobComplete.SPreviousOrderDelievered;
                                jobsDetail.SPreviousOrder1KG = jobComplete.SPreviousOrder1KG;
                                jobsDetail.SPreviousOrder5KG = jobComplete.SPreviousOrder5KG;
                                jobsDetail.SPOSMaterialAvailable = jobComplete.SPOSMaterialAvailable;

                                jobsDetail.SUblAcctOpened = jobComplete.SUblAcctOpened;
                                jobsDetail.SBrochureAvailable = jobComplete.SBrochureAvailable;
                                jobsDetail.SShadeCardAvailable = jobComplete.SShadeCardAvailable;
                                jobsDetail.SSmsCardAvailable = jobComplete.SSmsCardAvailable;
                                jobsDetail.SDisplay = jobComplete.SDisplay;
                                jobsDetail.SWhite40KgAvailable = jobComplete.SWhite40KgAvailable;

                                if (jobComplete.SImage == "" || jobComplete.SImage == null)
                                {
                                    jobsDetail.SImage = null;
                                }
                                else
                                {
                                    jobsDetail.SImage = ConvertIntoByte(jobComplete.SImage, jobsDetail.RetailerID.Value, "ret", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""));
                                }
                                jobsDetail.SNote = jobComplete.SNote;
                                jobsDetail.Status = true;
                                job.Status = true;
                                jobsDetail.DateComplete = DateTime.Now;
                                //END

                                //Add Token Detail ...
                                TokenDetail tokenDetail = new TokenDetail();
                                tokenDetail.TokenName = jobComplete.Token;
                                tokenDetail.Action = "Complete A Retailer Job";
                                tokenDetail.ProcessedDateTime = DateTime.Now;
                                db.TokenDetails.Add(tokenDetail);
                                //END

                                db.SaveChanges();

                                if (jobComplete.SendUpdatedSMS)
                                {
                                    try
                                    {
                                        SendSMSUpdated(jobsDetail.SalesOficerID.Value, jobsDetail.RetailerID.Value, jobComplete.SNewQuantity1KG + "", jobComplete.SNewQuantity5KG + "");
                                    }
                                    catch { }
                                }

                                return Ok(new
                                {
                                    status = new CheckInLatLongResp
                                    {
                                        code = "200",
                                        message = "Order submitted successfully"
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
                                        message = "Invalid SMS PIN"
                                    }
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = new CheckInLatLongResp
                                {
                                    code = "400",
                                    message = "Please provide valid token to complete job form"
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error(ex, "Shop Job Completion API Failed");
                        return Ok(new
                        {
                            status = new CheckInLatLongResp
                            {
                                code = "400",
                                message = "Shop job completion market info form API failed"
                            }
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Please provide job details Id"
                        }
                    });
                }
            }
            return Ok(new
            {
                status = new CheckInLatLongResp
                {
                    code = "400",
                    message = "Please provide job Id"
                }
            });
        }

        public void SendSMSUpdated(int soId, int retailerId, string oneKgQty, string fiveKgQty)
        {
            var retailer = db.Retailers.Where(p => p.ID == retailerId).FirstOrDefault();
            string phoneNo = "";
            string phone2 = "";
            if (retailer != null)
            {
                phoneNo = retailer.Phone1.Trim();
            }

            var msgText = "Your order of 5Kg = " + fiveKgQty +
                ", 1Kg = " + oneKgQty + " is confirmed.";
            
            var result = CallSmsApi(phoneNo, msgText);

            if (retailer != null)
            {
                phone2 = retailer.Phone2;
                if (phone2 != null && phone2.Trim().Length > 0)
                {
                    CallSmsApi(phone2, msgText);
                }
            }
        }
        public string ConvertIntoByte(string Base64, int RetailerID, string RetailerName, string SendDateTime)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = RetailerID + RetailerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/RetailerJobImages/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/RetailerJobImages/" + RetailerID + "" + RetailerName + "" + SendDateTime + ".jpg";
        }

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }


        public class PainterJobData
        {
            public int JobID { get; set; }
            public int JobsDetailID { get; set; }
            public int SalesOfficerID { get; set; }
            public int PainterID { get; set; }
            public bool PUseWC { get; set; }
            public int PUseWC1KG { get; set; }
            public int PUseWC5KG { get; set; }
            public bool PNewOrder { get; set; }
            public int PNewOrder1KG { get; set; }
            public int PNewOrder5KG { get; set; }
            public bool PNewLead { get; set; }
            public string PNewLeadMobNo { get; set; }
            public string PRemarks { get; set; }
            public string Token { get; set; }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public bool PainterJobCompletion(PainterJobData jobComplete)
        {
            bool Res = true;

            JobsHistory jobInquiry = new JobsHistory();

            var job = db.Jobs.Where(j => j.ID == jobComplete.JobID).FirstOrDefault();
            JobsDetail jobsDetail = job.JobsDetails.Where(s => s.PainterID == jobComplete.PainterID).FirstOrDefault();
            //JobsHistory jobsHistory = new JobsHistory();
            try
            {

                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(jobComplete.Token))
                {

                    //Add Record To jobsDetail Table ...
                    jobsDetail.ID = jobComplete.JobsDetailID;
                    jobsDetail.JobID = jobComplete.JobID;
                    jobsDetail.SalesOficerID = jobComplete.SalesOfficerID;
                    jobsDetail.PainterID = jobComplete.PainterID;
                    jobsDetail.PUseWC = jobComplete.PUseWC;
                    jobsDetail.PUseWC1KG = jobComplete.PUseWC1KG;
                    jobsDetail.PUseWC5KG = jobComplete.PUseWC5KG;
                    jobsDetail.PNewOrder = jobComplete.PNewOrder;
                    jobsDetail.PNewOrder1KG = jobComplete.PNewOrder1KG;
                    jobsDetail.PNewOrder5KG = jobComplete.PNewOrder5KG;
                    jobsDetail.PNewLead = jobComplete.PNewLead;
                    jobsDetail.PNewLeadMobNo = jobComplete.PNewLeadMobNo;
                    jobsDetail.PRemarks = jobComplete.PRemarks;
                    jobsDetail.Status = true;
                    jobsDetail.DateComplete = DateTime.Now;
                    //END

                    //Add Record To JobHistory Table ...
                    //jobsHistory.SoRetailerJobID = jobComplete.JobsDetailID;
                    //jobsHistory.JobID = jobComplete.JobID;
                    //jobsHistory.SaleOfficerID = jobComplete.SalesOfficerID;
                    //jobsHistory.PainterID = jobComplete.PainterID;
                    //jobsHistory.PUseWC = jobComplete.PUseWC;
                    //jobsHistory.PUseWC1KG = jobComplete.PUseWC1KG;
                    //jobsHistory.PUseWC5KG = jobComplete.PUseWC5KG;
                    //jobsHistory.PNewOrder = jobComplete.PNewOrder;
                    //jobsHistory.PNewOrder1KG = jobComplete.PNewOrder1KG;
                    //jobsHistory.PNewOrder5KG = jobComplete.PNewOrder5KG;
                    //jobsHistory.PNewLead = jobComplete.PNewLead;
                    //jobsHistory.PNewLeadMobNo = jobComplete.PNewLeadMobNo;
                    //jobsHistory.PRemarks = jobComplete.PRemarks;
                    //jobsHistory.Status = true;
                    //jobsHistory.VisitedDate = DateTime.Now;
                    //db.JobsHistories.Add(jobsHistory);
                    //END

                    //Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = jobComplete.Token;
                    tokenDetail.Action = "Complete A Painter Job";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();
                    Res = true;
                }
                else
                {
                    Res = false;
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Painter Job Completion API Failed");
                Res = false;
            }

            return Res;
        }


        public class B2BJobData
        {
            public int JobID { get; set; }
            public int AreaID { get; set; }
            public double Lattitude { get; set; }
            public double Longitude { get; set; }
            public string BLocationName { get; set; }
            public int SalesOfficerID { get; set; }
            public string BShop { get; set; }
            public string BOldHouse { get; set; }
            public string BNewHouse { get; set; }
            public string BParking { get; set; }
            public string BPlazaBasement { get; set; }
            public string BFactoryArea { get; set; }
            public string BMosque { get; set; }
            public string BOthers { get; set; }
            public bool BLead { get; set; }
            public bool BSampleApplied { get; set; }
            public string BRemarks { get; set; }
            public string Token { get; set; }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public bool B2BJobCompletion(B2BJobData jobComplete)
        {
            bool Res = true;
            var job = db.Jobs.Where(j => j.ID == jobComplete.JobID).FirstOrDefault();

            try
            {

                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(jobComplete.Token))
                {
                    JobsDetail jobsDetail = new JobsDetail();
                    //JobsHistory jobsHistory = new JobsHistory();

                    //Add Record To jobsDetail Table ...
                    jobsDetail.JobID = jobComplete.JobID;
                    jobsDetail.SalesOficerID = jobComplete.SalesOfficerID;
                    jobsDetail.BAreaID = jobComplete.AreaID;
                    jobsDetail.BLocation = "" + Math.Round(jobComplete.Lattitude, 12) + "," + Math.Round(jobComplete.Longitude, 12) + "";
                    jobsDetail.BLocationName = jobComplete.BLocationName;
                    jobsDetail.BShop = jobComplete.BShop == "" ? null : jobComplete.BShop;
                    jobsDetail.BOldHouse = jobComplete.BOldHouse == "" ? null : jobComplete.BOldHouse;
                    jobsDetail.BNewHouse = jobComplete.BNewHouse == "" ? null : jobComplete.BNewHouse;
                    jobsDetail.BParking = jobComplete.BParking == "" ? null : jobComplete.BParking;
                    jobsDetail.BPlazaBasement = jobComplete.BPlazaBasement == "" ? null : jobComplete.BPlazaBasement;
                    jobsDetail.BFactoryArea = jobComplete.BFactoryArea == "" ? null : jobComplete.BFactoryArea;
                    jobsDetail.BMosque = jobComplete.BMosque == "" ? null : jobComplete.BMosque;
                    jobsDetail.BOthers = jobComplete.BOthers == "" ? null : jobComplete.BOthers;
                    jobsDetail.BLead = jobComplete.BLead;
                    jobsDetail.BSampleApplied = jobComplete.BSampleApplied;
                    jobsDetail.BRemarks = jobComplete.BRemarks;
                    jobsDetail.Status = true;
                    jobsDetail.DateComplete = DateTime.Now;
                    db.JobsDetails.Add(jobsDetail);
                    //END

                    //Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = jobComplete.Token;
                    tokenDetail.Action = "Complete A B2B Job";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();
                    Res = true;
                }
                else
                {
                    Res = false;
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "B2B Job Completion API Failed");
                Res = false;
            }

            return Res;
        }


        public class AreaInfo
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        [Route("api/job/{soid}/{jobid}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetJobs(int soid, int jobid)
        {

            List<AreaInfo> lst = new List<AreaInfo>();

            try
            {
                var AreaName = db.Jobs.Where(j => j.ID == jobid && j.SaleOfficerID == soid).Select(j => j.Areas).FirstOrDefault();

                return Ok(new
                {
                    Areas = db.GetSalesOfficerJobAreas(soid, jobid, AreaName).Select(
                            u => new AreaInfo
                            {
                                ID = u.ID,
                                Name = u.Name,
                            }).ToList()
                });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get jobs API Failed");
                //throw new WebException("Error: Sales Officer Login API Failed", ex); 
                return Ok(lst);
            }
        }



        #endregion


        #region new apis


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ValidateCheckInLatLong(CheckInLatLongReq req)
        {
            if (req.RetailerId > 0 && !string.IsNullOrEmpty(req.Latitude.ToString()) && !string.IsNullOrEmpty(req.Longitude.ToString()))
            {
                object[] param = { req.Latitude, req.Longitude, req.RetailerId };
                bool flag = false;
                decimal distance = 0;
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var result = dbContext.Database.SqlQuery<SpReturn>("exec spValidateCheckInLatLong @latitude, @longitude, @shopId",
                                    new SqlParameter("latitude", param[0]),
                                    new SqlParameter("longitude", param[1]),
                                    new SqlParameter("shopId", param[2])).ToList();
                    if (result.Count() > 0)
                    {
                        distance = result.FirstOrDefault().distance;
                        flag = true;
                    }
                }
                if (flag)
                {

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "ok"
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
                            message = "No shop found nearby"
                        }
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Please provide lat long"
                    }
                });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateJobMarketInfoForm(JobMarketInfoData model)
        {
            JobsDetail jobsDetail = db.JobsDetails.Where(j => j.ID == model.JobsDetailID).FirstOrDefault();

            if (jobsDetail != null)
            {
                try
                {

                    if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(model.Token))
                    {
                        jobsDetail.SPOSMaterialAvailable = model.SPOSMaterialAvailable;
                        jobsDetail.SUblAcctOpened = model.SUblAcctOpened;
                        jobsDetail.SBrochureAvailable = model.SBrochureAvailable;
                        jobsDetail.SShadeCardAvailable = model.SShadeCardAvailable;
                        jobsDetail.SSmsCardAvailable = model.SSmsCardAvailable;
                        jobsDetail.SDisplay = model.SDisplay;
                        jobsDetail.SWhite40KgAvailable = model.SWhite40KgAvailable;
                        jobsDetail.SMarketInfoNote = model.SMarketInfoNote;
                        //END



                        //Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = model.Token;
                        tokenDetail.Action = "Complete a retailer job Market Info form";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END

                        db.SaveChanges();
                        return Ok(new
                        {
                            status = new CheckInLatLongResp
                            {
                                code = "200",
                                message = "ok"
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
                                message = "Please provide valid token to update market info form"
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Shop job completion market info form API failed");
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Shop job completion market info form API failed"
                        }
                    });
                }
            }
            else
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Please provide job details Id"
                    }
                });
            }
        }

        #endregion

        #region new api for calendar

        [HttpPost]
        public IHttpActionResult UpdateSOJob(JobUpdateForCalander model)
        {
            try
            {
                var jobDet = db.JobsDetails.Where(p => p.ID == model.JobDetailID).FirstOrDefault();
                if (jobDet != null)
                {
                    if (model.VisitPlanType == 1 || model.VisitPlanType == 6)
                    {
                        if (!jobDet.Status.Value)
                        {
                            jobDet.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11));
                            db.SaveChanges();
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = new CheckInLatLongResp
                                {
                                    code = "404",
                                    message = "This job is already visited and so cannot be updated"
                                }
                            });
                        }
                    }
                    else
                    {
                        var job = jobDet.Job;
                        if (job.JobsDetails.Any(p => p.Status == true && p.RetailerID == model.RetailerID))
                        {
                            return Ok(new
                            {
                                status = new CheckInLatLongResp
                                {
                                    code = "404",
                                    message = "One of retailer's job is already visited and so this beat plan cannot be updated"
                                }
                            });
                        }
                        else
                        {
                            if (model.VisitPlanType == 4)
                            {
                                int i = 0;
                                foreach (var det in job.JobsDetails)
                                {
                                    if (i > 3)// for next retailer
                                    {
                                        i = 0;
                                    }
                                    if (det.RetailerID == model.RetailerID)
                                    {
                                        if (i == 0)
                                        {
                                            det.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11));
                                        }
                                        else if (i == 1)
                                        {
                                            det.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11)).AddDays(7);
                                        }
                                        else if (i == 2)
                                        {
                                            det.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11)).AddDays(14);
                                        }
                                        else if (i == 3)
                                        {
                                            det.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11)).AddDays(21);
                                        }
                                        i++;
                                    }
                                }
                                db.SaveChanges();
                            }
                            else if (model.VisitPlanType == 5)
                            {
                                int i = 0;
                                foreach (var det in job.JobsDetails)
                                {
                                    if (det.RetailerID == model.RetailerID)
                                    {
                                        if (i == 0)
                                        {
                                            det.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11));
                                        }
                                        else if (i == 1)
                                        {
                                            det.JobDate = DateTime.Parse(model.NewDateStr.Substring(4, 11)).AddDays(14);
                                        }
                                        i++;
                                    }
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "500",
                            message = "Something went wrong"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "UpdateSOJob API Failed of calendar view");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Something went wrong, please consult administrator"
                    }
                });
            }

            return Ok(new
            {
                status = new CheckInLatLongResp
                {
                    code = "200",
                    message = "Job updated successfully"
                }
            });
        }

        [HttpPost]
        public IHttpActionResult DeleteSinglePlan(JobDeleteForCalander model)
        {
            int rowcount = 0;
            try
            {
                string qry = "delete from jobs where status = 0 and id  = " + model.JobID;

                if (model.VisitPlanType == 4 || model.VisitPlanType == 5)
                {
                    var job = db.Jobs.Where(p => p.ID == model.JobID).FirstOrDefault();

                    if (model.VisitPlanType == 4 && job.JobsDetails.Count > 4)
                    {
                        qry = "delete from jobsdetail where jobid = " + model.JobID +
                                " and retailerid = " + model.RetailerID +
                                " and status = 0 ";
                    }

                    if (model.VisitPlanType == 5 && job.JobsDetails.Count > 2)
                    {
                        qry = "delete from jobsdetail where jobid = " + model.JobID +
                                " and retailerid = " + model.RetailerID +
                                " and status = 0 ";
                    }
                }
                else if (model.VisitPlanType == 1 || model.VisitPlanType == 6)
                {
                    var job = db.Jobs.Where(p => p.ID == model.JobID).FirstOrDefault();

                    if (job.JobsDetails.Count > 1)
                    {
                        qry = "delete from jobsdetail where status = 0 and id  = " + model.JobDetailID;
                    }
                }
                rowcount = db.Database.ExecuteSqlCommand(qry);
            }
            catch
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "500",
                        message = "Something went wrong"
                    }
                });

            }

            string msg = "Job is deleted successfully";

            if (model.VisitPlanType == 4 || model.VisitPlanType == 5)
            {
                if (rowcount == 0)
                {
                    msg = "This beat plan's job is already visited, so couldn't delete this beat";
                }
                else
                {
                    msg = "Job's beat deleted successfully";
                }
            }
            else
            {
                if(rowcount == 0)
                {
                    msg = "Something went wrong or either job is already visited";
                }
            }

            return Ok(new
            {
                status = new CheckInLatLongResp
                {
                    code = "200",
                    message = msg
                }
            });
        }

        [HttpPost]
        public IHttpActionResult DeletePlans(JobDeleteForCalander model)
        {
            try
            {
                DateTime fromDate = DateTime.Parse("01 " + model.Month);
                var lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
                DateTime toDate = DateTime.Parse(lastDay + " " + model.Month + " 23:59:59");
                string visitPlanTypes = "1";
                if (model.Beat != null && model.Beat.Equals("1"))
                {
                    visitPlanTypes = "4,5,6";
                }

                string qry = "delete from jobsdetail where VisitPlanType in (" + visitPlanTypes + ") and status = 0 "+
                    " and SalesOficerID = " + model.SOID + " and " +
                    " jobdate >= '" + fromDate + "' and jobdate <= '" + toDate +
                    "' ";
                
                db.Database.ExecuteSqlCommand(qry);

            }
            catch
            {
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "500",
                        message = "Something went wrong"
                    }
                });

            }

            string msg = "Jobs monthly plan deleted successfully";
            if (model.Beat != null && model.Beat.Equals("1"))
            {
                msg = "Jobs beat plan deleted successfully";
            }
            return Ok(new
            {
                status = new CheckInLatLongResp
                {
                    code = "200",
                    message = msg
                }
            });
        }

        #endregion
    }

    public class JobUpdateForCalander
    {
        public int JobID { get; set; }
        public int JobDetailID { get; set; }
        public int RetailerID { get; set; }
        public int VisitPlanType { get; set; }
        public string NewDateStr { get; set; }
    }
    public class JobDeleteForCalander
    {
        public int JobID { get; set; }
        public int JobDetailID { get; set; }
        public int SOID { get; set; }
        public string Month { get; set; }
        public string Beat { get; set; }
        public int RetailerID { get; set; }
        public int VisitPlanType { get; set; }
                        
    }
    public class CheckInLatLongReq
    {
        public int RetailerId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Location { get; set; }
    }

    public class CheckInLatLongResp
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class VisitsInfo
    {
       
        public DateTime VisitDate { get; set; }
        public int TradeVisits { get; set; }
        public int HousingVisits { get; set; }
        public int CorporateVisits { get; set; }
        public int AllPurposeVisits { get; set; }
    }

    public class RetailerCounts
    {
        
        public DateTime VisitDate { get; set; }
        public int TradeCounts{ get; set; }
        public int HousingCounts { get; set; }
        public int CorporateCounts { get; set; }
        public int AllPurposeCounts { get; set; }
    }

    public class SpReturn
    {
        public int id { get; set; }
        public decimal distance { get; set; }
    }

    public class JobMarketInfoData
    {
        public int JobsDetailID { get; set; }
        public bool SPOSMaterialAvailable { get; set; }
        public bool SUblAcctOpened { get; set; }
        public bool SBrochureAvailable { get; set; }
        public bool SSmsCardAvailable { get; set; }
        public bool SShadeCardAvailable { get; set; }
        public bool SDisplay { get; set; }
        public bool SWhite40KgAvailable { get; set; }
        public string SMarketInfoNote { get; set; }
        public string Token { get; set; }
    }

}

