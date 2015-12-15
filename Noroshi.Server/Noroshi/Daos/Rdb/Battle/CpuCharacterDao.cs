using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuCharacterSchema;

namespace Noroshi.Server.Daos.Rdb.Battle
{
    public class CpuCharacterDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
