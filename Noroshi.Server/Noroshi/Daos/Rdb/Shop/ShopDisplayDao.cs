using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShopDisplaySchema;

namespace Noroshi.Server.Daos.Rdb.Shop
{
    public class ShopDisplayDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByShopIDs(IEnumerable<uint> shopIds)
        {
            return _select("ShopID IN @ShopIDs", new { ShopIDs = shopIds });
        }
    }
}
