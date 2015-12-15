using System.Collections.Generic;
using System.Linq;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaContentSchema;

namespace Noroshi.Server.Daos.Rdb.Gacha
{
    public class GachaContentDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGachaIDs(IEnumerable<uint> gachaIds)
        {
            return _select("GachaID IN @GachaIDs", new { GachaIDs = gachaIds.ToList() });
        }
    }
}
