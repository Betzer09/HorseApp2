using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    [ApiVersion("1.0")]
    public class UpdateSireNameRequest
    {
        public string oldName { get; set; }
        public string newName { get; set; }
    }
}