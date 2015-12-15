using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaEntryPointSchema;

namespace Noroshi.Server.Daos.Rdb.Gacha
{
    public class GachaEntryPointDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
