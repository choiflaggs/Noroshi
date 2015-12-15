using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearEnchantLevelSchema;

namespace Noroshi.Server.Daos.Rdb.Item
{
    public class GearEnchantLevelDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGearID(uint gearId)
        {
            return _select("GearID = @GearID", new {GearID = gearId});
        }
    }
}