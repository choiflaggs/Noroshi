using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterEffectSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class CharacterEffectDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
