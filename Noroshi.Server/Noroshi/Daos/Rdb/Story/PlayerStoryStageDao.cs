using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerStoryStageSchema;

namespace Noroshi.Server.Daos.Rdb.Story
{
    public class PlayerStoryStageDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
        public IEnumerable<Schema.Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID", new { PlayerID = playerId });
        }

        public Schema.Record ReadByPlayerIDAndStageID(uint playerId, uint stageId, ReadType readType = ReadType.Slave)
        {
            return _select("PlayerID = @PlayerID AND StageID = @StageID", new { PlayerID = playerId, StageID = stageId }, readType).FirstOrDefault();
        }

        // ギャップロック回避のためロックをかけたい場合には最初に Insert を試みてしまう。
        public Schema.Record CreateOrSelect(uint playerId, uint stageId)
        {
            var record = _create(playerId, stageId);
            return record ?? ReadByPlayerIDAndStageID(playerId, stageId, ReadType.Lock);
        }

        Schema.Record _create(uint playerId, uint stageId)
        {
            var record = new Schema.Record { PlayerID = playerId, StageID = stageId, Rank = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime
            };
            return Create(record);
        }
    }
}