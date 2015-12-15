using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShopSchema;

namespace Noroshi.Server.Daos.Rdb.Shop
{
    public class ShopDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadNotResidentShops(ushort playerLevel, ushort vipLevel)
        {
            // 高々数レコードなので index は気にしないクエリ。
            return _select("AppearPlayerLevel <= @PlayerLevel AND @VipLevel < ResidentVipLevel", new { PlayerLevel = playerLevel, VipLevel = vipLevel });
        }

        public IEnumerable<Schema.Record> ReadMoreThanVipLevel(uint vipLevel)
        {
            return _select("ResidentVipLevel > @VipLevel", new { VipLevel = vipLevel });
        }
    }
}
