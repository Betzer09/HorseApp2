using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Models
{
    [ApiVersion("1.0")]
    public class ResponseMessage
    {
        public string Message { get; set; }
    }
}