using System.Collections.Generic;
using Newtonsoft.Json;

namespace Akinator.Core.Models.AkinatorResponse
{
    internal class WinParameters
    {
        [JsonProperty("elements")]
        public List<ElementHolder> Elements { get; set; } = new List<ElementHolder>();

        [JsonProperty("NbObjetsPertinents")]
        public string NbObjetsPertinents { get; set; }
    }
}
