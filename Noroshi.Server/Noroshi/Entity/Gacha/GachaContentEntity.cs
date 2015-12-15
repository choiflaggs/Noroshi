using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Gacha;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaContentSchema;

namespace Noroshi.Server.Entity.Gacha
{
    public class GachaContentEntity : AbstractDaoWrapperEntity<GachaContentEntity, GachaContentDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GachaContentEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }));
        }
        public static IEnumerable<GachaContentEntity> ReadAndBuildMultiByGachaIDs(IEnumerable<uint> gachaIds)
        {
            var dao = new GachaContentDao();
            return dao.ReadByGachaIDs(gachaIds).Select(r => _instantiate(r));
        }

        public uint ID => _record.ID;
        public uint GachaID => _record.GachaID;
        public uint Weight => _record.Weight;

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
