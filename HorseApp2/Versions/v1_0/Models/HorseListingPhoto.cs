using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    //One horse listing photo
    [ApiVersion("1.0")]
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
        public bool isVideo { get; set; }
    }
}