using Akinator.Core.Models.AkinatorResponse;

namespace Akinator.Core.Models.Game
{
    internal class GameState
    {
        public string BaseUrl { get; set; }

        public RegionUrlResponse RegionUrl { get; set; }

        public SessionResponse Session { get; set; }

        public Identification Identification { get; set; }

        public StepInformation StepInformation { get; set; }
    }
}
