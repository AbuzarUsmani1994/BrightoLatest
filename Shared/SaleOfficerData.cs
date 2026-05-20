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
    public class SaleOfficerData
    {
        public int TID { get; set; }
        public int ID { get; set; }

        public int SOTypeID { get; set; }

        [DisplayName("Sale Officer *")]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DisplayName("User Name *")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DisplayName("Password *")]
        public string Password { get; set; }

        public string ECode { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string RangeName { get; set; }
        public string RegionName { get; set; }
        //[Required(ErrorMessage = "* Required")]
        public int Type { get; set; }
        public int? RangeID { get; set; }
        public int? DesignationID { get; set; }
        public int? RegionID { get; set; }
        public int? RegionalHeadID { get; set; }
        public int HiddenRegionalHeadID { get; set; }
        //[Required(ErrorMessage = "* Required")]
        //[RegularExpression(@"^(92)[0-9]{10}$", ErrorMessage = "Invalid Phone No")]
        //[Required(ErrorMessage = "* Required")]
        //[DisplayName("Phone No 1 *")]
        public string Phone1 { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveYes { get; set; }

        //[RegularExpression(@"^(92)[0-9]{10}$", ErrorMessage = "Invalid Phone No")]
        //[DisplayName("Phone No 2")]
        public string Phone2 { get; set; }
        public string DesignationName { get; set; }
        public string Createdate { get; set; }
        public string leaveondate { get; set; }
        public int? CityID { get; set; }
        public int? PrimaryCityID { get; set; }
        public int? SecondaryCityID { get; set; }
        public int? SegmentTypeID { get; set; }
        public DateTime? CreatedAT { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public DateTime? Leaveon { get; set; }
        public int CreatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public List<RegionalHeadData> RegionalHead { get; set; }
        public IEnumerable<RegionalHeadTypeData> RegionalHeadTypeData { get; set; }
        public IEnumerable<RegionalHeadTypeData> Ranges { get; set; }
        public IEnumerable<RegionalHeadTypeData> Designations { get; set; }
        public IEnumerable<RegionalHeadTypeData> SOTypes { get; set; }
        public IEnumerable<RegionalHeadTypeData> SegmentTypes { get; set; }
        public IEnumerable<RegionalHeadTypeData> SORegion { get; set; }
        public IEnumerable<CityData> Cities { get; set; }

        [Required(ErrorMessage = "* Required")]
        public ICollection<Area> Areas { get; set; }

        [DisplayName("Regional Head Name")]
        public String RegionalHeadName { get; set; }

        public String AreaID { get; set; }

        [DisplayName("Area Name")]
        public String AreaName { get; set; }

        [DisplayName("City Name")]
        public String CityName { get; set; }
    }

    public class SaleOfficerTargets
    {
      
        public int ID { get; set; }

        public int SOID { get; set; }

        public int? RegionalHeadID { get; set; }
        public int VisitTargetTrade { get; set; }
        public int? VisitTargetHousing { get; set; }
        public int? VisitTargetCorporate { get; set; }

        public int NewCustomerTrade { get; set; }
        public int? NewCustomerHousing { get; set; }
        public int? NewCustomerCorporate { get; set; }

        public int SaleTrade { get; set; }
        public int? SaleHousing { get; set; }
        public int? SaleCorporate { get; set; }

        public int RecoveriesTrade { get; set; }
        public int? RecoveriesHousing { get; set; }
        public int? RecoveriesCorporate { get; set; }



      
        public string Createdate { get; set; }
        public string leaveondate { get; set; }
        
        public DateTime? CreatedAT { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public DateTime? Leaveon { get; set; }
        public int CreatedBy { get; set; }

        public List<RegionalHeadData> RegionalHead { get; set; }

    }

}