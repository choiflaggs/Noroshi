using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearSchema;
namespace Noroshi.Server.Daos.Rdb.Item
{
    public class GearDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => Schema.TableName;
    }
}
