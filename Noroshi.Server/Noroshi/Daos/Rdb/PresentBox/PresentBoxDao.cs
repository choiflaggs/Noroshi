using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PresentBoxSchema;

namespace Noroshi.Server.Daos.Rdb.PresentBox
{
    public class PresentBoxDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID ORDER BY CreatedAt DESC", new { PlayerID = playerId });
        }

        public Schema.Record Create(uint playerId, string possessionData, string textId, string textData)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
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
