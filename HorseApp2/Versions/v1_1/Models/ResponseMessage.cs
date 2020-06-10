using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Models
{
    [ApiVersion("1.1")]
    public class ResponseMessage
    {
        public string Message { get; set; }
    }
}