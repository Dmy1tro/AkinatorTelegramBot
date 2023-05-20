using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Akinator.Core.Models.Game
{
    // Give opportunity to save and load game but keep all internall properties hidden.
    public class GameSnapshot
    {
        internal GameSnapshot(GameState state)
        {
            SnapshotId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            GameState = state;
        }

        internal GameSnapshot()
        {
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(new
            {
                SnapshotId = SnapshotId,
                CreatedAt = CreatedAt,
                GameState = GameState
            }, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public static GameSnapshot Deserialize(string snapshotJson)
        {
            var obj = JObject.Parse(snapshotJson);

            return new GameSnapshot
            {
                SnapshotId = obj["snapshotId"].ToObject<Guid>(),
                CreatedAt = obj["createdAt"].ToObject<DateTime>(),
                GameState = obj["gameState"].ToObject<GameState>()
            };
        }

        public Guid SnapshotId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        internal GameState GameState { get; private set; }
    }
}
