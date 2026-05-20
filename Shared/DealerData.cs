using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOS.DataLayer;

namespace FOS.Shared
{
    public class DealerData
    {
        public DealerData()
        {
            RetailersPlanned = new List<RetailerData>();
            RetailersUnplanned = new List<RetailerData>();
        }
        public int ID { get; set; }

        [DisplayName("Dealer Name *")]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [DisplayName("Dealer Code *")]
        public string DealerCode { get; set; }

        //[Required(ErrorMessage = "* Required")]
        [DisplayName("Sales Officer *")]
        public int? SaleOfficerID { get; set; }

        [DisplayName("Regional Head *")]
        public int RegionalHeadID { get; set; }
        public int HiddenRegionalHeadID { get; set; }

        public string RegionalHeadName { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int CityID { get; set; }
        [Required(ErrorMessage = "* Required")]
        public int AreaID { get; set; }

        public int? rangeID { get; set; }

        // public int? CityID { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DisplayName("Address *")]
        [StringLength(250, ErrorMessage = "Dealer Address Must Be Less Then 250")]
        public string Address { get; set; }

        [RegularExpression(@"^(92)[0-9]{10}$", ErrorMessage = "Invalid Phone No")]
        [DisplayName("Phone No 1 *")]
        [Required(ErrorMessage = "* Required")]
        public string Phone1 { get; set; }

        [RegularExpression(@"^(92)[0-9]{10}$", ErrorMessage = "Invalid Phone No")]
        [DisplayName("Phone No 2")]
        public string Phone2 { get; set; }
        public List<RetailerData> RetailersPlanned { get; set; }
        public List<RetailerData> RetailersUnplanned { get; set; }

        public List<SaleOfficerData> SaleOfficers { get; set; }
        public List<CityData> Cities { get; set; }
        public List<AreaData> Areas { get; set; }
        public List<RegionalHeadData> RegionalHeads { get; set; }

        [DisplayName("Sale Officer Name")]
        public String SaleOfficerName { get; set; }

        public string BirthdayString { get; set; }
        //[DisplayName("City Name")]
        public string CityName { get; set; }
        public bool Planned { get; set; }

    }

    public class PlannedRetailerFilter
    {
        public PlannedRetailerFilter()
        {
            DealerList = new List<DealerData>();
        }
        public int RegionalHeadID { get; set; }
        public int CityID { get; set; }
        public int SaleOfficerID { get; set; }
        public int DealerID { get; set; }
        public int ZoneID { get; set; }
        public string month { get; set; }
        public List<DealerData> DealerList { get; set; }
    }
}