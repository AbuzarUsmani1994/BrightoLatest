using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageZone
    {

        // Get All Areas List ...
        public static List<ZoneData> GetZoneList()
        {
            List<ZoneData> zone = new List<ZoneData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    zone = dbContext.Zones
                            .Select(
                                u => new ZoneData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return zone;
        }

        
        // Get All ZONES List By CityID ...
        public static List<ZoneData> GetZonesByCityID(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var CityZones = dbContext.CityZones.Where(a => a.CityID == CityID).Select(a => new ZoneData 
                    {
                        ID = a.ZoneID,
                        Name = a.Zone.Name,
                    }).ToList();

                    return CityZones;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


                // Get All Areas By CityID In Edit Mode ...
        public static List<Area> GetAreaListByCityIDJobEdit(int JobID, int SOID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<Area> areaData = new List<Area>();

                    var SO = dbContext.SaleOfficers.Where(s => s.ID == SOID).FirstOrDefault();

                    var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    var ChooseAreas = dbContext.Cities.Where(a => a.ID == CityID).SelectMany(a => a.Areas);

                    var AreasIDs = dbContext.Jobs.Where(j => j.ID == JobID && j.SaleOfficerID == SOID && j.CityID == CityID && j.IsDeleted == false).Select(j => j.Areas).FirstOrDefault();

                    if (AreasIDs != null)
                    {
                        var query = from val in AreasIDs.Split(',')
                                    select int.Parse(val);

                        areaData = dbContext.Areas.Where(a => a.CityID == CityID && query.Contains(a.ID)).ToList();
                    }
                    else
                    {
                        areaData = dbContext.Areas.Where(a => a.CityID == CityID).ToList();
                    }


                    return areaData.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get All Areas By CityID In Edit Mode ...
        public static List<AreaData> GetAreaListByCityIDEdit(int intSaleOfficerID, int intCityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var query = from saleoff in dbContext.SaleOfficers
                                where saleoff.Areas.Any(s => s.CityID == intCityID)
                                select saleoff.Areas;

                    var qArea = dbContext.SaleOfficers.Where(p => p.CityID == intCityID).SelectMany(p => p.Areas);

                    var qAreaEdit = dbContext.SaleOfficers.Where(p => p.ID == intSaleOfficerID).SelectMany(p => p.Areas);

                    var area = (from areas in dbContext.Areas
                                where areas.CityID == intCityID &&
                                !
                                    ((from saleoffarea in qArea
                                      select new
                                      {
                                          saleoffarea.ID
                                      }).Distinct()).Contains(new { ID = areas.ID })
                                orderby areas.Name
                                select new AreaData()
                                {
                                    ID = areas.ID,
                                    Name = areas.Name,
                                }).Union
                                (
                                     from saleoffarea in qAreaEdit
                                     orderby saleoffarea.Name
                                     select new AreaData()
                                     {
                                         ID = saleoffarea.ID,
                                         Name = saleoffarea.Name,
                                     }
                                );

                    return area.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Insert OR Update Area ...
        public static int AddUpdateArea(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Area AreaObj = new Area();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Areas.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = obj.CityID;
                            AreaObj.ShortCode = obj.ShortCode;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedDate = DateTime.Now;
                            AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.CreatedBy = 1;

                            dbContext.Areas.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Areas.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = obj.CityID;
                            AreaObj.ShortCode = obj.ShortCode;
                            AreaObj.LastUpdate = DateTime.Now;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        // Delete Area...
        public static int DeleteArea(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Area obj = dbContext.Areas.Where(u => u.ID == AreaID).FirstOrDefault();
                    dbContext.Areas.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Areas List For Grid ...
        public static List<AreaData> GetAreaForGrid(int intCityID)
        {
            List<AreaData> areaData = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Areas.Where(u => u.CityID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new AreaData
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    CityID = u.CityID,
                                    Name = u.Name,
                                    RegionName = "",
                                    CityName = u.City.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }


        public static List<AreaData> GetResult(string search, string sortOrder, int start, int length, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<AreaData> FilterResult(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            IQueryable<AreaData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[6].ToLower())))
                );

            return results;
        }
    
    }
}