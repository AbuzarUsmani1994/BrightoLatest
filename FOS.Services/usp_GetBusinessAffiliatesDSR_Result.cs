using System;

namespace FOS.DataLayer
{
    public class usp_GetBusinessAffiliatesDSR_Result
    {
        public int VisitID { get; set; }
        public DateTime? VisitDate { get; set; }
        public int? SOID { get; set; }
        public string SaleOfficerName { get; set; }
        public int? BusinessAffiliateID { get; set; }
        public string BusinessName { get; set; }
        public string ContactPerson { get; set; }
        public int? ContactNumber { get; set; }
        public string BusinessAddress { get; set; }
        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public int? CityID { get; set; }
        public string CityName { get; set; }
        public string PurposeOfVisit { get; set; }
        public string TargetAgreement { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
