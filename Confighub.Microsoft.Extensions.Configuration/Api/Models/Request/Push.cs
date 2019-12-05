using Newtonsoft.Json;
using System.Collections.Generic;

namespace Confighub.Microsoft.Extensions.Configuration.Api.Models.Request
{
    public class Push
    {
        [JsonProperty("changeComment")]
        public string ChangeComment { get; set; }
        [JsonProperty("enableKeyCreation")]
        public bool EnableKeyCreation { get; set; }
        [JsonProperty("data")]
        public List<PushData> Data { get; set; }
    }
}