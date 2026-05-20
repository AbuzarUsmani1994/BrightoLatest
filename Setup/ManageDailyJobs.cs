using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FOS.Setup
{
    public class ManageDailyJobs
    {

        // Insert OR Update Jobs ...
        public static int AddUpdateDailyJob(JobsData obj)
        {
            int boolFlag = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Job JobObj = new Job();


                        #region VisitType "SHOP" & RetailerType "REGULAR OR FOCUSED"

                        if (obj.VisitType == "Shop" && (obj.RetailerType == "Both" || obj.RetailerType == "Focused" || obj.RetailerType == "Regular"))
                        {
                            if (obj.SelectedRetailers == null)
                            {
                                boolFlag = 2;
                            }
                            else
                            {
                                var dbSOObj = dbContext.SaleOfficers.Where(u => u.ID == obj.SaleOfficerID).FirstOrDefault();
                                if (obj.ID == 0)
                                {
                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = "";
                                    JobObj.RegionalHeadType = dbSOObj.RegionalHead.RegionalHeadsType.ID;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = dbSOObj.RegionalHeadID;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.VisitPlanType = 1;
                                    JobObj.JobType = "A";
                                    //JobObj.RetailerType = obj.RetailerType;

                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    JobObj.StartingDate = DateTime.Now;

                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedRetailers))
                                    {
                                        String[] JobsW = obj.SelectedRetailers.Split('|');

                                        foreach (var W in JobsW)
                                        {
                                            if (W == "")
                                            {
                                                continue;
                                            }
                                            String[] SelectedJob = W.Split(',');

                                            int O = 0;
                                            int RetailerID = 0;
                                            int DealerID = 0;
                                            string Date = "";

                                            for (int ii = 0; ii < SelectedJob.Length; ii++)
                                            {
                                                if (ii == 0)
                                                {
                                                    Date = SelectedJob[ii];
                                                }
                                                else if (ii == 1)
                                                {
                                                    RetailerID = Convert.ToInt32(SelectedJob[ii]);
                                                }
                                                else if (ii == 2)
                                                {
                                                    DealerID = Convert.ToInt32(SelectedJob[ii]);
                                                }
                                                else
                                                { }
                                            }

                                            JobsDetail jobDetail = new JobsDetail();
                                            Retailer ret = dbContext.Retailers.Where(r => r.ID == RetailerID).FirstOrDefault();
                                            jobDetail.JobID = JobObj.ID;
                                            jobDetail.DealerID = ret.DealerID;
                                            JobObj.RetailerType = ret.RetailerType;

                                            jobDetail.VisitPlanType = 1;

                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                            jobDetail.RetailerID = RetailerID;

                                            jobDetail.JobDate = Convert.ToDateTime(Date);
                                            jobDetail.Status = false;

                                            dbContext.JobsDetails.Add(jobDetail);
                                        }
                                        dbContext.Jobs.Add(JobObj);
                                    }
                                }
                                else
                                {


                                }
                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }

                        }

                        #endregion


                        #region VisitType "Painter"

                        else if (obj.VisitType == "Painter")
                        {

                            if (obj.SelectedPainters == null)
                            {
                                boolFlag = 3;
                            }
                            else
                            {
                                if (obj.ID == 0)
                                {
                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.RetailerType = "";
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 3;

                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;

                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                    {
                                        String[] JobsW = obj.SelectedPainters.Split('|');

                                        foreach (var W in JobsW)
                                        {
                                            if (W == "")
                                            {
                                                continue;
                                            }

                                            String[] SelectedJob = W.Split(',');

                                            int O = 0;
                                            int PainterID = 0;
                                            int DealerID = 0;
                                            string Date = "";

                                            for (int ii = 0; ii < SelectedJob.Length; ii++)
                                            {
                                                if (ii == 0)
                                                {
                                                    Date = SelectedJob[ii];
                                                }
                                                else if (ii == 1)
                                                {
                                                    PainterID = Convert.ToInt32(SelectedJob[ii]);
                                                }
                                                else if (ii == 2)
                                                {
                                                    DealerID = Convert.ToInt32(SelectedJob[ii]);
                                                }
                                                else
                                                { }
                                            }

                                            JobsDetail jobDetail = new JobsDetail();
                                            int intRetailerID = PainterID;
                                            jobDetail.JobID = JobObj.ID;
                                           // jobDetail.DealerID = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.DealerID).FirstOrDefault();
                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                            jobDetail.RetailerID = intRetailerID;
                                            JobObj.RetailerType = obj.RetailerType;
                                            jobDetail.JobDate = Convert.ToDateTime(Date);
                                            jobDetail.Status = false;
                                            dbContext.JobsDetails.Add(jobDetail);
                                        }
                                        dbContext.Jobs.Add(JobObj);
                                    }
                                }
                                else
                                { }
                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }

                        }

                        #endregion

                        else
                        {
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "AddUpdateDailyJob Failed");
                boolFlag = 0;
            }
            return boolFlag;
        }



    }
}
