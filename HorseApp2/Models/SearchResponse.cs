using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    public class SearchResponse
    {
        public int totalNumOfListings { get; set; }
        
        public List<HorseListing> listings { get; set; }
    }
}