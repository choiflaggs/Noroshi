using Noroshi.Server.Entity.PlayerCounter;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerArenaRankEntity : PlayerCounterEntity
    {

        public static uint GenerateRank()
        {
            return new PlayerArenaRankEntity()._generateNumber(PlayerCounterType.ArenaRank);
        }

        public static uint GetLastRank()
        {
            return new PlayerArenaRankEntity()._getLastNumber(PlayerCounterType.ArenaRank);
        }



    }
}
