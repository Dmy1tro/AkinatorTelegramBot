using Newtonsoft.Json;
using System.Collections.Generic;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class StepParameters
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

        [JsonProperty("status_minibase")]
        public string StatusMinibase { get; set; }

        [JsonProperty("options")]
        public List<object> Options { get; set; }

        public StepInformation ToStepInformation()
        {
            return new StepInformation
            {
                Answers = Answers,
                Infogain = Infogain,
                Progression = Progression,
                Question = Question,
                Questionid = Questionid,
                Step = Step
            };
        }
    }
}
