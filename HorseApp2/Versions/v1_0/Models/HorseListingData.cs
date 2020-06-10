using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    //Contains all the necessary fields to build a horse listing besides the photos
    [ApiVersion("1.0")]
    public class HorseListingData
    {
        public long ActiveListingId { get; set; }
        public string Age { get; set; }
        public string Color { get; set; }
        public string Dam { get; set; }
        public string Sire { get; set; }
        public string DamSire { get; set; }
        public string Description { get; set; }
        public string FireBaseId { get; set; }
        public string Gender { get; set; }
        public string HorseName { get; set; }
        public bool InFoal { get; set; }
        public decimal Lte { get; set; }
        public string OriginalDateListed { get; set; }
        public decimal Price { get; set; }
        public string PurchaseListingType { get; set; }
        public string RanchPhoto { get; set; }
        public string SellerId { get; set; }
        public string HorseType { get; set; }
    }
}