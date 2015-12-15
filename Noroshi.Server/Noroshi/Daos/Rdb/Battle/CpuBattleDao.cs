using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleSchema;

namespace Noroshi.Server.Daos.Rdb.Battle
{
    public class CpuBattleDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
