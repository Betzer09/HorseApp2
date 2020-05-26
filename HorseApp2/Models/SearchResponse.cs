using System.Collections.Generic;

namespace HorseApp2.Models
{
    //Response object for searching active listings
    public class SearchResponse
    {
        public int totalNumOfListings { get; set; }

        public int pageNumber { get; set; }
        
        public List<HorseListing> listings { get; set; }
    }
}