using System.Linq;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record ReadByUDID(string udid, ReadType readType)
        {
            return _select("UDID = @UDID", new { UDID = udid }, readType).FirstOrDefault();
        }

        public Schema.Record ReadBySessionID(string sessionId)
        {
            return _select("SessionID = @SessionID", new { SessionID = sessionId }).FirstOrDefault();
        }

        public Schema.Record Create(string udid, string sessionId, uint shardId)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                UDID = udid,
                SessionID = sessionId,
                ShardID = shardId,
            };
            return Create(record);
        }
    }
}
