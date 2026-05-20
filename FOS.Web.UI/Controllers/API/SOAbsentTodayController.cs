using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SOAbsentTodayController : ApiController
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
                // List<Tbl_Access> list = null;
                var list = dbContext.Tbl_Access.Where(x => x.RepotedUP == Saleofficerid).ToList();

                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                List<AbsentData> obj = new List<AbsentData>();


                foreach (var itm in list)
                {

                    List<AccessLog> itmC = dbContext.AccessLogs.Where(x => x.LoginDate >= dtFromToday && x.LoginDate <= dtToToday && x.SaleOfficerID == itm.SaleOfficerID).ToList();
                    if (itmC.Count == 0)
                    {

                        if (obj.Count >= 1)
                        {
                            var result = obj.Find(x => x.SaleOfficerID == itm.SaleOfficerID);
                            if (result == null)
                            {
                                obj.Add(new AbsentData()
                                {
                                    //the name of the db column

                                    SaleOfficerName = itm.SaleOfficer.Name,
                                    SaleOfficerID = itm.SaleOfficerID,
                                    LoginDate=itm.CreatedOn

                                });

                            }
                        }
                        else
                        {
                            obj.Add(new AbsentData()
                            {
                                //the name of the db column

                                SaleOfficerName = itm.SaleOfficer.Name,
                                SaleOfficerID = itm.SaleOfficerID,
                                LoginDate = itm.CreatedOn
                            });
                        }

                    }
                }

                //if (obj.Count <= 0)
                //{
                //    obj.Add(new AbsentData()
                //    {
                //        SaleOfficerName = "No Absent Today",
                //        ID = 0,

                //        SaleOfficerID = 0,

                //    });
                //}
                var data = obj.ToList();

                if (data.Count != 0)
                {
                    return Ok(new
                    {
                        AbsentSO = data
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
                AbsentSO = paramm
            });

        }
    }
}
public class AbsentData
{
    public int ID { get; set; }
    public int SaleOfficerID { get; set; }
    public System.DateTime LoginDate { get; set; }
    public Nullable<int> Status { get; set; }
    public string SaleOfficerName { get; set; }
}