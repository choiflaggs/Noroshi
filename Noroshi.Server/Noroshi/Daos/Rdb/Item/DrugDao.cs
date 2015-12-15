using Schema = Noroshi.Server.Daos.Rdb.Schemas.DrugSchema;
namespace Noroshi.Server.Daos.Rdb.Item
{
    public class DrugDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
