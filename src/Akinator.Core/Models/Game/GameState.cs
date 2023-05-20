using Akinator.Core.Models.AkinatorResponse;
using Newtonsoft.Json;

namespace Akinator.Core.Models.Game
{
    internal class GameState
    {
        public string BaseUrl { get; set; }

        public RegionUrlResponse RegionUrl { get; set; }

        public SessionResponse Session { get; set; }

        public Identification Identification { get; set; }

        public StepInformation StepInformation { get; set; }

        public GameState Copy()
        {
            return JsonConvert.DeserializeObject<GameState>(JsonConvert.SerializeObject(this));
        }
    }
}
