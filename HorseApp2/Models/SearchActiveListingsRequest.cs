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

        /// <summary>
        /// New Fields
        /// </summary>
        public bool DamSearch { get; set; }
        public bool DamSireSearch { get; set; }
        public bool ColorSearch { get; set; }
        public bool LteSearch { get; set; }
        public bool InFoalSearch { get; set; }


        public List<string> HorseTypes { get; set; }
        public decimal PriceLow { get; set; }
        public decimal PriceHigh { get; set; }
        public List<string> Sires { get; set; }
        public List<string> Genders { get; set; }
        public List<string> Ages { get; set; }
        public List<string> Dams { get; set; }
        public List<string> DamSires { get; set; }
        public List<string> Colors { get; set; }
        public decimal LteHigh { get; set; }
        public decimal LteLow { get; set; }
        public bool InFoal { get; set; }
        /// <summary>
        /// New Items
        /// </summary>
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public bool OrderBy { get; set; }
        public int OrderByType { get; set; }
        public bool OrderByDesc { get; set; }

    }
}