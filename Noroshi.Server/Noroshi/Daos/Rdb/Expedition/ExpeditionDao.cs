using Schema = Noroshi.Server.Daos.Rdb.Schemas.ExpeditionSchema;

namespace Noroshi.Server.Daos.Rdb.Expedition
{
    public class ExpeditionDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
