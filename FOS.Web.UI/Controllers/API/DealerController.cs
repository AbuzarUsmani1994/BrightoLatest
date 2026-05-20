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
    public class DealerController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        //public IHttpActionResult GetSODealers(int regionalHeadId)
        //{
        //    //try
        //    //{
        //    //    int soId = regionalHeadId; // now we are getting SOID instead of regionalHeadId
        //    //    List<int> soDealerIds = db.Retailers.Where(p => p.SaleOfficerID == soId 
        //    //    && !p.IsDeleted && p.IsActive && p.Status).Select(x => x.DealerID ?? 0).ToList<int>();
        //    //    return Ok(new
        //    //    {
        //    //        Dealers = db.Dealers.Where(d => soDealerIds.Contains(d.ID)
        //    //        && d.IsActive && !d.IsDeleted).Select(d => new
        //    //        {
        //    //            d.ID,
        //    //            d.Name
        //    //        }).OrderBy(d => d.Name)
        //    //    });
        //    //    //return Ok(new
        //    //    //{
        //    //    //Dealers = db.Dealers.Where(d => d.RegionalHeadID == regionalHeadId
        //    //    //&& d.IsActive && !d.IsDeleted).Select(d => new
        //    //    //{
        //    //    //    d.ID,
        //    //    //    d.Name
        //    //    //}).OrderBy(d => d.Name)
        //    //    //});

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Log.Instance.Error(ex, "GetSODealers API Failed");
        //    //    return Ok(new
        //    //    {
        //    //        Dealers = new { }
        //    //    });
        //    //}
        //}

        //public IHttpActionResult GetSODealersBirthdaysList(int soId)
        //{
        //    try
        //    {
        //        DateTime fromDate = DateTime.Today.AddDays(-3);
        //        DateTime toDate = DateTime.Today.AddDays(4);
        //        List<int> soDealerIds = db.Retailers.Where(p => p.SaleOfficerID == soId
        //        && p.IsActive && p.Status && !p.IsDeleted
        //        ).Select(x => x.DealerID ?? 0).ToList<int>();
        //        return Ok(new
        //        {
        //            Dealers = db.Dealers.Where(d => soDealerIds.Contains(d.ID)
        //            && d.IsActive && !d.IsDeleted
        //            && d.Birthday >= fromDate && d.Birthday <= toDate).Select(d => new
        //            {
        //                d.ID,
        //                d.Name,
        //                d.Birthday
        //            }).OrderBy(d => d.Name)
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Instance.Error(ex, "GetSODealersBirthdaysList API Failed");
        //        return Ok(new
        //        {
        //            Dealers = new { }
        //        });
        //    }
        //}
        //public IHttpActionResult GetSODealersList(int soId) // now its of no use for time being as above one is used instead of it.
        //{
        //    try
        //    {
        //        List<int> soDealerIds = db.Retailers.Where(p => p.SaleOfficerID == soId
        //        && p.IsActive && p.Status && !p.IsDeleted
        //        ).Select(x => x.DealerID ?? 0).ToList<int>();
        //        return Ok(new
        //        {
        //            Dealers = db.Dealers.Where(d => soDealerIds.Contains(d.ID)
        //            && d.IsActive && !d.IsDeleted).Select(d => new
        //            {
        //                d.ID,
        //                d.Name
        //            }).OrderBy(d => d.Name)
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Instance.Error(ex, "GetSODealersList API Failed");
        //        return Ok(new
        //        {
        //            Dealers = new { }
        //        });
        //    }
        //}
        //public IHttpActionResult GetSODealersBirthdays(int soId, int regionalHeadId)
        //{
        //    try
        //    {
        //        //var dte = DateTime.Now.Date;
        //        List<int> soRetailers = db.Retailers.Where(p => p.SaleOfficerID == soId
        //        && p.IsActive && p.Status && !p.IsDeleted
        //        ).Select(pp => pp.ID).ToList<int>();
        //        var DealersBirthdays = db.Dealers.Where(d => d.RegionalHeadID == regionalHeadId
        //        && d.Planned == true
        //        && d.IsActive && !d.IsDeleted
        //        && d.Birthday == DateTime.Today).Select(d => new DBirthdaysModel
        //        {
        //            ID = d.ID,
        //            Name = d.Name,
        //            Phone1 = d.Phone1,
        //            Address = d.Address,
        //            Birthday = d.Birthday.Value,
        //            Retailers = d.Retailers,
        //            Yes = false
        //        }).OrderBy(d => d.Name).ToList();

        //        foreach (var dealer in DealersBirthdays)
        //        {
        //            List<int> dealerRetailers = dealer.Retailers.Select(k => k.ID).ToList<int>();
        //            foreach (var soRet in soRetailers)
        //            {
        //                if (dealerRetailers.Contains(soRet))
        //                {
        //                    dealer.Yes = true;
        //                }
        //            }
        //        }

        //        return Ok(new
        //        {
        //            DealersBirthdays = DealersBirthdays.Where(dd => dd.Yes).Select(s => new
        //            {
        //                s.ID,
        //                s.Name,
        //                s.Birthday,
        //                s.Phone1,
        //                s.Address
        //            }).ToList()
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Instance.Error(ex, "GetSODealersBirthdays API Failed");
        //        return Ok(new
        //        {
        //            DealersBirthdays = new { }
        //        });
        //    }
        //}
    }
    public class DBirthdaysModel
    {
        public DBirthdaysModel()
        {
            this.Retailers = new HashSet<Retailer>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public bool Yes { get; set; }
        public DateTime Birthday { get; set; }

        public virtual ICollection<Retailer> Retailers { get; set; }

    }
}