using FOS.DataLayer;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOS.Setup
{
    public class ManageItem
    {
        public static List<Items> GetItemList(int? RangeID)
        {
            List<Items> itemsList = new List<Items>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    itemsList = dbContext.Items.Where (u=>u.MainCategID==RangeID && u.IsActive==true)
                            .Select(
                                u => new Items
                                {
                                    ID = u.ItemID,
                                    ItemName = u.ItemName,
                                    ItemPacking = u.Packing,
                                    //RegionName = u.City.Region.Name,
                                    //CityID = u.CityID,
                                    //CityName = u.City.Name,
                                    ItemPrice = u.Price,
                                    MainCategoryID = u.MainCategID
                                }).OrderBy(u=>u.ItemName).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return itemsList;
        }


        //public static List<Items> GetResult(string search, string sortOrder, int start, int length, List<Items> dtResult, List<string> columnFilters)
        //{
        //    return FilterResult(search, dtResult, columnFilters).sortby(sortOrder).Skip(start).Take(length).ToList();
        //}


        public static int Count(string search, List<Items> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }
        private static IQueryable<Items> FilterResult(string search, List<Items> dtResult, List<string> columnFilters)
        {
            IQueryable<Items> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.ItemName != null && p.ItemName.ToLower().Contains(search.ToLower()) ))
                && (columnFilters[3] == null || (p.ItemName != null && p.ItemName.ToLower().Contains(columnFilters[3].ToLower())))
               );

            return results;
        }
    }
}
