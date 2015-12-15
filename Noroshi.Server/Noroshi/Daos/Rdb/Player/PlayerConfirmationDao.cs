using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerConfirmationSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerConfirmationDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record CreateOrRead(uint playerId, byte referenceId)
        {
            var defaultRecord = new Schema.Record
            {
                PlayerID = playerId,
                ReferenceID = referenceId,
                ConfirmedAt = 0,
            };
            var record = Create(defaultRecord);
            if (record != null) return record;
            return ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, ReferenceID = referenceId }, ReadType.Lock);
        }
    }
}
