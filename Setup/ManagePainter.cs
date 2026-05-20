using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOS.Shared;
using FOS.DataLayer;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Globalization;
using Shared.Diagnostics.Logging;

namespace FOS.Setup
{
    public class ManagePainter
    {


        // Get All Retailer Painters Association List ...
        public static List<PainterAssociationData> GetAllPaintersRelatedToRetailersListForGrid(int RegionalHeadID, int SaleOfficerID)
        {
            List<PainterAssociationData> painterAssociationData = new List<PainterAssociationData>();
            
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    painterAssociationData = dbContext.RetailerPainters.Where(r => r.Retailer.SaleOfficer.RegionalHeadID == RegionalHeadID && r.Retailer.SaleOfficerID == SaleOfficerID)
                            .Select(
                                u => new PainterAssociationData
                                {
                                    City = dbContext.Cities.Where(c => c.ID == u.Retailer.CityID).Select(c => c.Name).FirstOrDefault(), 
                                    RetailerID = u.RetailerID,
                                    RetailerName = u.Retailer.Name,
                                    NoOfPainters = 0
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return painterAssociationData;
        }

        // Get All Retailer Painters Association List ...
        public static List<PainterCityNamesData> GetPainterCityNamesList()
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return null;
                    //return dbContext.vPainters.Where(p => p.City != null && p.City != "").GroupBy(p => p.City)
                    //         .Select(
                    //             u => new PainterCityNamesData
                    //             {
                    //                 ID = 1,
                    //                 CityName = u.Select(a => a.City).FirstOrDefault()
                    //             }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get Territory List ...
        public List<TerritoriesNameData> GetTerritoryNamesList()
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Regions.Where(p => p.Name != null && p.Name != "")//.GroupBy(p => p.City)
                             .Select(
                                 u => new TerritoriesNameData
                                 {
                                     ID = u.ID,
                                     TeritoryName = u.Name
                                 }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LovClass> getFosLov(int TID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var v = (from s in dbContext.SaleOfficers
                             join r in dbContext.RegionalHeads on s.RegionalHeadID equals r.ID
                             join g in dbContext.RegionalHeadRegions on r.ID equals g.RegionHeadID
                             join rg in dbContext.Regions on g.RegionID equals rg.ID
                             where (rg.ID == TID || TID == null)
                             select new LovClass
                             {
                                 ID=s.ID.ToString(),
                                 Name=s.Name,
                             }).ToList();

                    return v;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PainterCityNamesData> GetCityList(int TID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Cities.Where(p => p.RegionID == TID)
                             .Select(
                                 u => new PainterCityNamesData
                                 {
                                     ID = u.ID,
                                     CityName = u.Name
                                 }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PainterCityNamesData> GetAllCitiesList()
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Cities
                             .Select(
                                 u => new PainterCityNamesData
                                 {
                                     ID = u.ID,
                                     CityName = u.Name
                                 }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PainterCityNamesData> GetShopsList(int CID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Retailers.Where(p => p.CityID == CID)//.GroupBy(p => p.City)
                             .Select(
                                 u => new PainterCityNamesData
                                 {
                                     ID = u.ID,
                                     CityName = u.ShopName
                                 }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PainterCityNamesData> GetAreaList(int CID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Areas.Where(p => p.CityID == CID)//.GroupBy(p => p.City)
                             .Select(
                                 u => new PainterCityNamesData
                                 {
                                     ID = u.ID,
                                     CityName = u.Name
                                 }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PainterCityNamesData> GetMarketList(int CID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Retailers.Where(p => p.CityID == CID)//.GroupBy(p => p.City)
                             .Select(
                                 u => new PainterCityNamesData
                                 {
                                     ID = u.ID,
                                     CityName = u.Market
                                 }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Insert OR Update Retailer Painters Association ...
        public static int AddUpdatePaintersRelatedToRetailers(PainterAssociationData obj)
        {
            int boolFlag = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {

                        

                        if (obj.SelectedPainters == null)
                        {
                            boolFlag = 2;
                        }
                        else
                        {
                            if (obj.ID == 0)
                            {

                                RetailerPainter RetailerPaintersDetailObj = new RetailerPainter();
                                var SelectedPainters = dbContext.RetailerPainters.Where(r => r.RetailerID == obj.RetailerID).ToList();

                                foreach (var deletePainter in SelectedPainters)
                                {
                                    dbContext.RetailerPainters.Remove(deletePainter);
                                }

                                dbContext.SaveChanges();

                                if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                {

                                    String[] strPainterId = obj.SelectedPainters.Split(',');
                                    foreach (var painterid in strPainterId)
                                    {
                                        int intPainterID = Convert.ToInt32(painterid);
                                        //vPainter vPainter = new vPainter();
                                        //vPainter = dbContext.vPainters.Where(vp => vp.ID == intPainterID).FirstOrDefault();

                                        RetailerPainter RetailerPaintersObj = new RetailerPainter();
                                        RetailerPaintersObj.SaleOfficerID = obj.SaleOfficerID;
                                        RetailerPaintersObj.RetailerID = obj.RetailerID;
                                        RetailerPaintersObj.PainterID = intPainterID;
                                        //RetailerPaintersObj.PainterName = vPainter.Name;
                                        //RetailerPaintersObj.City = vPainter.City;
                                        //RetailerPaintersObj.Address = vPainter.Address;
                                        //RetailerPaintersObj.CNIC = vPainter.CNIC;
                                        //RetailerPaintersObj.Market = vPainter.Market;
                                        //RetailerPaintersObj.POS = vPainter.POS;
                                        //RetailerPaintersObj.AddedBy = 1;

                                        dbContext.RetailerPainters.Add(RetailerPaintersObj);
                                    }

                                }
                               

                            }
                            else
                            {
                                RetailerPainter RetailerPaintersDetailObj = new RetailerPainter();
                                var SelectedPainters = dbContext.RetailerPainters.Where(r => r.RetailerID == obj.RetailerID).ToList();

                                foreach (var deletePainter in SelectedPainters)
                                {
                                    dbContext.RetailerPainters.Remove(deletePainter);
                                }

                                dbContext.SaveChanges();
                                
                                if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                {

                                    String[] strPainterId = obj.SelectedPainters.Split(',');
                                    foreach (var painterid in strPainterId)
                                    {
                                        int intPainterID = Convert.ToInt32(painterid);
                                        //vPainter vPainter = new vPainter();
                                        //vPainter = dbContext.vPainters.Where(vp => vp.ID == intPainterID).FirstOrDefault();

                                        RetailerPainter RetailerPaintersObj = new RetailerPainter();
                                        RetailerPaintersObj.SaleOfficerID = obj.SaleOfficerID;
                                        RetailerPaintersObj.RetailerID = obj.RetailerID;
                                        RetailerPaintersObj.PainterID = intPainterID;
                                        //RetailerPaintersObj.PainterName = vPainter.Name;
                                        //RetailerPaintersObj.City = vPainter.City;
                                        //RetailerPaintersObj.Address = vPainter.Address;
                                        //RetailerPaintersObj.CNIC = vPainter.CNIC;
                                        //RetailerPaintersObj.Market = vPainter.Market;
                                        //RetailerPaintersObj.POS = vPainter.POS;
                                        RetailerPaintersObj.AddedBy = 1;

                                        dbContext.RetailerPainters.Add(RetailerPaintersObj);
                                    }

                                }

                            }
                            dbContext.SaveChanges();
                            boolFlag = 1;
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Retailer & Painter Association Is Failed");
                boolFlag = 0;
            }
            return boolFlag;
        }


        // Delete Retailer Painters Association ...
        public static int DeleteRetailerPaintersAssociation(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //RetailerPainter obj = dbContext.RetailerPainters.Where(u => u.ID == ID).FirstOrDefault();
                    //dbContext.RetailerPainters.Remove(obj);
                    //dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer Painters Association Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<PainterAssociationData> GetResult(string search, string sortOrder, int start, int length, List<PainterAssociationData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<PainterAssociationData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<PainterAssociationData> FilterResult(string search, List<PainterAssociationData> dtResult, List<string> columnFilters)
        {
            IQueryable<PainterAssociationData> results = dtResult.AsQueryable();

            results = null;

            return results;
        }

        // Get Painters Related To Retailer In Edit Mode ...
        public static List<PainterAssociationData> GetEditPainter(int RegionalHeadID, int SaleOfficerID, int RetailerID, string CityName)
        {
            List<PainterAssociationData> painterData = new List<PainterAssociationData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    //var so = dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).FirstOrDefault();
                    //var rt = dbContext.RetailerPaintersMasters.Where(j => j.ID != RetailerPaintersMasterID).SelectMany(j => j.RetailerPaintersDetails.Select(rp => rp.PainterID));

                    //painterData = dbContext.vPainters.Where(r => r.City == CityName)
                    //    .Select(
                    //         u => new PainterAssociationData
                    //         {
                    //             ID = u.ID,
                    //             PainterName = u.Name,
                    //             City  = u.City,
                    //             PainterAssociationStatus = dbContext.RetailerPainters.Where(r => r.PainterID == u.ID).Count() == 0 ? false : true
                    //         }).OrderBy(r => r.PainterAssociationStatus).ThenBy(x => x.PainterName).ToList();

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Edit Painter Failed");
                throw;
            }

            return painterData;
        }


    }
}
