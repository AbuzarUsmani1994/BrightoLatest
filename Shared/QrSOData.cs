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
    public class QrSOData
    {

        public QrSOData()
        {

        }
        public int QrSoID { get; set; }
        public int SaleOfficerId { get; set; }
        public int RetailerID { get; set; }
        public string QrCode { get; set; }
        public string SaleOfficerName { get; set; }
        public string RetailerName { get; set; }
        public string Remarks { get; set; }
        public string CreatedOnString { get; set; }
        public DateTime CreatedOn { get; set; }

    }
    
}