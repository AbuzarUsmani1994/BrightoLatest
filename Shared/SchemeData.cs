using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
   public class SchemeData
    {
        public int SchemeID { get; set; }
        public string SchemeInfo { get; set; }
        public DateTime SchemeDateFrom { get; set; }
        public DateTime SchemeDateTo { get; set; }
        public string dateTo { get; set; }
        public string dateFrom { get; set; }
        public int RangeID { get; set; }
        public int DealerID { get; set; }
        public int SOID { get; set; }
        public string RangeName { get; set; }
        public string SOName { get; set; }
        public string DealerName { get; set; }
        public List<CategoryData> RangeData { get; set; }

        public Boolean isActive { get; set; }
    }
}
