using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class RegionData
    {
        public int TID { get; set; }

        public int ID { get; set; }
        public int RegionID { get; set; }

        [DisplayName("Region Name *")]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int TerritoryID { get; set; }

        public string ActiveStatus { get; set; }

        [DisplayName("Region Code *")]
        [Required(ErrorMessage = "* Required")]
        public string ShortCode { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime LastUpdate { get; set; }

    }


    public class TerritoryData 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
    }




     public class PurposeOfActivityData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public Nullable<System.DateTime> LastUpdated { get; set; }
    }
}