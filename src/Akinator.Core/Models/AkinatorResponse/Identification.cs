using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class Identification
    {
        [JsonProperty("channel")]
        public int Channel { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("challenge_auth")]
        public string ChallengeAuth { get; set; }
    }
}
