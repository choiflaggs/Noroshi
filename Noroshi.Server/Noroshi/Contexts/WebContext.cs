using System;
using Noroshi.Server.Entity.Player;

namespace Noroshi.Server.Contexts
{
    public class WebContext : AbstractContext
    {
        public PlayerEntity Player;

        public bool LoadPlayer(string sessionId)
        {
            Player = PlayerEntity.ReadAndBuildBySessionID(sessionId);
            if (Player != null) ShardID = Player.ShardID;
            return Player != null;
        }

        public void SetShardID(uint shardId)
        {
            ShardID = shardId;
        }
        
    }
}
