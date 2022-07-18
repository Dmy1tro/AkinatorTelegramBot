using System.Collections.Concurrent;
using Akinator.Core.Interfaces;

namespace Akinator.Api.Services
{
    internal class GameStore
    {
        private readonly ConcurrentDictionary<string, IAkinatorGame> _games = new();

        public void Add(string key, IAkinatorGame game)
        {
            _games[key] = game;
        }

        public IAkinatorGame? Get(string key)
        {
            if (_games.TryGetValue(key, out var game))
            {
                return game;
            }

            return null;
        }

        public void Remove(string key)
        {
            _games.TryRemove(key, out _);
        }
    }
}
