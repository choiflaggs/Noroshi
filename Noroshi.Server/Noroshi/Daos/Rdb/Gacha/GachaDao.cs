using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaSchema;

namespace Noroshi.Server.Daos.Rdb.Gacha
{
    public class GachaDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
