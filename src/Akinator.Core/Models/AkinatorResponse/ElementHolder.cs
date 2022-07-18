using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class ElementHolder
    {
        [JsonProperty("element")]
        public ElementValue Element { get; set; }
    }
}
