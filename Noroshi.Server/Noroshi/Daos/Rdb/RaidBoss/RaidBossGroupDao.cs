using Schema = Noroshi.Server.Daos.Rdb.Schemas.RaidBossGroupSchema;

namespace Noroshi.Server.Daos.Rdb.RaidBoss
{
    public class RaidBossGroupDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
