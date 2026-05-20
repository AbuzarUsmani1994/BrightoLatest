using FOS.DataLayer;
using FOS.Setup;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetVisitsDeleteController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Get(int VisitID,int SegmentTypeID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
                {

                if (SegmentTypeID == 2)
                {

                    Tbl_HousingVisits housing = new Tbl_HousingVisits();

                    //ADD New Retailer 
                    housing = db.Tbl_HousingVisits.Where(u => u.ID == VisitID).FirstOrDefault();

                    housing.IsActive = false;

                    db.SaveChanges();

                }

                if (SegmentTypeID == 1)
                {

                    Tbl_TradeVisitsFinal trade = new Tbl_TradeVisitsFinal();

                    //ADD New Retailer 
                    trade = db.Tbl_TradeVisitsFinal.Where(u => u.ID == VisitID).FirstOrDefault();

                    trade.IsActive = false;

                    db.SaveChanges();

                }
                if (SegmentTypeID == 3)
                {

                    Tbl_CorporateVisits coorporate = new Tbl_CorporateVisits();

                    //ADD New Retailer 
                    coorporate = db.Tbl_CorporateVisits.Where(u => u.ID == VisitID).FirstOrDefault();

                    coorporate.IsActive = false;

                    db.SaveChanges();

                }

                if (SegmentTypeID == 5)
                {

                    Tbl_AllPurposeVisits coorporate = new Tbl_AllPurposeVisits();

                    //ADD New Retailer 
                    coorporate = db.Tbl_AllPurposeVisits.Where(u => u.ID == VisitID).FirstOrDefault();

                    coorporate.IsActive = false;

                    db.SaveChanges();

                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Deleted Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null


                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Deleted API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Deleted API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        

        }


    }
}