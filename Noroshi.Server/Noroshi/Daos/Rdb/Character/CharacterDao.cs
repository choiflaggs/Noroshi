using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class CharacterDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
