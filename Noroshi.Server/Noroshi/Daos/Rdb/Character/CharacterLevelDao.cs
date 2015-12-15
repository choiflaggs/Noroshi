using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterLevelSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class CharacterLevelDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
