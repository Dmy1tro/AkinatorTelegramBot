using System.Collections.Concurrent;
using Akinator.Core.Models.Game;

namespace Akinator.Api.Services
{
    internal class GameStorage : IGameStorage
    {
        private static readonly ConcurrentDictionary<string, string> _games = new();

        public void Save(string key, GameSnapshot snapshot)
        {
            _games[key] = snapshot.Serialize();
        }

        public GameSnapshot? Get(string key)
        {
            if (_games.TryGetValue(key, out var snapshot))
            {
                return GameSnapshot.Deserialize(snapshot);
            }

            return null;
        }

        public void Remove(string key)
        {
            _games.TryRemove(key, out _);
        }
    }
}
