using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class VisitDetailMapViewController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string Date, int SegmentType)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<VisitDetailMapDto> list = new List<VisitDetailMapDto>();

                VisitDetailMapDto comlist;
                List<VisitDetailMapDto> lists = new List<VisitDetailMapDto>();

                VisitDetailMapDto comlists;
                DateTime dtFromTodayUtc = Convert.ToDateTime(Date);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                var RegionID = db.SOAttendances.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromTodayUtc && x.CreatedAt <= dtToToday).FirstOrDefault();

                if (SOID > 0)
                {
                    object[] param = { SOID };


                    if (SegmentType == 1)
                    {

                        var result = db.Tbl_TradeVisitsFinal.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromTodayUtc && x.CreatedAt <= dtToToday && x.IsActive==true).ToList();

                        foreach (var item in result)

                        {
                            comlist = new VisitDetailMapDto();
                            comlist.CustomerName = item.Retailer.ShopName + "/" + item.CreatedAt;
                            comlist.Lattitude = item.Latitude;
                            comlist.Longitude = item.Longitude;
                            comlist.VisitPurpose = "Ordering";
                            comlist.VisitDate = item.CreatedAt;

                            list.Add(comlist);
                        }

                    }

                    if (SegmentType == 2)
                    {

                        var result = db.Tbl_HousingVisits.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromTodayUtc && x.CreatedAt <= dtToToday && x.IsActive == true).ToList();

                        foreach (var item in result)

                        {
                            comlist = new VisitDetailMapDto();
                            comlist.CustomerName = item.Retailer.ShopName + "/" + item.CreatedAt;
                            comlist.Lattitude = item.Latitude;
                            comlist.Longitude = item.Longitude;
                            comlist.VisitPurpose = "Ordering";
                            comlist.VisitDate = item.CreatedAt;

                            list.Add(comlist);
                        }

                    }

                    if (SegmentType == 3)
                    {

                        var result = db.Tbl_CorporateVisits.Where(x => x.SOID == SOID && x.CreatedAt >= dtFromTodayUtc && x.CreatedAt <= dtToToday && x.IsActive == true).ToList();

                        foreach (var item in result)

                        {
                            comlist = new VisitDetailMapDto();
                            comlist.CustomerName = item.Retailer.ShopName + "/" + item.CreatedAt;
                            comlist.Lattitude = item.Latitude;
                            comlist.Longitude = item.Longitude;
                            comlist.VisitPurpose = "Ordering";
                            comlist.VisitDate = item.CreatedAt;

                            list.Add(comlist);
                        }

                    }

                    //var result = dbContext.Sp_MyVisitsMapViewForGPC1_4(SOID,dtFromToday,dtToToday,RegionID.RegionID,RegionID.CityID).ToList();

                 

                    if (list != null)
                    {
                        return Ok(new
                        {
                            MyVisitsMapView = list

                        });
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyVisitsMapView  API Failed");
            }
            object[] paramm = {  };
            return Ok(new
            {
                MyVisitsMapView = paramm
            });

        }


    }
}