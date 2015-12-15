using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PresentBoxReceivedLogSchema;

namespace Noroshi.Server.Daos.Rdb.PresentBox
{
    public class PresentBoxReceivedLogDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID ORDER BY CreatedAt DESC", new { PlayerID = playerId });
        }

        public Schema.Record Create(uint id, uint playerId, string possessionData, string textId, string textData)
        {
            var record = new Schema.Record
            {
                ID = id,
                PlayerID = playerId,
                PossessionData = possessionData,
                TextID = textId,
                TextData = textData,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
            return Create(record);
        }

    }
}
