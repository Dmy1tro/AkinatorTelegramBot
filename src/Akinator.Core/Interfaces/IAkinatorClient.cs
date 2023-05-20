using System.Threading.Tasks;
using Akinator.Core.Models.Game;

namespace Akinator.Core.Interfaces
{
    public interface IAkinatorClient
    {
        Task<IAkinatorGame> StartNewGame();

        IAkinatorGame LoadGameFromSnapshot(GameSnapshot snapshot);
    }
}
