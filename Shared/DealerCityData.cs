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
    public class DealerCityData : DealerData
    {
        public DealerCityData()
        {
            Dealers = new List<DealerData>();
        }
        public int DealerID { get; set; }
        public string AreaName { get; set; }
        public List<DealerData> Dealers { get; set; }

    }
}