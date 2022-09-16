using System;
using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class StepResponse
    {
        [JsonProperty("completion")]
        public string Completion { get; set; }

        [JsonProperty("parameters")]
        public StepParameters Parameters { get; set; }

        [JsonIgnore]
        public bool IsSuccess => Completion.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
    }
}
