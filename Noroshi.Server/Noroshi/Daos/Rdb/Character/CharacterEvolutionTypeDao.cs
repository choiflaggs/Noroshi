using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterEvolutionTypeSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class CharacterEvolutionTypeDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => Schema.TableName;


        public IEnumerable<Schema.Record> ReadByType(ushort type)
        {
            return _select("Type = @Type", new {Type = type});
        }
    }
}