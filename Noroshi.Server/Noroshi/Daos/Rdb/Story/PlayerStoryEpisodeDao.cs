using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerStoryEpisodeSchema;

namespace Noroshi.Server.Daos.Rdb.Story
{
    public class PlayerStoryEpisodeDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => Schema.TableName;

        public Schema.Record ReadByPlayerID(uint playerId, ReadType readType=ReadType.Slave)
        {
            return ReadByPK(new Schema.PrimaryKey { PlayerID = playerId}, readType);
        }

        public Schema.Record CreateOrSelect(uint playerId)
        {
            var record = _create(playerId) ?? ReadByPlayerID(playerId, ReadType.Lock);
            return record;
        }

        Schema.Record _create(uint playerId)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                EpisodeID = 1,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime
            };
            return Create(record);
        }
    }
}