using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    //Response object for searching sires
    public class SireSearchResponse
    {
        public SireSearchResponse()
        {
            sires = new List<SireListing>();
        }
        public List<SireListing> sires { get; set; }
        public long totalResultCount { get; set; }

        public  int pageCount { get; set; }
    }
}