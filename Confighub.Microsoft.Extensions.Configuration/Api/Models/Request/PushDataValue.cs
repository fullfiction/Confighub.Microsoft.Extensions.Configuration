using Newtonsoft.Json;

namespace Confighub.Microsoft.Extensions.Configuration.Api.Models.Request
{
    public class PushDataValue
    {
        [JsonProperty("context")]
        public string Context { get; set; }
        [JsonProperty("value")]
        public object Value { get; set; }
        [JsonProperty("active")]
        public bool? Active { get; set; }
        [JsonProperty("opp")]
        public string Operation { get; set; }
    }
}