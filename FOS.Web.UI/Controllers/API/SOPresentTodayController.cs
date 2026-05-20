using FOS.DataLayer;
using FOS.Setup;
using Newtonsoft.Json;
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
    public class SOPresentTodayController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {


                var SoHeirarchy = dbContext.Sp_PresentSOHeirarchy1_1(SOID).ToList();

                int Saleofficerid = 0;
                foreach (var item in SoHeirarchy)
                {
                    Saleofficerid = item.SaleOfficerID;
                }
             
                var list = dbContext.Tbl_Access.Where(x => x.RepotedUP == Saleofficerid).ToList();

                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                List<PresentData> obj = new List<PresentData>();


                foreach (var itm in list)
                {
                    var data = db.SOAttendances.Where(x => x.SOID == itm.SaleOfficerID && x.Type == "Market start" && x.CreatedAt >= dtFromToday && x.CreatedAt <= dtToToday).FirstOrDefault();

                    //DateTime? xx = dbContext.JobsDetails.Where(x => x.JobDate >= dtFromToday && x.SalesOficerID==itm.SaleOfficerID).Select(u => u.JobDate).Min();
                    //List<AccessLog> itmC = dbContext.AccessLogs.Where(x => x.LoginDate >= dtFromToday && x.LoginDate <= dtToToday && x.SaleOfficerID == itm.SaleOfficerID).ToList();
                    //if (itmC.Count >= 1 && xx!=null)
                    //{
                    //    foreach (var ele in itmC)
                    //    {
                    //        var data = dbContext.Sp__FirstVistCity(dtFromToday, dtToToday, ele.SaleOfficerID).SingleOrDefault();
                    //        if(data!=null)
                    //        {
                    obj.Add(new PresentData()
                    {
                        SaleOfficerName = data.SaleOfficer.Name,
                        ID = data.SaleOfficer.ID,
                        LoginDate = data.CreatedAt,
                        RetailerName = db.Retailers.Where(x => x.ID == data.RetailerID).Select(x => x.ShopName).FirstOrDefault()
                    }) ;
                    //        }
                    //        else

                    //        {
                    //            obj.Add(new PresentData()
                    //            {
                    //                SaleOfficerName = ele.SaleOfficer.Name,
                    //                ID = ele.ID,
                    //                LoginDate = xx,
                    //                SaleOfficerID = ele.SaleOfficerID,
                    //                Status = ele.Status
                    //            });

                    //        }

                    //    }
                    //}
                }

              
                //var data = obj;
                var list1 = obj;
                if (list1 != null)
                {
                    
                    return Ok(new
                    {
                        PresentSO = list1

                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                PresentSO = paramm
            });

        }


    }
}
public class PresentData
{
    public int ID { get; set; }
    public int SaleOfficerID { get; set; }
    public System.DateTime? LoginDate { get; set; }
    public Nullable<int> Status { get; set; }
    public string RetailerName { get; set; }

    public string SaleOfficerName { get; set; }
}