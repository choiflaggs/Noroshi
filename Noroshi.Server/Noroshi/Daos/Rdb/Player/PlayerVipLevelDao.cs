using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerVipLevelSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerVipLevelDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
