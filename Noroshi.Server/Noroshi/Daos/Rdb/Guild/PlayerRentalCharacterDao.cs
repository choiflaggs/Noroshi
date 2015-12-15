using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerRentalCharacterSchema;

namespace Noroshi.Server.Daos.Rdb.Guild
{
    public class PlayerRentalCharacterDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByPlayerIDs(IEnumerable<uint> playerIds)
        {
            return _select("PlayerID IN @PlayerIDs", new { PlayerIDs = playerIds });
        }
        public IEnumerable<Schema.Record> ReadByPlayerCharacterIDs(IEnumerable<uint> playerCharacterIds, ReadType readType = ReadType.Slave)
        {
            return _select("PlayerCharacterID IN @PlayerCharacterIDs", new { PlayerCharacterIDs = playerCharacterIds }, readType);
        }

        public Schema.Record Create(uint playerId, byte no, uint playerCharacterId)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                No = no,
                PlayerCharacterID = playerCharacterId,
                RentalNum = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
            return Create(record);
        }
    }
}
