using Schema = Noroshi.Server.Daos.Rdb.Schemas.AttributeSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class AttributeDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
