
namespace FOS.DataLayer
{

    public partial class Sp_GetCallSummaryReport_Simple_Result
    {
        public long SrNo { get; set; }
        public string CallerName { get; set; }
        public string Region { get; set; }
        public int TotalSO { get; set; }
        public int CallDays { get; set; }
        public int TotalCalls { get; set; }
        public int CallsPerDay { get; set; }
        public int Owner { get; set; }
        public int PaintContractor { get; set; }
        public int ConstContractor { get; set; }
        public int ReadyToPaint { get; set; }
        public int ConstructionStageOther { get; set; }
        public int ApplyingBrighto { get; set; }
        public int Competitor { get; set; }
        public int OtherSiteStatus { get; set; }
        public int CallAttended { get; set; }
        public int NotAttended { get; set; }
        public int WrongData { get; set; }
    }

}
