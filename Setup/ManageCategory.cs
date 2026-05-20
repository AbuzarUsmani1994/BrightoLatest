using FOS.DataLayer;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Setup
{
    public class ManageCategory
    {
        public static List<CategoryData> GetCat()
        {
            List<CategoryData> cat = new List<CategoryData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cat = dbContext.MainCategories.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new CategoryData
                                {
                                    MainCategID = u.MainCategID,
                                    MainCategDesc = u.MainCategDesc,
                                    //RegionID = u.RegionID,
                                    //RegionName = u.Region.Name,
                                    //ShortCode = u.ShortCode,
                                    //LastUpdate = u.LastUpdate
                                }).OrderBy(x => x.MainCategDesc).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return cat;
        }

    }
}
