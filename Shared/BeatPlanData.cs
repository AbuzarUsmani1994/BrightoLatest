using FOS.DataLayer;
using FOS.Shared.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{

    public class BeatPlanData
    {

        public BeatPlanData()
        {
            this.VisitPlanEach = new SelectedWeekday("0000000");
        }

        public int ID { get; set; }

        [DisplayName("Job Title *")]
        [Required(ErrorMessage = "* Required")]
        public string JobTitle { get; set; }

        public int Type { get; set; }
        [DisplayName("Regional Head *")]
        [Required(ErrorMessage = "* Required")]
        public Nullable<int> RegionalHeadID { get; set; }
        public int HiddenRegionalHeadID { get; set; }
        [DisplayName("Sales Officer Name *")]
        public string RegionalHeadName { get; set; }

        //Sales Officer
        [DisplayName("Sale Officer ID *")]
        [Required(ErrorMessage = "* Required")]
        public Nullable<int> SaleOfficerID { get; set; }
        [DisplayName("Sales Officer Name *")]
        public string SaleOfficerName { get; set; }

        public string RetailerType { get; set; }
        public string RT { get; set; }

        public string VisitType { get; set; }

        /// <Retailer>
        [DisplayName("RetailerID *")]
        [Required(ErrorMessage = "* Required")]
        public int RetailerID { get; set; }

        public string SelectedRetailers { get; set; }
        public string SelectedDealers { get; set; }
        public string SelectedPainters { get; set; }
        public string SelectedAreas { get; set; }
        
        [DisplayName("Retailer Name *")]
        public string RetailerName { get; set; }
        public int DealerID { get; set; }
        public string DealerName { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string LocationMargin { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public int? CityID { get; set; }
        public string CItyName { get; set; }
        public string AreaID { get; set; }
        public string AreaName { get; set; }
        /// </Retailer>

        [DisplayName("Schedule No")]
        [Required(ErrorMessage = "* Required")]
        public string JobNo { get; set; }

        /// <Visit Plan>
        [DisplayName("Visit Plan")]
        [Required(ErrorMessage = "* Required")]
        public int VisitPlanID { get; set; }

        public int VisitPlanHiddenID { get; set; }
        public string VisitPlanName { get; set; }
        public string VisitPlanDays { get; set; }
        public string VisitPlanWeeklyDays { get; set; }
        public string VisitPlanEachDays { get; set; }

        public SelectedWeekday VisitPlanEach
        {
            get;
            set;
        }
         
        public bool Status { get; set; }

        [DisplayName("Job Assign Date")]
        public string DateOfAssign { get; set; }
        public string VisitedDate { get; set; }

        [DisplayName("Job Compeletion Date")]
        public string DateOfExecution { get; set; }

        public Nullable<System.DateTime> StartingDate { get; set; }
        
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public IEnumerable<RegionalHeadTypeData> RegionalHeadTypeData { get; set; }
        public virtual List<RegionalHeadData> RegionalHead { get; set; }
        public virtual List<SaleOfficer> SaleOfficer { get; set; }
        public virtual List<DealerData> Dealer { get; set; }
        public virtual List<RetailerData> Retailers { get; set; }
        public virtual List<CityData> Cities { get; set; }
        public virtual List<VisitPlanData> VisitPlan { get; set; }

        // Painter 
        public string City { get; set; }
        public virtual List<PainterCityNamesData> PainterCityNames { get; set; }
        public int PainterCityID { get; set; }


        public string SelectiveDays { get; set; }
        public string BeatPlanType { get; set; }
        



    }
    
}


