namespace Noroshi.Server.Entity.Ranking
{
    public class GuildRankingDataEntity
    {
        public readonly uint GuildID;
        public readonly uint UniqueRank;
        public readonly uint Rank;
        public readonly int Value;

        public GuildRankingDataEntity(uint guildId, uint uniqueRank, uint rank, int value)
        {
            GuildID = guildId;
            UniqueRank = uniqueRank;
            Rank = rank;
            Value = value;
        }
    }
}
