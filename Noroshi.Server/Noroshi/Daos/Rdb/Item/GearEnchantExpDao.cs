using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearEnchantExpSchema;

namespace Noroshi.Server.Daos.Rdb.Item
{
    public class GearEnchantExpDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => Schema.TableName;
    }
}