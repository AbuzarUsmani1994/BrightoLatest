using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class ShowJobData
    {
        public int JobID { get; set; }
        public int JobDetailID { get; set; }
        public int SalesOfficerID { get; set; }
        public string SalesOfficerName { get; set; }
        public int DealerID { get; set; }
        public string DealerName { get; set; }
        public int RetailerID { get; set; }
        public string RetailerName { get; set; }
        public string ShopName { get; set; }
        public int PainterID { get; set; }
        public string PainterName { get; set; }
        public int VisitPlanType { get; set; }
        public DateTime JobDate { get; set; }
        public DateTime? DateComplete { get; set; }
        public bool Status { get; set; }
        public int Count { get; set; }
    }


}