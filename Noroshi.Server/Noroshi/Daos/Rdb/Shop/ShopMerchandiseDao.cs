using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShopMerchandiseSchema;

namespace Noroshi.Server.Daos.Rdb.Shop
{
    public class ShopMerchandiseDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByMerchandiseGroupIDs(IEnumerable<uint> merchandiseGroupIds)
        {
            return _select("MerchandiseGroupID IN @MerchandiseGroupIDs", new { MerchandiseGroupIDs = merchandiseGroupIds });
        }
    }
}
