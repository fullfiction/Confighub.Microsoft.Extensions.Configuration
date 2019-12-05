using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Confighub.Microsoft.Extensions.Configuration.Api.Models.Response
{
    public class Pull
    {
        [JsonProperty("generatedOn")]
        public DateTime GeneratedOn { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("repo")]
        public string Repository { get; set; }
        [JsonProperty("context")]
        public string Context { get; set; }
        [JsonProperty("properties")]
        public Dictionary<string, PullProperty> Properties { get; set; }
    }
}