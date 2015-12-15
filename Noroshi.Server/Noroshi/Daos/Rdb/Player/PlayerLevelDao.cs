using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerLevelSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerLevelDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
