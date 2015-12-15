using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DynamicTextSchema;

namespace Noroshi.Server.Daos.Rdb.FrameWork
{
    public class DynamicTextDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByLanguageID(uint languageId)
        {
            return _select("LanguageID = @LanguageID", new { LanguageID = languageId });
        }
        public IEnumerable<Schema.Record> ReadByLanguageIDAndTextKeys(uint languageId, IEnumerable<string> textKeys)
        {
            return _select("LanguageID = @LanguageID AND TextKey IN @TextKeys", new { LanguageID = languageId, TextKeys = textKeys });
        }
    }
}
