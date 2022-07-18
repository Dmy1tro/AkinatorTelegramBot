using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class WinResponse
    {
        [JsonProperty("completion")]
        public string Completion { get; set; }

        [JsonProperty("parameters")]
        public WinParameters Parameters { get; set; }
    }
}
