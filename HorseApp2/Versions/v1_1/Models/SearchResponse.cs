using System.Collections.Generic;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Models
{
    //Response object for searching active listings
    [ApiVersion("1.1")]
    public class SearchResponse
    {
        public int totalNumOfListings { get; set; }

        public int pageNumber { get; set; }
        
        public List<HorseListing> listings { get; set; }
    }
}