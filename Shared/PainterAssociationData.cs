using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FOS.Shared
{
    public class PainterAssociationData
    {


        public int ID { get; set; }

        public int PainterID { get; set; }
        public string PainterName { get; set; }
        public string City { get; set; }
        public string SelectedPainters { get; set; }


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

        //Retailer
        [DisplayName("RetailerID *")]
        [Required(ErrorMessage = "* Required")]
        public int RetailerID { get; set; }
        [DisplayName("Retailer Name *")]
        public string RetailerName { get; set; }
        public bool PainterAssociationStatus { get; set; }

        
        public int NoOfPainters { get; set; }
        public bool Selected { get; set; }


        public virtual List<RegionalHeadData> RegionalHeads { get; set; }
        public virtual List<SaleOfficer> SalesOfficer { get; set; }
        public virtual List<RetailerData> Retailers { get; set; }
        public virtual List<PainterCityNamesData> PainterCityNames { get; set; }
        public int PainterCityID { get; set; }

    }


    public class PainterCityNamesData {
        public int ID { get; set; }
        public string CityName { get; set; }
        public bool Selected { get; set; }
    }

    public class ReportsInputData
    {
        public int TID { get; set; }
        public string FosID { get; set; }
        public string CID { get; set; }
        public string Shoptypeid { get; set; }
        public string Reporttypeid { get; set; }
        public virtual List<TerritoriesNameData> Territories { get; set; }
        public virtual List<PainterCityNamesData> PainterCityNames { get; set; }
        public virtual IEnumerable<SelectListItem> FosNames { get; set; }
        public virtual IEnumerable<SelectListItem> ShopTypeLov { get; set; }
        public virtual IEnumerable<SelectListItem> ReportTypeLov { get; set; }
        public ReportsInputData()
        {
            var ShopType = new List<LovClass>
            {
                    new LovClass { Name = "Regular", ID = "Regular" },
                    new LovClass { Name = "Focused", ID = "Focused" },
            };
            ShopTypeLov = ShopType.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID
            });

            var ReportType = new List<LovClass>
            {
                    new LovClass { Name = "Intake Report", ID = "I" },
                    new LovClass { Name = "Delivery Report", ID = "D" },
            };
            ReportTypeLov = ReportType.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID
            });
        }
        //public virtual IEnumerable<SelectListItem> PainterCityNames { get; set; }
    }

    public class TerritoriesNameData
    {
        public int ID { get; set; }
        public string TeritoryName { get; set; }
    }

    public class LovClass
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
