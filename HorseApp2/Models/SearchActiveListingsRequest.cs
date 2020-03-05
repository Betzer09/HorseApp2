using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    //Request object for searching the active listings
    public class SearchActiveListingsRequest
    {
        //NOT REQUIRED
        public bool TypeSearch { get; set; }
        //NOT REQUIRED
        public bool PriceSearch { get; set; }
        //NOT REQUIRED
        public bool SireSearch { get; set; }
        //NOT REQUIRED
        public bool GenderSearch { get; set; }
        //NOT REQUIRED
        public bool AgeSearch { get; set; }
        //NOT REQUIRED
        public bool DamSearch { get; set; }
        //NOT REQUIRED
        public bool DamSireSearch { get; set; }
        //NOT REQUIRED
        public bool ColorSearch { get; set; }
        //NOT REQUIRED
        public bool LteSearch { get; set; }
        //NOT REQUIRED
        public bool InFoalSearch { get; set; }
        public bool ActiveListingIdSearch { get; set; }
        //NOT REQUIRED
        public List<string> HorseTypes { get; set; }
        //NOT REQUIRED
        public decimal PriceLow { get; set; }
        //NOT REQUIRED
        public decimal PriceHigh { get; set; }
        //NOT REQUIRED
        public List<string> Sires { get; set; }
        //NOT REQUIRED
        public List<string> Genders { get; set; }
        //NOT REQUIRED
        public List<int> Ages { get; set; }
        //NOT REQUIRED
        public List<string> Dams { get; set; }
        //NOT REQUIRED
        public List<string> DamSires { get; set; }
        //NOT REQUIRED
        public List<string> Colors { get; set; }
        //NOT REQUIRED
        public decimal LteHigh { get; set; }
        //NOT REQUIRED
        public decimal LteLow { get; set; }
        //NOT REQUIRED
        public bool InFoal { get; set; }
        public List<string> ActiveListingIds { get; set; }
        //REQUIRED
        public int ItemsPerPage { get; set; }
        //REQUIRED
        public int Page { get; set; }
        //REQUIRED
        public bool OrderBy { get; set; }
        //REQUIRED
        public int OrderByType { get; set; }
        //REQUIRED
        public bool OrderByDesc { get; set; }

    }
}