using System.Collections.Generic;
using Newtonsoft.Json;

namespace Confighub.Microsoft.Extensions.Configuration.Api.Models.Request
{
    public class PushData
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("opp")]
        public string Operation { get; set; }
        [JsonProperty("readme")]
        public string ReadMe { get; set; }
        [JsonProperty("deprecated")]
        public bool? Deprecated { get; set; }
        [JsonProperty("vdt")]
        public string Type { get; set; }
        [JsonProperty("push")]
        public bool? Push { get; set; }
        [JsonProperty("securityGroup")]
        public string SecurityGroup { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("values")]
        public List<PushDataValue> Values { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return ((PushData)obj).Key.Equals(Key);
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }
}