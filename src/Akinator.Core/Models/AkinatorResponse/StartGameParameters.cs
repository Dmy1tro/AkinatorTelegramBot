using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class StartGameParameters
    {
        [JsonProperty("identification")]
        public Identification Identification { get; set; }

        [JsonProperty("step_information")]
        public StepInformation StepInformation { get; set; }
    }
}
