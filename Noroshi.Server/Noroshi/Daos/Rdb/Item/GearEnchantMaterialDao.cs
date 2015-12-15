using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearEnchantMaterialSchema;
namespace Noroshi.Server.Daos.Rdb.Item
{
    public class GearEnchantMaterialDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
