using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class RegionalHeadData
    {
        public int TID { get; set; }
        public int ID { get; set; }

        [DisplayName("Regional Head Name *")]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [DisplayName("Regional Head Name *")]
        [Required(ErrorMessage = "* Required")]
        public int Type { get; set; }
        public string TypeName { get; set; }

      
        public string Phone1 { get; set; }

        public string Phone2 { get; set; }
        public string IsActiveYes { get; set; }

        public System.DateTime LastUpdate { get; set; }
      
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? RangeID { get; set; }
        public IEnumerable<RegionData> Regions { get; set; }

        [DisplayName("Region *")]
        public String RegionID { get; set; }

        [DisplayName("Region Name *")]
        public String RegionName { get; set; }
        public string RangeName { get; set; }
        public IEnumerable<RegionalHeadTypeData> Ranges { get; set; }
        public IEnumerable<RegionalHeadTypeData> RegionalHeadTypeData { get; set; }
    }

    public class RegionalHeadGraphData
    {
        public string RegionalHeadName { get; set; }
        public int RegionCount { get; set; }
    }

    public class RegionalHeadTypeData
    {
        public int ID { get; set; }
        public string Type { get; set; }
    }



}