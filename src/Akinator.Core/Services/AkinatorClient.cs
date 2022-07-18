using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akinator.Core.Exceptions;
using Akinator.Core.Helpers;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.AkinatorResponse;
using Akinator.Core.Models.Game;
using Akinator.Core.Models.Options;
using Newtonsoft.Json;

namespace Akinator.Core.Services
{
    internal class AkinatorClient : IAkinatorClient
    {
        private static readonly Regex _regexRegionUri = new(@"\[{""translated_theme_name"":""[\s\S]*"",""urlWs"":""https:\\\/\\\/srv[0-9]+\.akinator\.com:[0-9]+\\\/ws"",""subject_id"":""[0-9]+""}]");
        private static readonly Regex _regexSession = new("var uid_ext_session = '(.*)';\\n.*var frontaddr = '(.*)';");

        private readonly HttpClient _httpClient;
        private readonly AkinatorOptions _options;

        public AkinatorClient(IHttpClientFactory httpClientFactory, AkinatorOptions options)
        {
            _httpClient = httpClientFactory.CreateClient("akinator");
            _options = options;
        }

        public async Task<IAkinatorGame> StartNewGame()
        {
            var gameState = new GameState();

            gameState.BaseUrl = AkinatorUrlBuilder.BaseUrl(GetLanguage());
            gameState.RegionUrl = await GetRegionUrl(gameState.BaseUrl);
            gameState.Session = await GetSession();

            var url = AkinatorUrlBuilder.StartGame(gameState);
            var startGameResponse = await _httpClient.GetAkinatorCallbackResponse<StartGameResponse>(url);

            gameState.Identification = startGameResponse.Parameters.Identification;
            gameState.StepInformation = startGameResponse.Parameters.StepInformation;

            return new AkinatorGame(_httpClient, gameState);
        }

        private async Task<RegionUrlResponse> GetRegionUrl(string baseUrl)
        {
            var data = await _httpClient.GetStringAsync(baseUrl);
            var res = _regexRegionUri.Match(data);
            var regionUrl = JsonConvert.DeserializeObject<RegionUrlResponse[]>(res.Value);

            return regionUrl.First();
        }

        private async Task<SessionResponse> GetSession()
        {
            var uri = AkinatorUrlBuilder.Session();
            var response = await _httpClient.GetStringAsync(uri);
            var match = _regexSession.Match(response);

            if (match?.Groups?.Count != 3)
            {
                throw new AkinatorException("Failed to parse session.");
            }

            var uid = match.Groups[1].Value;
            var frontaddr = match.Groups[2].Value;

            return new SessionResponse
            {
                UID = uid,
                FrontAddr = frontaddr
            };
        }

        private string GetLanguage() => _options.Region.ToString().ToLower();
    }
}
