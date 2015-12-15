using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Quest;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.QuestRewardSchema;

namespace Noroshi.Server.Entity.Quest
{
    public class QuestRewardEntity : AbstractDaoWrapperEntity<QuestRewardEntity, QuestRewardDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<QuestRewardEntity> ReadAndBuildByQuestIDs(IEnumerable<uint> questIds)
        {
            return (new QuestRewardDao()).ReadByQuestIDs(questIds).Select(r => _instantiate(r));
        }

        public uint QuestID => _record.QuestID;
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
