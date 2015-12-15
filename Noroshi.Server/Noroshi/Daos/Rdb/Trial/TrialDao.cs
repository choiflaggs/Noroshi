using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrialSchema;

namespace Noroshi.Server.Daos.Rdb.Trial
{
    public class TrialDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
