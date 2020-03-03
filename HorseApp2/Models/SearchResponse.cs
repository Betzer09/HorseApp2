using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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