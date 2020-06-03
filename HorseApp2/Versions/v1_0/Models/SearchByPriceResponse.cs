using System.Collections.Generic;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    [ApiVersion("1.0")]
    public class SearchByPriceResponse
    {
        public List<HorseListing> Listings { get; set; }
    }
}