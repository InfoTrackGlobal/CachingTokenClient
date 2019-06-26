using Newtonsoft.Json;

namespace InfoTrack.Authentication
{
    public class AuthServerResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
