using System.Collections.Generic;
using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class StepInformation
    {
        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("answers")]
        public List<AnswerResponse> Answers { get; set; }

        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("progression")]
        public string Progression { get; set; }

        [JsonProperty("questionid")]
        public string Questionid { get; set; }

        [JsonProperty("infogain")]
        public string Infogain { get; set; }
    }
}
