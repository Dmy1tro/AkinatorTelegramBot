using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class Answer
    {
        [JsonProperty("answer")]
        public string Text { get; set; }
    }
}
