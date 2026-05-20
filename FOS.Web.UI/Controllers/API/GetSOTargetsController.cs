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
    public class GetSOTargetsController : ApiController
    {
      

        public IHttpActionResult Get(int SOID,string Date)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<SOTargetsDto> MAinCat = new List<SOTargetsDto>();
                SOTargetsDto cty;
                DateTime now = Convert.ToDateTime(Date);
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                int? Visitssum = 0;
                int? CustomerSum = 0;
                int? SalesSum = 0;
                int? RecoveriesSum = 0;
                int? ActualVisitssum = 0;
                int? ActualCustomerSum = 0;
                int? ActualSalesSum = 0;
                decimal? ActualRecoveriesSum = 0;

                if (SOID > 0)
                {
                    object[] param = { SOID };


                    var sotargetsData = dbContext.Tbl_Targets.Where(x => x.SOID == SOID).FirstOrDefault();

                    if (sotargetsData != null)
                    {
                         Visitssum = sotargetsData.VisitsTargetsTrade + sotargetsData.VisitsTargetsHousing + sotargetsData.VisitsTargetsCorporate;
                         CustomerSum = sotargetsData.NewCustomerTrade + sotargetsData.NewCustomerHousing + sotargetsData.NewCustomerCorporate;
                         SalesSum = sotargetsData.SaleTrade + sotargetsData.SaleHousing + sotargetsData.SaleCorporate;
                         RecoveriesSum = sotargetsData.RecoveriesTrade + sotargetsData.RecoveriesHousing + sotargetsData.RecoveriesCorporate;
                    }

                    ActualCustomerSum = dbContext.Retailers.Where(x => x.SaleOfficerID == SOID && x.LastUpdate >= startDate && x.LastUpdate <= endDate).Count();

                    ActualRecoveriesSum = ActualRecoveriesSum = dbContext.TBL_Payments
                                      .Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                                      .Sum(x => (decimal?)x.Amount) ?? 0;


                    var tradevisit = dbContext.Tbl_TradeVisitsFinal.Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate).Count();


                    var Corporatevisit = dbContext.Tbl_CorporateVisits.Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate).Count();

                    var Housingvisit = dbContext.Tbl_HousingVisits.Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate).Count();

                    ActualVisitssum = tradevisit + Corporatevisit + Housingvisit;

                    var tradeVolume = dbContext.Tbl_TradeVisitsFinal
                              .Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                              .Sum(x => x.OrderVolume);



                    var CorporateVolume = dbContext.Tbl_CorporateVisits
                              .Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                              .Sum(x => x.OrderVolume);


                    var HousingVolume = dbContext.Tbl_HousingVisits
                              .Where(x => x.SOID == SOID && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                              .Sum(x => x.OrderVolume);



                    ActualSalesSum = (tradeVolume ?? 0) + (CorporateVolume ?? 0) + (HousingVolume ?? 0);



                    cty = new SOTargetsDto();

                    cty.VisitsTargets = Visitssum;
                    cty.ActualVisits = ActualVisitssum;
                    cty.CustomerTargets = CustomerSum;
                    cty.ActualCustomer = ActualCustomerSum;
                    cty.Recoveriestargets = RecoveriesSum;
                    cty.ActualRecoveries = ActualRecoveriesSum;
                    cty.SalesTargets = SalesSum;
                    cty.ActualSales = ActualSalesSum;
                    MAinCat.Add(cty);


                   // var result = dbContext.sp_GetSOListAccordingToDistributor(DistributorID, fromdate, todate).ToList();

                        if (MAinCat != null && MAinCat.Count > 0)
                        {
                            return Ok(new
                            {
                                SOTargets = MAinCat

                            });
                        }
                  
              
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DistributorRelatedToSO GET API Failed");
            }
            object[] paramm = {  };
            return Ok(new
            {
                SOTargets = paramm
            });

        }
    }
}