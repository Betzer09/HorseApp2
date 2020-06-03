using System.Collections.Generic;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    //Response object for searching active listings
    [ApiVersion("1.0")]
    public class SearchResponse
    {
        public int totalNumOfListings { get; set; }

        public int pageNumber { get; set; }
        
        public List<HorseListing> listings { get; set; }
    }
}