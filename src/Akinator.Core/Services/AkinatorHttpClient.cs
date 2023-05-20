using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akinator.Core.Exceptions;
using Akinator.Core.Interfaces;
using Akinator.Core.Models.AkinatorResponse;
using Newtonsoft.Json;

namespace Akinator.Core.Services
{
    internal class AkinatorHttpClient : IAkinatorHttpClient
    {
        private static readonly Regex _regexRegionUri = new(@"\[{""translated_theme_name"":""[\s\S]*"",""urlWs"":""https:\\\/\\\/srv[0-9]+\.akinator\.com:[0-9]+\\\/ws"",""subject_id"":""[0-9]+""}]", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex _regexSession = new("var uid_ext_session = '(.*)';\\n.*var frontaddr = '(.*)';", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex _regexCallbackResponse = new(@"jQuery\d+_\d+\(([\s\S]*?)\)$", RegexOptions.Compiled, TimeSpan.FromSeconds(5));

        private readonly HttpClient _httpClient;

        public AkinatorHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("akinator");
        }
        public async Task<RegionUrlResponse> GetRegionUrl(string url)
        {
            var data = await _httpClient.GetStringAsync(url);
            var res = _regexRegionUri.Match(data);
            var regionUrl = JsonConvert.DeserializeObject<RegionUrlResponse[]>(res.Value);

            return regionUrl.First();
        }

        public async Task<SessionResponse> GetSession(string url)
        {
            var response = await _httpClient.GetStringAsync(url);
            var match = _regexSession.Match(response);

            if (match?.Groups?.Count != 3)
            {
                throw new AkinatorException("Failed to get session.");
            }

            var uid = match.Groups[1].Value;
            var frontaddr = match.Groups[2].Value;

            return new SessionResponse
            {
                UID = uid,
                FrontAddr = frontaddr
            };
        }

        public async Task<T> GetCallbackResponse<T>(string url)
        {
            var data = await _httpClient.GetStringAsync(url);
            var match = _regexCallbackResponse.Match(data);

            if (match.Groups.Count != 2)
            {
                throw new AkinatorException("Failed to parse callback repsonse.");
            }

            var response = JsonConvert.DeserializeObject<T>(match.Groups[1].Value);

            return response;
        }
    }
}
