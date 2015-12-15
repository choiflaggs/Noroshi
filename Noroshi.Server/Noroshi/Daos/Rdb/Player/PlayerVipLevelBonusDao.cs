using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerVipLevelBonusSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerVipLevelBonusDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
