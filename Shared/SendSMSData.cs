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
   public class SendSMSData
    {
        public string msglanguage { get; set; }
        [DisplayName("Message *")]
        [Required(ErrorMessage = "* Required")]
        public string msg { get; set; }
        public string Type { get; set; }
        public int? SaleOfficerID { get; set; }
        public string SaleOfficerName { get; set; }
        public int? DealerID { get; set; }
        public string DealerName { get; set; }
        public int? ReatilerID { get; set; }
        public string ReatilerName { get; set; }
        public int? CityID { get; set; }
        public string CityName { get; set; }
        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public string MsgStatus { get; set; }
        public DateTime? MSgDate { get; set; }
        public string Phone1 { get; set; }
        public List<RegionData> regiondata { get; set; }
        public List<RetailerData> retailerdata{ get; set; }

        public List<SaleOfficerData> SaleOfficersdata { get; set; }
        public List<CityData> citiesdata { get; set; }
        public List<AreaData> areadata { get; set; }
        public List<RegionalHeadData> RegionalHeads { get; set; }
        public List<DealerData> dealerdata { get; set; }

    }
}
