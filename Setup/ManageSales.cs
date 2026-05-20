using FOS.DataLayer;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Setup
{
    public class ManageSales
    {


        // Insert OR Update Jobs ...
        public static int AddShopVisit(JobsDetailData obj)
        {
            int Res = 0;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Job JobObj = new Job();
                    JobsHistory ShopJobObj = new JobsHistory();

                    var Retailer = dbContext.Retailers.Where(r => r.ID == obj.RetailerID).FirstOrDefault();

                    JobObj.ID = 2;
                    JobObj.SaleOfficerID = Retailer.SaleOfficerID;
                    JobObj.RetailerType = Retailer.RetailerType;

                    ShopJobObj.JobID = 2;
                    ShopJobObj.DealerID = obj.DealerID;
                    ShopJobObj.RetailerID = (int)obj.RetailerID;
                    ShopJobObj.SaleOfficerID = Retailer.SaleOfficerID;
                    ShopJobObj.RetailerID = (int)obj.RetailerID;
                    ShopJobObj.SAvailable = obj.SAvailable;
                    ShopJobObj.SQuantity1KG = obj.SQuantity1KG;
                    ShopJobObj.SQuantity5KG = obj.SQuantity5KG;
                    ShopJobObj.SNewOrder = obj.SNewOrder;
                    ShopJobObj.SNewQuantity1KG = obj.SNewQuantity1KG;
                    ShopJobObj.SNewQuantity5KG = obj.SNewQuantity5KG;
                    ShopJobObj.SPreviousOrder1KG = obj.SPreviousOrder1KG;
                    ShopJobObj.SPreviousOrder5KG = obj.SPreviousOrder5KG;
                    ShopJobObj.SPOSMaterialAvailable = obj.SPOSMaterialAvailable;
                    ShopJobObj.SNote = obj.SNote;
                    ShopJobObj.JobType = "M";
                    ShopJobObj.Status = true;
                    ShopJobObj.VisitedDate = DateTime.Now;

                    
                    //string[] ARRAY = obj.VisitedDate.Split('-');
                    //ShopJobObj.VisitedDate = new DateTime(Convert.ToInt32(ARRAY[1]), Convert.ToInt32(ARRAY[0]), DateTime.DaysInMonth(Convert.ToInt32(ARRAY[1]), Convert.ToInt32(ARRAY[0])));
                    ShopJobObj.CreatedDate = DateTime.Now;

                    dbContext.JobsHistories.Add(ShopJobObj);
                    dbContext.SaveChanges();
                    Res = 1;
                }
            }
            catch (Exception)
            {
                Res = 0;
            }


            return Res;
        }



    }
}
