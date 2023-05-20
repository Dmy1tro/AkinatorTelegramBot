using Akinator.Core.Models.Game;

namespace Akinator.Api.Services
{
    internal interface IGameStorage
    {
        void Save(string key, GameSnapshot snapshot);

        GameSnapshot? Get(string key);

        void Remove(string key);
    }
}
