using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Gacha;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaGuaranteedLotSchema;

namespace Noroshi.Server.Entity.Gacha
{
    public class GachaGuaranteedLotEntity : AbstractDaoWrapperEntity<GachaGuaranteedLotEntity, GachaGuaranteedLotDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GachaGuaranteedLotEntity> ReadAndBuildByGachaID(uint gachaId)
        {
            return _instantiate((new GachaGuaranteedLotDao()).ReadByGachaID(gachaId));
        }

        public uint HitNum => _record.HitNum;
        public uint MissLotNum => _record.MissLotNum;
        public PossessionCategory GuaranteedPossessionCategory => (PossessionCategory)_record.GuaranteedPossessionCategory;
    }
}
