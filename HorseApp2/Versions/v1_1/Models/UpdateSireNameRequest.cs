using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Models
{
    [ApiVersion("1.1")]
    public class UpdateSireNameRequest
    {
        public string oldName { get; set; }
        public string newName { get; set; }
    }
}