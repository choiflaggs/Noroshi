using System.Collections.Generic;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Daos.Rdb.Quest
{
    public class PlayerQuestTriggerDao : AbstractShardDBDao<PlayerQuestTriggerDao.PrimaryKey, PlayerQuestTriggerDao.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => "player_quest_trigger";

        public class Record : AbstractRecord
        {
            public uint PlayerID { get; set; }
            public uint TriggerID { get; set; }
            public uint CurrentNum { get; set; }
            public uint ReceiveRewardThreshold { get; set; }
            public uint CreatedAt { get; set; }
            public uint UpdatedAt { get; set; }
        }

        public class PrimaryKey : IPrimaryKey
        {
            public uint PlayerID { get; set; }
            public uint TriggerID { get; set; }
        }
        /* ここまでテーブルマッピング設定 */

        public IEnumerable<Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID", new { PlayerID = playerId });
        }

        public Record Create(uint playerId, uint triggerId, uint currentNum)
        {
            var record = new Record
            {
                PlayerID = playerId,
                TriggerID = triggerId,
                CurrentNum = currentNum,
                ReceiveRewardThreshold = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
            return Create(record);
        }
    }
}
