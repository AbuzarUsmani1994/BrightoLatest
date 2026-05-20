using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using System;
using System.Linq;
using System.Web.Mvc;
using Excel;
namespace FOS.Web.UI.Controllers
{
    public class TestController : Controller
    {

        FOSDataModel db = new FOSDataModel();

        public ActionResult Upload()
        {
            var excelFilePath = Server.MapPath(@"~\Content\retailer.xlsx");

            foreach (var worksheet in Workbook.Worksheets(excelFilePath))
            {
                int i = 1;
                foreach (var row in worksheet.Rows.Skip(1))
                {
                    i++;
                    if (row.Cells[1] != null && row.Cells[1].Value.Trim().Length > 0)
                    {

                        ManageRetailer.AddUpdateRetailer(new RetailerData
                        {
                            ID = 0,//GetRetailerId(row.Cells[6].Text.Trim()),
                            Name = row.Cells[4].Text.Trim(),
                            ShopName = row.Cells[6].Text.Trim(),
                            DealerID = GetDealerId(row.Cells[3].Text.Trim()),
                           // SaleOfficerID = GetSOId(row.Cells[2].Text.Trim(), row.Cells[1].Text.Trim()),
                            CityID = GetCityId(row.Cells[8].Text.Trim(), row.Cells[1].Text.Trim()),
                            AreaID = GetAreaId(row.Cells[9].Text.Trim(), row.Cells[1].Text.Trim(), row.Cells[8].Text.Trim()),
                            Address = row.Cells[10].Text.Trim(),
                            RetailerCode = "",
                            CNIC = "",
                            AccountNo = "",
                            AccountNo2 = "",
                            CardNumber = "",
                            Email = "",
                            BankName2 = "",
                            Phone1 = row.Cells[11].Text.Trim(),
                            Phone2 = row.Cells[12].Text.Trim(),
                            RetailerType = row.Cells[5].Text.Trim()
                        });
                    }
                }
            }
            
            return View();
        }

        private int GetRetailerId(string shopName)
        {
            shopName = shopName.Trim();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var retailer = dbContext.Retailers.Where(u => u.ShopName.ToLower().Equals(shopName.Trim().ToLower())).FirstOrDefault();
                    if (retailer != null)
                    {
                        return retailer.ID;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private int GetDealerId(string dealerName)
        {
            dealerName = dealerName.Trim();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var dealer = dbContext.Dealers.Where(u => u.Name.ToLower().Equals(dealerName.Trim().ToLower())).FirstOrDefault();
                    if (dealer != null)
                    {
                        return dealer.ID;
                    }
                    else
                    {
                        return -1;
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private int GetSOId(string soName, string thName)
        //{
        //    soName = soName.Trim();
        //    thName = thName.Trim();
        //    try
        //    {
        //        using (FOSDataModel dbContext = new FOSDataModel())
        //        {
        //        //    var so = dbContext.SaleOfficers.Where(u => u.Name.ToLower().Equals(soName.Trim().ToLower())).FirstOrDefault();
        //        //    if (so != null)
        //        //    {
        //        //        return so.ID;
        //        //    }
        //        //    else
        //        //    {
        //        //        ManageSaleOffice.AddUpdateSaleOfficer(new SaleOfficerData
        //        //        {
        //        //            Name = soName,
        //        //            UserName = soName,
        //        //            Password = soName + "123",
        //        //            HiddenRegionalHeadID = thName.Equals("Zeeshan Haral") ? 2 : (thName.Equals("Usman Javaid") ? 1 : 3)

        //        //        });

        //        //        return GetSOId(soName, thName);
        //        //    }
        //        //}
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        private int GetCityId(string cityName, string thName)
        {
            cityName = cityName.Trim();
            thName = thName.Trim();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var city = dbContext.Cities.Where(u => u.Name.ToLower().Equals(cityName.Trim().ToLower())).FirstOrDefault();
                    if (city != null)
                    {
                        return city.ID;
                    }
                    else
                    {
                        ManageCity.AddUpdateCity(new CityData
                        {
                            Name = cityName,
                            RegionID = thName.Equals("Zeeshan Haral") ? 1 : (thName.Equals("Usman Javaid") ? 2 : 3),
                            ShortCode = ""

                        });

                        return GetCityId(cityName, thName);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int GetAreaId(string areaName, string thName, string cityName)
        {
            areaName = areaName.Trim();
            cityName = cityName.Trim();
            thName = thName.Trim();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    int cityId = GetCityId(cityName, thName);
                    var area = dbContext.Areas.Where(u => u.Name.ToLower().Equals(areaName.Trim().ToLower()) && u.CityID == cityId).FirstOrDefault();
                    if (area != null)
                    {
                        return area.ID;
                    }
                    else
                    {
                        ManageArea.AddUpdateArea(new AreaData
                        {
                            Name = areaName,
                            RegionID = thName.Equals("Zeeshan Haral") ? 1 : (thName.Equals("Usman Javaid") ? 2 : 3),
                            CityID = cityId,
                            ShortCode = ""
                        });

                        return GetAreaId(areaName, thName, cityName);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
