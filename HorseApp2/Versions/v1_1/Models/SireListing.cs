using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Models
{
    //One listing of a sire
    [ApiVersion("1.1")]
    public class SireListing
    {
        public long sireServerId { get; set; }
        public string name { get; set; }
        public string horseType { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }
    }
}