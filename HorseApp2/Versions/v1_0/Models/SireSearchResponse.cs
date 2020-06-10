using System.Collections.Generic;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    [ApiVersion("1.0")]
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