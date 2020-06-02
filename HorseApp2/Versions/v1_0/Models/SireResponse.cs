using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    [ApiVersion("1.0")]
    public class SireResponse
    {
        public long sireServerId { get; set; }
        public string name { get; set; }
        public string horseType { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }

    }
}