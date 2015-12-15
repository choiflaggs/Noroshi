using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterActionSequenceSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class CharacterActionSequenceDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}
