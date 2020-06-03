using System.Collections.Generic;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    //Request object for searching the active listings
    [ApiVersion("1.0")]
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
        public bool InFoalToSearch { get; set; }
        public bool isSoldSearch { get; set; }
        public bool ActiveListingIdSearch { get; set; }
        public bool isRegisteredSearch { get; set; }
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
        public List<string> InFoalTo { get; set; }
        public bool IsSold { get; set; }
        public bool isRegistered { get; set; }
        //Might need, not sure yet
        //public bool callForPrice { get; set }
        public bool HeightSearch { get; set; }
        public List<double> Heights { get; set; }

        #region Location Search Parameters

        // Start Location Search Parameters
        public bool LocationsSearch { get; set; } // Not Required
        public string PostalCode { get; set; }
        public int Range { get; set; } = 25;
        public string CountryCode { get; set; }
        public string Unit { get; set; } = "mile";
        // End Location Search Parameters

        #endregion
        

        public bool IsSireRegisteredSearch { get; set; }
        public bool IsDamSireRegisteredSearch { get; set; }
        public bool IsSireRegistered { get; set; }
        public bool IsDamSireRegistered { get; set; }

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