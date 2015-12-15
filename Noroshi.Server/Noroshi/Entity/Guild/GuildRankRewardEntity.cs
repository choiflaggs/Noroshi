using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Guild;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRankRewardSchema;

namespace Noroshi.Server.Entity.Guild
{
    public class GuildRankRewardEntity : AbstractDaoWrapperEntity<GuildRankRewardEntity, GuildRankRewardDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GuildRankRewardEntity> ReadAndBuildAll()
        {
            return _instantiate((new GuildRankRewardDao()).ReadAll());
        }

        public GuildRank GuildRank => (GuildRank)_record.GuildRank;
        /// <summary>
        /// 番号（ユニーク制約、表示順のために利用しているだけ）。
        /// </summary>
        public byte No => _record.No;

        public PossessionParam GetPossessionParam()
        {
            return new PossessionParam
            {
                Category = (PossessionCategory)_record.PossessionCategory,
                ID = _record.PossessionID,
                Num = _record.PossessionNum,
            };
        }
    }
}
