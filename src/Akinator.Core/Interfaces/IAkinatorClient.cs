using System.Threading.Tasks;

namespace Akinator.Core.Interfaces
{
    public interface IAkinatorClient
    {
        Task<IAkinatorGame> StartNewGame();
    }
}
