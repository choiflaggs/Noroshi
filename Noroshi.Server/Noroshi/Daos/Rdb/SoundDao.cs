using Schema = Noroshi.Server.Daos.Rdb.Schemas.SoundSchema;

namespace Noroshi.Server.Daos.Rdb
{
    public class SoundDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
