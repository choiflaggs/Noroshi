using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearRecipeSchema;

namespace Noroshi.Server.Daos.Rdb.Item
{
    public class GearRecipeDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
        public IEnumerable<Schema.Record> ReadByCraftItemID(uint craftItemId)
        {
            return _select("CraftItemID = @CraftItemID", new { CraftItemID = craftItemId });
        }
    }
}