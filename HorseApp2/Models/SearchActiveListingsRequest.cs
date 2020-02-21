using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    public class SearchActiveListingsRequest
    {
        public bool TypeSearch { get; set; }
        public bool PriceSearch { get; set; }
        public bool SireSearch { get; set; }
        public bool GenderSearch { get; set; }
        public bool AgeSearch { get; set; }

        public string HorseType { get; set; }
        public decimal PriceLow { get; set; }
        public decimal PriceHigh { get; set; }
        public string Sire { get; set; }
        public string Gender { get; set; }
        public List<int> Ages { get; set; }
    }
}