using Newtonsoft.Json;

namespace InfoTrack.OAuth
{
    public class AuthServerResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
