using System.Collections.Generic;
using System.Linq;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.SoulSchema;
namespace Noroshi.Server.Daos.Rdb.Item
{
    public class SoulDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByCharacterIDs(IEnumerable<uint> characterIds)
        {
            return _select("CharacterID IN @CharacterIDs", new { CharacterIDs = characterIds.ToList() });
        }
    }
}
