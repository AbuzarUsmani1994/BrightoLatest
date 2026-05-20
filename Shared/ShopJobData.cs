using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class ShopJobData
    {
        public int JobID { get; set; }
        public int JobsDetailID { get; set; }
        public int RegionalHeadID { get; set; }
        public int DealerID { get; set; }
        public int SalesOfficerID { get; set; }
        public int RetailerID { get; set; }
        public string SAvailable { get; set; }
        public int SQuantity1KG { get; set; }
        public int SQuantity5KG { get; set; }
        public string SNewOrder { get; set; }
        public int SNewQuantity1KG { get; set; }
        public int SNewQuantity5KG { get; set; }
        public int SPreviousOrder1KG { get; set; }
        public int SPreviousOrder5KG { get; set; }
        public string SPOSMaterialAvailable { get; set; }
        public string SImage { get; set; }
        public string SNote { get; set; }
        public string Token { get; set; }
        public string JobType { get; set; }
        public bool Status { get; set; }
        public DateTime DateComplete { get; set; }


        public List<DealerData> Dealers { get; set; }
        public List<RetailerData> Retailers { get; set; }
    }

    public class ConsolidateModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}

