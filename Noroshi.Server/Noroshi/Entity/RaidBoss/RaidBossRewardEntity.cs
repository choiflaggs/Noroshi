using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.RaidBoss;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RaidBossRewardSchema;

namespace Noroshi.Server.Entity.RaidBoss
{
    public class RaidBossRewardEntity : AbstractDaoWrapperEntity<RaidBossRewardEntity, RaidBossRewardDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<RaidBossRewardEntity> ReadAndBuildByRaidBossIDs(IEnumerable<uint> raidBossIds)
        {
            return _instantiate((new RaidBossRewardDao()).ReadByRaidBossIDs(raidBossIds));
        }

        public uint RaidBossID => _record.RaidBossID;
        public RaidBossRewardCategory Category => (RaidBossRewardCategory)_record.Category;
        public byte No => _record.No;
        public float Probability => _record.Probability;
        public PossessionCategory PossessionCategory => (PossessionCategory)_record.PossessionCategory;
        public uint PossessionID => _record.PossessionID;
        public uint PossessionNum => _record.PossessionNum;

        public PossessionParam GetPossessionParam()
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
