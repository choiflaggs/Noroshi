using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DailyQuestRewardSchema;
using Noroshi.Server.Daos.Rdb.Quest;

namespace Noroshi.Server.Entity.Quest
{
    public class DailyQuestRewardEntity : AbstractDaoWrapperEntity<DailyQuestRewardEntity, DailyQuestRewardDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<DailyQuestRewardEntity> ReadAndBuildByDailyQuestIDs(IEnumerable<uint> dailyQuestIds)
        {
            return (new DailyQuestRewardDao()).ReadByDailyQuestIDs(dailyQuestIds).Select(r => _instantiate(r));
        }

        public uint DailyQuestID => _record.DailyQuestID;
        public byte No => _record.No;
        public PossessionCategory PossessionCategory => (PossessionCategory)_record.PossessionCategory;
        public uint PossessionID => _record.PossessionID;
        public uint PossessionNum => _record.PossessionNum;

        public PossessionParam GetPossessableParam()
        {
            return new PossessionParam
            {
                Category = PossessionCategory,
                ID = PossessionID,
                Num = PossessionNum,
            };
        }
    }
}
