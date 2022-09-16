using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class AnswerResponse
    {
        [JsonProperty("answer")]
        public string Text { get; set; }
    }
}
