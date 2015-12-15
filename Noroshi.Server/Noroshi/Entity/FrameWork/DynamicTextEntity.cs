using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.FrameWork;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DynamicTextSchema;

namespace Noroshi.Server.Entity.FrameWork
{
    public class DynamicTextEntity : AbstractDaoWrapperEntity<DynamicTextEntity, DynamicTextDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<DynamicTextEntity> ReadAndBuildByLanguageID(uint languageId)
        {
            return _instantiate((new DynamicTextDao()).ReadByLanguageID(languageId));
        }
        public static IEnumerable<DynamicTextEntity> ReadAndBuildByLanguageIDAndTextKeys(uint languageId, IEnumerable<string> textKeys)
        {
            return _instantiate((new DynamicTextDao()).ReadByLanguageIDAndTextKeys(languageId, textKeys));
        }

        public string TextKey => _record.TextKey;
        public string Text => _record.Text;


        public Core.WebApi.Response.Master.DynamicText ToResponseData()
        {
            return new Core.WebApi.Response.Master.DynamicText
            {
                ID = TextKey,
                Text = Text,
            };
        }
    }
}
