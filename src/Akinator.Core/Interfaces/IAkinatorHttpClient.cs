using System.Threading.Tasks;
using Akinator.Core.Models.AkinatorResponse;

namespace Akinator.Core.Interfaces
{
    internal interface IAkinatorHttpClient
    {
        Task<RegionUrlResponse> GetRegionUrl(string url);

        Task<SessionResponse> GetSession(string url);

        Task<T> GetCallbackResponse<T>(string url);
    }
}
