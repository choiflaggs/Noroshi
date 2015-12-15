using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShadowCharacterSchema;

namespace Noroshi.Server.Daos.Rdb
{
    public class ShadowCharacterDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
