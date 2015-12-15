using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerCharacterDao : AbstractShardDBDao<PlayerCharacterDao.PrimaryKey, PlayerCharacterDao.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => "player_character";

        public class Record : AbstractRecord
        {
            public uint ID { get; set; }
            public uint PlayerID { get; set; }
            public uint CharacterID { get; set; }
            public ushort Level { get; set; }
            public uint Exp { get; set; }
            public byte PromotionLevel { get; set; }
            public byte EvolutionLevel { get; set; }
            public ushort ActionLevel1 { get; set; }
            public ushort ActionLevel2 { get; set; }
            public ushort ActionLevel3 { get; set; }
            public ushort ActionLevel4 { get; set; }
            public ushort ActionLevel5 { get; set; }
            public uint Gear1 { get; set; }
            public uint Gear2 { get; set; }
            public uint Gear3 { get; set; }
            public uint Gear4 { get; set; }
            public uint Gear5 { get; set; }
            public uint Gear6 { get; set; }
        }

        public class PrimaryKey : IPrimaryKey
        {
            public uint ID { get; set; }
        }
        /* ここまでテーブルマッピング設定 */

        public IEnumerable<Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID", new { PlayerID = playerId });
        }

        public Record ReadByPlayerIDAndChracterID(uint playerId, uint characterId, ReadType readType)
        {
            return _select("PlayerID = @PlayerID AND CharacterID = @CharacterID", new { PlayerID = playerId, CharacterID = characterId }, readType).FirstOrDefault();
        }
        public IEnumerable<Record> ReadByPlayerIDAndChracterIDs(uint playerId, IEnumerable<uint> characterIds, ReadType readType)
        {
            return _select("PlayerID = @PlayerID AND CharacterID IN @CharacterIDs", new { PlayerID = playerId, CharacterIDs = characterIds }, readType);
        }
        public IEnumerable<Record> ReadByPlayerIDs(IEnumerable<uint> playerIds)
        {
            return _select("PlayerID IN @PlayerIDs", new { PlayerIDs = playerIds });
        }

        public Record Create(uint playerId, uint characterId, byte evolutionLevel)
        {
            return Create(playerId, characterId, evolutionLevel, ContextContainer.GetContext().ShardID.Value);
        }
        public Record Create(uint playerId, uint characterId, byte evolutionLevel, uint shardId)
        {
            var record = new Record
            {
                PlayerID = playerId,
                CharacterID = characterId,
                Level = 1,
                ActionLevel1 = 1,
                ActionLevel2 = 1,
                ActionLevel3 = 1,
                ActionLevel4 = 1,
                ActionLevel5 = 1,
                PromotionLevel = 1,
                EvolutionLevel = evolutionLevel,
                Exp = 0,
                Gear1 = 0,
                Gear2 = 0,
                Gear3 = 0,
                Gear4 = 0,
                Gear5 = 0,
                Gear6 = 0,
            };
            return Create(record, shardId) ? record : null;
        }
    }
}
