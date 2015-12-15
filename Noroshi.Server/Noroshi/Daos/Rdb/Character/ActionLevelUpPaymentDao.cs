using Schema = Noroshi.Server.Daos.Rdb.Schemas.ActionLevelUpPaymentSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class ActionLevelUpPaymentDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
