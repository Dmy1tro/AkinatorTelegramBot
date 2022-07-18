using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akinator.Core.Exceptions;
using Newtonsoft.Json;

namespace Akinator.Core.Helpers
{
    internal static class HttpClientExtensions
    {
        private static readonly Regex _regexCallbackResponse = new(@"^jQuery331023608747682107778_\d+\((.+)\)$");

        public static async Task<T> GetAkinatorCallbackResponse<T>(this HttpClient httpClient, string url)
        {
            var data = await httpClient.GetStringAsync(url);
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
