using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    //1 horse listing that includes the photos for the post
    public class HorseListing
    {
        public HorseListing()
        {
            photos = new List<HorseListingPhoto>();
        }

        public string activeListingId { get; set; }
        public int age { get; set; }
        public string color { get; set; }
        public string dam { get; set; }
        public string sire { get; set; }
        public string damSire { get; set; }
        public string description { get; set; }
        //public string fireBaseId { get; set; }
        public string gender { get; set; }
        public string horseName { get; set; }
        public bool inFoal { get; set; }
        public decimal lte { get; set; }
        public string originalDateListed { get; set; }
        public decimal price { get; set; }
        public string purchaseListingType { get; set; }
        public string ranchPhoto { get; set; }
        public string sellerId { get; set; }
        public string horseType { get; set; }
        public bool isSold { get; set; }

        public List<HorseListingPhoto> photos { get; set; }

    }
}