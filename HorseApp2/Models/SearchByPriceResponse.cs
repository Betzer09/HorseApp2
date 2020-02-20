using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    public class SearchByPriceResponse
    {
        public List<HorseListing> Listings { get; set; }
    }
}