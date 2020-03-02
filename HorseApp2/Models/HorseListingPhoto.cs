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
        public long ActiveListingPhotoId { get; set; }
        public string ActiveListingId { get; set; }
        public string PhotoURL { get; set; }
        public int PhotoOrder { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
    }
}