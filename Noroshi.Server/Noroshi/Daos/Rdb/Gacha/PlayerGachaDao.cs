using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerGachaSchema;

namespace Noroshi.Server.Daos.Rdb.Gacha
{
    public class PlayerGachaDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record CreateOrRead(uint playerId, uint gachaId)
        {
            var record = Create(GetDefaultRecord(playerId, gachaId));
            return record ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, GachaID = gachaId }, ReadType.Lock);
        }

        public Schema.Record GetDefaultRecord(uint playerId, uint gachaId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                GachaID = gachaId,
                HitNum = 0,
                MissLotNum = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
