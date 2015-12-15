using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerBattleSessionSchema;

namespace Noroshi.Server.Daos.Rdb.Battle
{
    public class PlayerBattleSessionDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
