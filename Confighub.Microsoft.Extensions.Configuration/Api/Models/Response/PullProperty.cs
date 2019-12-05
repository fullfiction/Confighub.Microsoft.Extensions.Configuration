
using Newtonsoft.Json;

namespace Confighub.Microsoft.Extensions.Configuration.Api.Models.Response
{
    public class PullProperty
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("val")]
        public dynamic Value { get; set; }
    }
}