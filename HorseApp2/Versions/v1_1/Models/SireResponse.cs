using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Models
{
    [ApiVersion("1.1")]
    public class SireResponse
    {
        public long sireServerId { get; set; }
        public string name { get; set; }
        public string horseType { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }

    }
}