using Schema = Noroshi.Server.Daos.Rdb.Schemas.ActionSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class ActionDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
