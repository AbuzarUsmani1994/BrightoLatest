using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class RetailerPendingData
    {
        public int TID { get; set; }
        public int ID { get; set; }

        public string PersonName { get; set; }
        public string Name { get; set; }
        public int? DealerID { get; set; }
        public string DealerName { get; set; }
        public int SaleOfficerID { get; set; }
        public int? RegionalHeadID { get; set; }

        [DisplayName("Sale Officer Name")]
        public string SaleOfficerName { get; set; }


        [DisplayName("Shop Name")]
        public string ShopName { get; set; }

        public int AreaID { get; set; }
        [DisplayName("Area Name")]
        public string AreaName { get; set; }

        public int CityID { get; set; }
        [DisplayName("City Name")]
        public string CityName { get; set; }
        public string ZoneName { get; set; }
        public int ZoneID { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string BankName { get; set; }
        public string BankName2 { get; set; }
        public string AccountNo { get; set; }
        public string AccountNo2 { get; set; }
        public List<DealerData> Dealers { get; set; }
        public string strRetailerID { get; set; }
        public string strDealerID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnStr { get; set; }
    }
}