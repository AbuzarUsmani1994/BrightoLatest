using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetNotificationRelatedToSOControllerController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int EmployeeID, string StartDate, string EndDate,int SegmentID)
        {
            try
            {
                DateTime dtFromTodayUtc = Convert.ToDateTime(StartDate);
                DateTime dtFromTodayUt1 = Convert.ToDateTime(EndDate);
                DateTime dtFromToday = dtFromTodayUt1.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
               

                List<AllRetailersRetailerToSO> MAinCat2 = new List<AllRetailersRetailerToSO>();
                AllRetailersRetailerToSO cty2;
                List<AllRetailersRetailerToSO> list2;

               
                if (EmployeeID > 0)
                {

                    if (SegmentID == 1)
                    {
                        var Data = db.Tbl_TradeVisitsFinal.Where(x => x.SOID == EmployeeID && x.NextVisitDate >= dtFromTodayUtc && x.NextVisitDate <= dtToToday).ToList();

                        foreach (var item in Data)
                        {
                            cty2 = new AllRetailersRetailerToSO();
                            cty2.ID = item.CustomerID;
                            cty2.Name = item.Retailer.ShopName + "/" + item.Retailer.Address;
                            cty2.Date = item.NextVisitDate;
                            MAinCat2.Add(cty2);

                        }

                    }
                    else if (SegmentID == 2)
                    {

                        var Data = db.Tbl_HousingVisits.Where(x => x.SOID == EmployeeID && x.NextVisitDate >= dtFromTodayUtc && x.NextVisitDate <= dtToToday).ToList();

                        foreach (var item in Data)
                        {
                            cty2 = new AllRetailersRetailerToSO();
                            cty2.ID = item.CustomerID;
                            cty2.Name = item.Retailer.ShopName + "/" + item.Retailer.Address;
                            cty2.Date = item.NextVisitDate;
                            MAinCat2.Add(cty2);

                        }

                    }

                    else if (SegmentID == 3)
                    {

                        var Data = db.Tbl_CorporateVisits.Where(x => x.SOID == EmployeeID && x.NextVisitDate >= dtFromTodayUtc && x.NextVisitDate <= dtToToday).ToList();

                        foreach (var item in Data)
                        {
                            cty2 = new AllRetailersRetailerToSO();
                            cty2.ID = item.CustomerID;
                            cty2.Name = item.Retailer.ShopName + "/" + item.Retailer.Address;
                            cty2.Date = item.NextVisitDate;
                            MAinCat2.Add(cty2);

                        }

                    }






                    if (MAinCat2 != null && MAinCat2.Count > 0)
                    {
                        return Ok(new
                        {
                            NotificationList = MAinCat2

                        });
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "CitiesController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                MyListForAttendance = param
            });
        }


    }
}