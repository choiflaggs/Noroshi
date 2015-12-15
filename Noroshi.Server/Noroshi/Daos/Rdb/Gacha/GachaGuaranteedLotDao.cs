using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaGuaranteedLotSchema;

namespace Noroshi.Server.Daos.Rdb.Gacha
{
    public class GachaGuaranteedLotDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGachaID(uint gachaId)
        {
            return _select("GachaID = @GachaID", new { GachaID = gachaId });
        }
    }
}
