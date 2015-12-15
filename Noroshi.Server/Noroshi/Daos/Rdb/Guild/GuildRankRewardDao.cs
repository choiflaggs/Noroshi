using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRankRewardSchema;

namespace Noroshi.Server.Daos.Rdb.Guild
{
    public class GuildRankRewardDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
