using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseApp2.Models
{
    //One horse listing photo
    public class HorseListingPhoto
    {
        public HorseListingPhoto()
        {

        }
        public long activeListingPhotoId { get; set; }
        public string activeListingId { get; set; }
        public string photoUrl { get; set; }
        public int photoOrder { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }
    }
}