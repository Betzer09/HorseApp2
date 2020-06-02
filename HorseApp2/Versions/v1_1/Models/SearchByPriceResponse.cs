using System.Collections.Generic;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Models
{
    [ApiVersion("1.1")]
    public class SearchByPriceResponse
    {
        public List<HorseListing> Listings { get; set; }
    }
}