using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterGearSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class CharacterGearDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByCharacterIDs(IEnumerable<uint> characterIds)
        {
            return _select("CharacterID IN @CharacterIDs", new { CharacterIDs = characterIds });
        }
    }
}
