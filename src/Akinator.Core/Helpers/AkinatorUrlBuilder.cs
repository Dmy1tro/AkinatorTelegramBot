using System;
using Akinator.Core.Models.Game;

namespace Akinator.Core.Helpers
{
    internal static class AkinatorUrlBuilder
    {
        public static string BaseUrl()
        {
            return $"https://en.akinator.com";
        }

        public static string Session()
        {
            return "https://en.akinator.com/game";
        }

        public static string StartGame(GameState game)
        {
            return $"{game.BaseUrl}/new_session?callback=jQuery331023608747682107778_{DateTime.UtcNow.Ticks}" +
                   $"&urlApiWs={game.RegionUrl.UrlWs}&partner=1&childMod={false}" +
                   $"&player=website-desktop&uid_ext_session={game.Session.UID}" +
                   $"&frontaddr={game.Session.FrontAddr}&constraint=ETAT<>'AV'";
        }

        public static string Answer(GameState game, int answerId)
        {
            return $"{game.BaseUrl}/answer_api?callback=jQuery331023608747682107778_{DateTime.UtcNow.Ticks}" +
                   $"&urlApiWs={game.RegionUrl.UrlWs}&partner=1&childMod={false}" +
                   $"&session={game.Identification.Session}" +
                   $"&signature={game.Identification.Signature}" +
                   $"&step={game.StepInformation.Step}&answer={answerId}&frontaddr={game.Session.FrontAddr}";
        }

        public static string Back(GameState game)
        {
            return $"{game.RegionUrl.UrlWs}/cancel_answer?&callback=jQuery331023608747682107778_{DateTime.UtcNow.Ticks}" +
                   $"&session={game.Identification.Session}" +
                   $"&childMod={false}&signature={game.Identification.Signature}" +
                   $"&step={game.StepInformation.Step}&answer=-1";
        }

        public static string Win(GameState game)
        {
            return $"{game.RegionUrl.UrlWs}/list?callback=jQuery331023608747682107778_{DateTime.UtcNow.Ticks}" +
                   $"&signature={game.Identification.Signature}&step={game.StepInformation.Step}" +
                   $"&session={game.Identification.Session}";
        }
    }
}
