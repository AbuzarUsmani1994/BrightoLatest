using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageDealer
    {

        // Get All Dealers List For Grid ...
        public static List<DealerData> PlannedDistributors(PlannedRetailerFilter model)
        {
            List<DealerData> dealerData = new List<DealerData>();
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                    dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false
                    && u.CityID == (model.CityID > 0 ? model.CityID : u.CityID)
                    && u.RegionalHeadID == (model.RegionalHeadID > 0 ? model.RegionalHeadID : u.RegionalHeadID)
                    && u.ID == (model.DealerID > 0 ? model.DealerID : u.ID)

                    )
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    CityID = u.CityID ?? 0,
                                    CityName = u.City.Name,
                                    Planned = u.Planned ?? false
                                }).OrderBy(a => a.CityName).ThenBy(aa => aa.Name).ToList();

                foreach (var deal in dealerData)
                {
                    var retailers = dbContext.Retailers.Where(r => r.IsActive == true 
                                    && r.IsDeleted == false
                                    && r.Status == true
                                    && r.DealerID == deal.ID).ToList();

                    if (retailers != null && retailers.Count > 0)
                    {
                        DateTime fromDate = DateTime.Parse("01 " + DateTime.Today.ToString("MMM yyy"));
                        var lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
                        DateTime toDate = DateTime.Parse(lastDay + " " + DateTime.Today.ToString("MMM yyy") + " 23:59:59");

                        if (!string.IsNullOrEmpty(model.month))
                        {
                            fromDate = DateTime.Parse("01 " + model.month + " " + DateTime.Today.Year);
                            lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
                            toDate = DateTime.Parse(lastDay + " " + model.month + " " + DateTime.Today.Year + " 23:59:59");
                        }

                        List<int> retIds = retailers.Select(r => r.ID).ToList();

                        var jobs = dbContext.JobsDetails.Where(j => 
                                    //j.Status == false && 
                                    j.Job.IsActive == true && j.Job.IsDeleted == false
                                    && j.JobDate >= fromDate 
                                    && j.JobDate <= toDate 
                                    && retIds.Contains(j.RetailerID.Value)).ToList();

                        List<int> plannedRetIds = new List<int>();

                        if (jobs != null && jobs.Count > 0)
                        {
                            foreach (var job in jobs)
                            {
                                if (!deal.RetailersPlanned.Any(p => p.ID == job.RetailerID))
                                {
                                    deal.RetailersPlanned.Add(new RetailerData
                                    {
                                        ID = job.RetailerID.Value,
                                        Name = job.Retailer.Name,
                                        ShopName = job.Retailer.ShopName
                                    });
                                }
                            }

                            plannedRetIds = jobs.Select(j => j.RetailerID.Value).ToList();
                        }

                        foreach (var ret in retailers)
                        {
                            if(!plannedRetIds.Contains(ret.ID))
                            {
                                deal.RetailersUnplanned.Add(new RetailerData
                                {
                                    ID = ret.ID,
                                    Name = ret.Name,
                                    ShopName = ret.ShopName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "PlannedDistributors List Failed");
                throw;
            }

            return dealerData;
        }

        // Get All Dealers List For Grid ...
        public static List<DealerData> GetAllDealersList()
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }
        public static List<DealerData> GetAllDealersListShops()
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.ShopName
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }

        public static List<DealerData> GetAllDealersByRegionAndCity(int? RegionID,int? CityID)
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (RegionID > 0 && CityID > 0)
                    {
                        dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false && u.RegionID == RegionID && u.CityID == CityID && u.IsActive == true)
                             .Select(
                                 u => new DealerData
                                 {
                                     ID = u.ID,
                                     Name = u.ShopName
                                 }).ToList();
                        dealerData.Insert(0, new DealerData
                        {
                            ID = 0,
                            Name = "All"
                        });
                    }
                    else if (RegionID > 0 && CityID==0)
                    {
                        dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false && u.RegionID == RegionID &&  u.IsActive == true)
                           .Select(
                               u => new DealerData
                               {
                                   ID = u.ID,
                                   Name = u.ShopName
                               }).ToList();
                        dealerData.Insert(0, new DealerData
                        {
                            ID = 0,
                            Name = "All"
                        });
                    }
                  
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }
        //  Get All Dealers List For Grid...
        public static List<DealerData> GetDealerList()
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                   // DealerCode = u.DealerCode,
                                    RegionalHeadID = (int)u.RegionalHeadID,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Address = u.Address,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2
                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }

        public static List<RetailerData> GetDealersForExportinExcel(int ID,int RegionID)
        {
            List<RetailerData> RetailerObj = new List<RetailerData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerObj = dbContext.Dealers.OrderBy(u => u.Name).Where(u => u.IsActive == true && u.IsDeleted == false && u.RegionID==RegionID)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    // DealerID = u.DealerID,
                                    DealerName = u.Name,
                                    SaleOfficerID = u.SaleOfficerID,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    RegionID = u.RegionID,
                                   RegionName = dbContext.Regions.Where(x=>x.ID==u.RegionID).Select(x=>x.Name).FirstOrDefault(),
                                    CityID = u.SaleOfficer.CityID,
                                    CItyName = u.City.Name,
                                  ////  AreaID = u.Area == null ? 0 : u.AreaID.Value,
                                   AreaName = u.NewArea,
                                    Address = u.Address == null ? "" : u.Address,
                                    Name = u.Name,
                                    RetailerCode = u.DealerCode,
                                    CNIC = u.CNIC,
                                   // AccountNo = u.AccountNo,
                                    Email = u.Email,
                                  //  AccountNo2 = u.AccountNo2,
                                    //BankName2 = u.BankName2,
                                    //CardNumber = u.CardNumber,
                                    ShopName = u.ShopName,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    Location = u.Location,
                                    //LocationName = u.LocationName,
                                    //LocationMargin = u.LocationMargin,
                                    //Type = u.Type,
                                    IsActive = (bool)u.IsActive,
                                    IsDeleted = (bool)u.IsDeleted,
                                    LastUpdate = (DateTime)u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "EXPORT Reatailer In Excel Not Work");
                throw;
            }

            return RetailerObj;
        }


        public static List<RetailerData> GetSegmentType()
        {
            List<RetailerData> RetailerObj = new List<RetailerData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerObj = dbContext.Tbl_Segmenttype.OrderBy(u => u.Name).Where(u => u.IsActive == true)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    // DealerID = u.DealerID,
                                    DealerName = u.Name,
                                    
                                    //
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "EXPORT Reatailer In Excel Not Work");
                throw;
            }

            return RetailerObj;
        }


        public static List<RetailerData> GetReportType()
        {
            List<RetailerData> RetailerObj = new List<RetailerData>();
            try
            {
                RetailerObj.Add(new RetailerData
                {
                    ID = 1,
                    DealerName = "Summary"
                });

                RetailerObj.Add(new RetailerData
                {
                    ID = 2,
                    DealerName = "Detail"
                });
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "EXPORT Reatailer In Excel Not Work");
                throw;
            }

            return RetailerObj;
        }

        //  Get Single Dealer By DealerID...
        public static DealerData GetDealerByDealerID(int intDealerID)
        {
            DealerData dealerData = new DealerData();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.IsDeleted == false && u.ID == intDealerID)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    DealerCode = u.DealerCode,
                                    RegionalHeadID = (int)u.RegionalHeadID,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Address = u.Address,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2
                                }).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dealerData;
        }


        // Get Dealers List By SalesOfficerID ...
        public static List<DealerData> GetDealerListBySaleOfficerID(int SaleOfficerID)
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.RegionalHead.SaleOfficers.Where(s =>s.ID==SaleOfficerID).Count()>0 && u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    DealerCode = u.DealerCode,
                                    SaleOfficerName = u.RegionalHead.SaleOfficers.Select(s =>s.ID).FirstOrDefault() == null ? "" : u.RegionalHead.SaleOfficers.Select(s =>s.Name).FirstOrDefault(),
                                    SaleOfficerID = u.RegionalHead.SaleOfficers.Select(s => s.ID).FirstOrDefault() == null ? 0 : u.RegionalHead.SaleOfficers.Select(s => s.ID).FirstOrDefault(),
                                    RegionalHeadID = (int)u.RegionalHeadID,
                                    RegionalHeadName = u.RegionalHead.Name == null ? "" : u.RegionalHead.Name,
                                    Address = u.Address,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dealerData;
        }


        // Insert OR Update Dealer ...
        public static int AddUpdateDealer(DealerData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Dealer dealerObj = new Dealer();
                        if (obj.ID == 0)
                        {
                            dealerObj.ID = dbContext.Dealers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            dealerObj.Name = obj.Name;
                            dealerObj.DealerCode = obj.DealerCode;
                            dealerObj.RegionalHeadID = obj.HiddenRegionalHeadID;
                            dealerObj.CityID = obj.CityID;
                            dealerObj.AreaID = obj.AreaID;
                            dealerObj.Address = obj.Address;
                            dealerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            dealerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                            dealerObj.IsActive = true;
                            dealerObj.CreatedDate = DateTime.Now;
                            dealerObj.LastUpdate = DateTime.Now;

                            if(!string.IsNullOrEmpty(obj.BirthdayString))
                            {
                                dealerObj.Birthday = DateTime.Parse(obj.BirthdayString);
                            }
                            //Created By Work Pending...
                            dealerObj.CreatedBy = 1;

                            dbContext.Dealers.Add(dealerObj);
                        }
                        else
                        {
                            dealerObj = dbContext.Dealers.Where(u => u.ID == obj.ID).FirstOrDefault();
                            dealerObj.Name = obj.Name;
                            dealerObj.DealerCode = obj.DealerCode;
                            dealerObj.RegionalHeadID = obj.HiddenRegionalHeadID;
                            dealerObj.CityID = obj.CityID;
                            dealerObj.AreaID = obj.AreaID;
                            dealerObj.Address = obj.Address;
                            dealerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            dealerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                            dealerObj.LastUpdate = DateTime.Now;
                            if (!string.IsNullOrEmpty(obj.BirthdayString))
                            {
                                dealerObj.Birthday = DateTime.Parse(obj.BirthdayString);
                            }
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Dealer Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Dealer"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static int AddDealerCityAreas(int DealerID, int CityID, int AreaID, int userId)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        DealerCityArea obj = new DealerCityArea();
                        obj.DealerID = DealerID;
                        obj.CityID = CityID;
                        obj.AreaID = AreaID;
                        obj.CreatedBy = userId;
                        obj.CreatedOn = DateTime.Now;

                        dbContext.DealerCityAreas.Add(obj);
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "AddDealerCityAreas Failed");
                Res = 0;
                return Res;
            }
            return Res;
        }
        public static int DeleteDealerCityAreas(int DealerID, int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var dealerCityAreas = dbContext.DealerCityAreas.Where(u => u.DealerID == DealerID
                    && u.CityID == (CityID > 0 ? CityID : u.CityID)).ToList();
                    foreach (var obj in dealerCityAreas)
                    {
                        dbContext.DealerCityAreas.Remove(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "DeleteDealerCityAreas Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int DeleteDealerCityAreasCitywise(int DealerID, int cityId)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var dealerCityAreas = dbContext.DealerCityAreas.Where(u => u.DealerID == DealerID && u.CityID == cityId).ToList();
                    foreach (var obj in dealerCityAreas)
                    {
                        dbContext.DealerCityAreas.Remove(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "DeleteDealerCityAreasCitywise Failed");
                Resp = 1;
            }
            return Resp;
        }
        // Delete Dealer ...
        public static int DeleteDealer(int DealerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Dealer obj = dbContext.Dealers.Where(u => u.ID == DealerID).FirstOrDefault();
                    dbContext.Dealers.Remove(obj);
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Dealer Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Dealers List For Grid ...
        public static List<DealerData> GetDealerListForGrid(int RegionalHeadID)
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    CityID = (int)u.CityID,
                                    AreaID = (int)u.AreaID,
                                    DealerCode = u.DealerCode,
                                    RegionalHeadID = (int)u.RegionalHeadID,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Address = u.Address,
                                    Phone1 = u.Phone1,
                                    Phone2 = u.Phone2,
                                    BirthdayString = u.Birthday.HasValue ? u.Birthday.Value.ToString("dd-MMM-yyyy") : ""
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }


        public static List<DealerData> GetResult(string search, string sortOrder, int start, int length, List<DealerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<DealerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<DealerData> FilterResult(string search, List<DealerData> dtResult, List<string> columnFilters)
        {
            IQueryable<DealerData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower()) || p.Address != null && p.Address.ToLower().Contains(search.ToLower())
                || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.Address != null && p.Address.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[6].ToLower())))
                && (columnFilters[7] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[7].ToLower())))
                );

            return results;
        }



        public static int UpdatePlannedUnplanned(int DealerID)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Dealer dealerObj = new Dealer();
                        dealerObj = dbContext.Dealers.Where(u => u.ID == DealerID).FirstOrDefault();

                        if (dealerObj.Planned.HasValue && dealerObj.Planned.Value)
                        {
                            dealerObj.Planned = false;
                        }
                        else
                        {
                            dealerObj.Planned = true;
                        }
                        
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "UpdatePlannedUnplanned Failed");
                Res = 0;
            }

            return Res;
        }

        // Get All Dealers List For Grid ...
        public static List<DealerData> GetAllDealersListRelatedToRegionalHead(int RegionalHeadID, bool selectOption = false, string selectText = "Select Any")
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.RegionID == RegionalHeadID && u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.ShopName,
                                }).OrderBy(x => x.Name).ToList();

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            if (selectOption)
            {
                dealerData.Insert(0, new DealerData
                {
                    ID = 0,
                    Name = selectText
                });
            }
            return dealerData;
        }

        public static List<DealerData> GetDistributors(int RegionalHeadID,int ID)
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.RegionID == RegionalHeadID && u.RangeID==ID  && u.IsActive==true)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.ShopName,
                                }).OrderBy(x => x.Name).ToList();

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }

        public static List<DealerData> GetDistributorsForRetailerTransfer(int RegionalHeadID,int rangeID)
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.RegionID == RegionalHeadID && u.RangeID==rangeID && u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.ShopName,
                                }).OrderBy(x => x.Name).ToList();

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            return dealerData;
        }
        public static List<DealerCityData> GetDealerCityAndAreas(int dealerId, int cityId)
        {
            var dealerCitiesList = new List<DealerCityData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerCitiesList = dbContext.DealerCityAreas.Where(u => u.DealerID == dealerId && u.CityID == (cityId > 0 ? cityId : u.CityID))
                            .Select(
                                u => new DealerCityData
                                {
                                    ID = u.ID,
                                    DealerID = u.DealerID,
                                    CityID = u.CityID,
                                    AreaID = u.AreaID,
                                    CityName = u.City.Name,
                                    AreaName = u.Area.Name

                                }).OrderBy(x => x.CityName).ThenBy(k => k.AreaName).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetDealerCityAndAreas List Failed");
                throw;
            }

            return dealerCitiesList;
        }

        public static List<DealerCityData> GetDealerCityAndAreasCitywise(int dealerId)
        {
            var dealerCitiesList = new List<DealerCityData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerCitiesList = dbContext.DealerCityAreas.Where(u => u.DealerID == dealerId)
                            .Select(
                                u => new DealerCityData
                                {
                                    ID = u.ID,
                                    DealerID = u.DealerID,
                                    CityID = u.CityID,
                                    AreaID = u.AreaID,
                                    CityName = u.City.Name,
                                    AreaName = u.Area.Name

                                }).OrderBy(x => x.CityName).ThenBy(k => k.AreaName).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetDealerCityAndAreasCitywise List Failed");
                throw;
            }

            return dealerCitiesList;
        }

        public static List<DealerData> GetAllDealersListRelatedToRegionalHeadSelectOption(int RegionalHeadID, bool selectOption = false, string selectText = "Select Any")
        {
            List<DealerData> dealerData = new List<DealerData>();
            
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(u => u.RegionalHeadID == (RegionalHeadID > 0 ? RegionalHeadID : u.RegionalHeadID) && u.IsDeleted == false)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Dealer List Failed");
                throw;
            }

            if (selectOption)
            {
                dealerData.Insert(0, new DealerData
                {
                    ID = 0,
                    Name = selectText
                });
            }
            return dealerData;
        }

        public static List<RetailerData> GetDistributorForGrid(int UserID)
        {
            List<RetailerData> RetailerData = new List<RetailerData>();
            RetailerData data;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())


                {
                    
                        RetailerData = dbContext.Dealers.OrderByDescending(r => r.ID).Where( u=>u.IsDeleted == false && u.RegionID==UserID )
                                .ToList().Select(
                                    u => new RetailerData
                                    {
                                        ID = u.ID,

                                        RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                        SaleOfficerID = u.SaleOfficerID,
                                        SaleOfficerName = u.SaleOfficer.Name,
                                    // RetailerType = u.RetailerType,
                                    CityID = u.CityID,
                                        CItyName = u.City.Name,
                                        AreaID = (int)u.AreaID,
                                        RegionID = (int)u.RegionID,
                                        RegionName = u.Region.Name,
                                        Address = u.Address == null ? "" : u.Address,
                                        Name = u.Name,
                                    // RetailerCode = u.RetailerCode,
                                    //  CNIC = u.CNIC,
                                    RangeName = dbContext.MainCategories.Where(x => x.MainCategID == u.RangeID).Select(x => x.MainCategDesc).FirstOrDefault(),
                                        IsActiveYes = u.IsActive == true ? "Yes" : "No",
                                        ShopName = u.ShopName,
                                        Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                        Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                        Location = u.Latitude + "," + u.Longitude,
                                        LocationName = u.Location,
                                    // TypeOfShop = u.TypeOfShop,
                                    //ShopCategory = u.ShopCategory,
                                    //LocationMargin = u.LocationMargin,
                                }).ToList();
                  
                   
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Retailer List Failed");
                throw;
            }

            return RetailerData;
        }


        public static int AddUpdateDistributor(RetailerData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Dealer retailerObj = new Dealer();

                        if (obj.ID == 0)
                        {
                            retailerObj.ID = dbContext.Dealers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                            retailerObj.Name = obj.Name;
                           // retailerObj.RetailerCode = obj.RetailerCode;
                            retailerObj.CNIC = obj.CNIC;
                            retailerObj.Email = obj.Email;
                            retailerObj.RegionID = obj.RegionID;
                            retailerObj.ZoneID = 1;
                            retailerObj.SaleOfficerID =1;
                            retailerObj.NewArea = obj.AreaName;
                            retailerObj.CityID = obj.CityID;
                            retailerObj.AreaID = 1;
                            retailerObj.RangeID = 6;
                            retailerObj.DealerCode = obj.RetailerCode;
                            retailerObj.ShopName = obj.ShopName;
                            retailerObj.Location = obj.Location;
                            retailerObj.Address = obj.Address;
                         //   retailerObj.RetailerType = obj.RetailerType;
                            retailerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            retailerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                          //  retailerObj.LandLineNo = obj.ContactPersonCell;
                            //retailerObj.Market = obj.Market;
                            retailerObj.IsActive = true;
                        //    retailerObj.Status = true;
                            retailerObj.LastUpdate = DateTime.Now;
                            retailerObj.CreatedDate = DateTime.Now;
                            retailerObj.CreatedBy = obj.CreatedBy;
                            //retailerObj.Type = "WALLCOAT";
                         //   retailerObj.Source = (int)RetSourceEnum.Web;
                            dbContext.Dealers.Add(retailerObj);

                        }
                        else
                        {
                            retailerObj = dbContext.Dealers.Where(u => u.ID == obj.ID).FirstOrDefault();

                            retailerObj.Name = obj.Name;
                         //   retailerObj.RetailerCode = obj.RetailerCode;
                            retailerObj.CNIC = obj.CNIC;
                            retailerObj.Email = obj.Email;
                            retailerObj.NewArea = obj.AreaName;
                            retailerObj.SaleOfficerID = 1;
                            retailerObj.RangeID = 6;
                            retailerObj.CityID = obj.CityID;
                            retailerObj.AreaID = 1;
                            retailerObj.RegionID = obj.RegionID;
                            retailerObj.ZoneID = 1;
                            retailerObj.ShopName = obj.ShopName;
                            retailerObj.DealerCode = obj.RetailerCode;
                            retailerObj.Location = obj.Location;
                            retailerObj.Address = obj.Address;
                         //   retailerObj.RetailerType = obj.RetailerType;
                            retailerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            retailerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                           // retailerObj.LandLineNo = obj.ContactPersonCell;
                            //retailerObj.Market = obj.Market;
                            retailerObj.IsActive = obj.IsActive;
                           // retailerObj.Status = true;
                            retailerObj.LastUpdate = DateTime.Now;
                            retailerObj.CreatedBy = obj.CreatedBy;
                            //retailerObj.Type = "WALLCOAT";
                            //retailerObj.Source = (int)RetSourceEnum.Web;
                            // retailerObj. = obj.UpdatedBy;

                            //var objHeadRegion = dbContext.DealerRanges.Where(c => c.DealerID == obj.ID);

                            //if (objHeadRegion != null)
                            //{

                            //    foreach (var HeadRegion in objHeadRegion)
                            //    {
                            //        dbContext.DealerRanges.Remove(HeadRegion);
                            //    }
                            //}

                            //DealerRanx regionalHeadRegionObj;
                            //String[] strRegionId = obj.RangeID.Split(',');

                            //foreach (var regionid in strRegionId)
                            //{
                            //    regionalHeadRegionObj = new DealerRanx();
                            //    regionalHeadRegionObj.DealerID = retailerObj.ID;
                            //    regionalHeadRegionObj.RangeID = Convert.ToInt32(regionid);
                            //    regionalHeadRegionObj.IsActive = true;
                            //    regionalHeadRegionObj.RegionID = obj.RegionID;
                            //    regionalHeadRegionObj.CityID = obj.CityID;
                            //    regionalHeadRegionObj.AreaID = 1;

                            //    dbContext.DealerRanges.Add(regionalHeadRegionObj);
                            //}
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Customer Failed");
                if (exp.InnerException.InnerException.Message.Contains("CNIC"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 3;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("AccountNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 4;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("CardNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 5;
                    return Res;
                }
                Res = 0;
            }
            return Res;
        }



        //public static string AddDistributorDispatchNote(RetailerData obj,string sdate,int? DealerID)
        //{
            
        //    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(sdate) ? DateTime.Now.ToString() : sdate);
        //    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(sdate) ? DateTime.Now.ToString() : sdate);
        //    DateTime final = end.AddDays(1);
        //    string Res = "0";

        //    try
        //    {
        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            using (FOSDataModel dbContext = new FOSDataModel())
        //            {
        //                DispatchInVan van = new DispatchInVan();

        //                if (obj.ID == 0)
        //                {
        //                    DispatchInVanMaster master = new DispatchInVanMaster();
        //                    var SOName = "";
        //                    var DBOY = "";

        //                    String[] strRegionId = obj.CityIDs.Split(',');

        //                    foreach (var val in strRegionId)
        //                    {
        //                        SOName += "/" + dbContext.SaleOfficers.Where(x => x.ID.ToString() == val).Select(x => x.Name).FirstOrDefault();
        //                    }
        //                    DBOY = dbContext.DelieveryBoys.Where(x => x.ID == obj.RegionID).Select(x => x.Name).FirstOrDefault();

        //                    master.DelieveryboyName = DBOY;
        //                    master.SoNames = SOName;
        //                    master.DealerID = DealerID;
        //                    dbContext.DispatchInVanMasters.Add(master);
        //                    dbContext.SaveChanges();



        //                    foreach (var regionid in strRegionId)
        //                        {
        //                        var retailers =  dbContext.DealerDSRDispatches.Where(r=>r.SOID.ToString() == regionid && r.FromDate >= start && r.ToDate <= final).Select(u => new Items
        //                        {
        //                            ItemID = (int)u.ItemID,
        //                            ItemName = u.ItemName,
        //                            JobID = u.JobID,
        //                            DispatchQuantity = u.DispatchQuantity,
        //                            Createddate = u.Createddate,
        //                            DateFromInv=u.FromDate,
        //                            DateToInv=u.ToDate

        //                        }).ToList();

        //                        foreach (var item in retailers)
        //                        {
        //                            var data = dbContext.DispatchInVans.Where(x => x.JobID == item.JobID && x.ItemID == item.ItemID).ToList();
        //                            if (data.Count == 0)
        //                            {
        //                                van.SOID = Convert.ToInt32(regionid);
        //                                van.ItemID = item.ItemID;
        //                                van.ItemName = item.ItemName;
        //                                van.JobID = item.JobID;
        //                                //van.RetailerID = dbContext.JobsDetails.Where(x => x.JobID == item.JobID).Select(x => x.RetailerID).FirstOrDefault();
        //                                van.DispatchQuantityinVan = item.DispatchQuantity;
        //                                van.CreatedOn = true;
        //                                van.InvoicedDate = item.Createddate;
        //                                van.DelieveryboyID = obj.RegionID;
        //                                van.DispatchDate = DateTime.UtcNow.AddHours(5);
        //                                van.FromDateOnInvoiced = item.DateFromInv;
        //                                van.ToDateOnInvoiced = item.DateToInv;
        //                                van.DispatchInVanMasterID = master.ID;
        //                                dbContext.DispatchInVans.Add(van);
                                     

        //                                var changeStatus = dbContext.JobsDetails.Where(x => x.JobID == item.JobID).FirstOrDefault();
        //                                if (changeStatus.Dispatchstatus == "Dispatched")
        //                                {

        //                                }
        //                                else
        //                                {
        //                                    changeStatus.Dispatchstatus = "Dispatched";
                                            
        //                                }
        //                                dbContext.SaveChanges();
        //                            }

        //                        }
                             


        //                    }

                         


        //                }


        //                dbContext.SaveChanges();
        //                Res = "1";
        //                scope.Complete();
        //            }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Log.Instance.Error(exp, "Add Customer Failed");
        //        if (exp.InnerException.InnerException.Message.Contains("CNIC"))
        //        {
        //            // Res = 2 Is For Unique Constraint Error...
        //            Res = "3";
        //            return Res;
        //        }

        //        if (exp.InnerException.InnerException.Message.Contains("AccountNo"))
        //        {
        //            // Res = 2 Is For Unique Constraint Error...
        //            Res = "4";
        //            return Res;
        //        }

        //        if (exp.InnerException.InnerException.Message.Contains("CardNo"))
        //        {
        //            // Res = 2 Is For Unique Constraint Error...
        //            Res = "5";
        //            return Res;
        //        }
        //        Res = "0";
        //    }
        //    return Res;
        //}



        public static int AddUpdateDistributorTransfer(RetailerData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Dealer retailerObj = new Dealer();

                        if (obj.ID == 0)
                        {
                            if (obj.TransferRange == 6)
                            {

                                String[] strRegionId = obj.CityIDs.Split(',');

                                foreach (var regionid in strRegionId)
                                {

                                    var retailers = dbContext.Retailers.Where(x => x.CityID.ToString() == regionid && x.RangeADealer==obj.TransferFrom && x.RangeID==6).ToList();

                                    foreach (var item in retailers)
                                    {
                                        item.DealerID = obj.TransferTo;
                                        item.RangeADealer = obj.TransferTo;
                                        dbContext.SaveChanges();

                                    }


                                  
                                }
                            }
                            else if (obj.TransferRange == 7)
                            {
                                String[] strRegionId = obj.CityIDs.Split(',');

                                foreach (var regionid in strRegionId)
                                {

                                    var retailers = dbContext.Retailers.Where(x => x.CityID.ToString() == regionid && x.RangeBDealer == obj.TransferFrom && x.RangeID==7).ToList();

                                    foreach (var item in retailers)
                                    {
                                      
                                        item.RangeBDealer = obj.TransferTo;
                                        dbContext.SaveChanges();

                                    }



                                }

                            }
                            else 
                            {
                                String[] strRegionId = obj.CityIDs.Split(',');

                                foreach (var regionid in strRegionId)
                                {

                                    var retailers = dbContext.Retailers.Where(x => x.CityID.ToString() == regionid && x.RangeCDealer == obj.TransferFrom && x.RangeID == 10).ToList();

                                    foreach (var item in retailers)
                                    {

                                        item.RangeCDealer = obj.TransferTo;
                                        dbContext.SaveChanges();

                                    }



                                }

                            }
                        }
                     

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Customer Failed");
                if (exp.InnerException.InnerException.Message.Contains("CNIC"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 3;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("AccountNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 4;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("CardNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 5;
                    return Res;
                }
                Res = 0;
            }
            return Res;
        }
        public static int AddUpdateRETAILERTransfer(RetailerData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Dealer retailerObj = new Dealer();

                        if (obj.ID == 0)
                        {
                            
                            if (obj.TransferToRange == 7)
                            {
                               
                                    var retailers = dbContext.Retailers.Where(x => x.RegionID == obj.RegionID  && x.RangeADealer == obj.TransferFrom && x.RangeID == 7).ToList();

                                    foreach (var item in retailers)
                                    {

                                        item.RangeBDealer = obj.TransferTo;
                                        

                                    }
                                dbContext.SaveChanges();
                            }


                            else if (obj.TransferToRange == 10)
                            {
                                
                                    var retailers = dbContext.Retailers.Where(x => x.RegionID == obj.RegionID  && x.RangeADealer == obj.TransferFrom && x.RangeID == 10).ToList();

                                    foreach (var item in retailers)
                                    {

                                        item.RangeCDealer = obj.TransferTo;
                                        

                                    }
                                dbContext.SaveChanges();
                            }
                            else
                            {

                                var retailers = dbContext.Retailers.Where(x => x.RegionID == obj.RegionID && x.RangeADealer == obj.TransferFrom && x.RangeID == 6).ToList();

                                foreach (var item in retailers)
                                {

                                    item.RangeADealer = obj.TransferTo;
                                    item.DealerID = obj.TransferTo;

                                }
                                dbContext.SaveChanges();
                            }

                        }


                      
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Customer Failed");
                if (exp.InnerException.InnerException.Message.Contains("CNIC"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 3;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("AccountNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 4;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("CardNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 5;
                    return Res;
                }
                Res = 0;
            }
            return Res;
        }

        public static RetailerData GetEditDealer(int RetailerID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Dealers.Where(u => u.ID == RetailerID ).Select(u => new RetailerData
                    {
                        ID = u.ID,
                        // DealerID = u.DealerID,
                        DealerName = dbContext.Dealers.Where(a => a.ID == u.ID).Select(a => a.Name).FirstOrDefault(),
                        RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                        SaleOfficerID = u.SaleOfficerID,
                        SaleOfficerName = u.SaleOfficer.Name,
                       RegionID = u.RegionID,
                        RegionName = dbContext.Regions.Where(x => x.ID == u.RegionID).Select(x => x.Name).FirstOrDefault(),
                        CityID = u.CityID,
                        CItyName = u.City.Name,
                        AreaID = (int)u.AreaID == null ? dbContext.Areas.Where(a => a.CityID == u.CityID).Select(a => a.ID).FirstOrDefault() : (int)u.AreaID,
                        Address = u.Address == null ? "" : u.Address,
                        Name = u.Name,
                      //  RetailerCode = u.RetailerCode,
                        CNIC = u.CNIC,
                       // AccountNo = u.AccountNo,
                        Email = u.Email,
                        RangeIDD = (int)u.RangeID,
                      //  BankName2 = u.BankName2,
                     //   CardNumber = u.CardNumber,
                        ShopName = u.ShopName,
                        RetailerCode=u.DealerCode,
                        
                        Phone1 = u.Phone1 == null ? "" : u.Phone1,
                        Phone2 = u.Phone2 == null ? "" : u.Phone2,
                    IsActive = u.IsActive,
                     //   Market = u.Market,
                        Location = u.Location,
                     //   LocationName = u.LocationName,
                      //  LocationMargin = u.LocationMargin,
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }





    }
}