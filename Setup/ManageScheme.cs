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
    public class ManageScheme
    {
        public static SchemeData GetEditScheme(int SchemeID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.TblMasterSchemes.Where(i => i.MasterSchemeID == SchemeID).Select(i => new SchemeData
                    {
                        SchemeID = i.MasterSchemeID,
                        SchemeInfo = i.SchemeInfo,
                        dateTo = i.SchemeDateTo.ToString(),
                        dateFrom = i.SchemeDateFrom.ToString()
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static int DeleteScheme(int schemeID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TblMasterScheme obj = dbContext.TblMasterSchemes.Where(u => u.MasterSchemeID == schemeID).FirstOrDefault();
                    dbContext.TblMasterSchemes.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete City Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static List<SchemeData> GetSchemesForGrid()
        {
            List<SchemeData> SchemeData = new List<SchemeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SchemeData = dbContext.TblMasterSchemes.OrderByDescending(x => x.isActive)
                            .ToList().Select(
                                u => new SchemeData
                                {
                                    SchemeID = u.MasterSchemeID,
                                    SchemeInfo = u.SchemeInfo,
                                    dateTo = Convert.ToDateTime(u.SchemeDateTo).ToString("dd-MM-yyyy"),
                                    dateFrom = Convert.ToDateTime(u.SchemeDateFrom).ToString("dd-MM-yyyy"),
                                    isActive = Convert.ToBoolean(u.isActive)
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Region List Failed");
                throw;
            }

            return SchemeData;
        }

        public static List<SchemeData> GetSchemeResult(string search, string sortOrder, int start, int length, List<SchemeData> dtResult, List<string> columnFilters)
        {
            return FilterSchemeResult(search, dtResult, columnFilters).Skip(start).Take(length).ToList();
        }

        private static IQueryable<SchemeData> FilterSchemeResult(string search, List<SchemeData> dtResult, List<string> columnFilters)
        {
            IQueryable<SchemeData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SchemeInfo != null && p.SchemeInfo.ToLower().Contains(search.ToLower())))
                );

            return results;
        }
        public static int Count(string search, List<SchemeData> dtResult, List<string> columnFilters)
        {
            return FilterSchemeResult(search, dtResult, columnFilters).Count();
        }
        public static int AddUpdateScheme(SchemeData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        TblMasterScheme RegionObj = new TblMasterScheme();

                        if (obj.SchemeID == 0)
                        {
                            RegionObj.MasterSchemeID = dbContext.TblMasterSchemes.OrderByDescending(u => u.MasterSchemeID).Select(u => u.MasterSchemeID).FirstOrDefault() + 1;
                            RegionObj.SchemeInfo = obj.SchemeInfo;
                            RegionObj.SchemeDateFrom = obj.SchemeDateFrom;
                            RegionObj.SchemeDateTo = obj.SchemeDateTo;
                            RegionObj.isActive = true;
                            dbContext.TblMasterSchemes.Add(RegionObj);
                        }
                        else
                        {
                            RegionObj = dbContext.TblMasterSchemes.Where(u => u.MasterSchemeID == obj.SchemeID).FirstOrDefault();
                            RegionObj.SchemeInfo = obj.SchemeInfo;
                            RegionObj.SchemeDateFrom = obj.SchemeDateFrom;
                            RegionObj.SchemeDateTo = obj.SchemeDateTo;
                            RegionObj.isActive = true;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
    }
}
