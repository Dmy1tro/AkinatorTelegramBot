using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class StartGameResponse
    {
        [JsonProperty("completion")]
        public string Completion { get; set; }

        [JsonProperty("parameters")]
        public StartGameParameters Parameters { get; set; }
    }
}
