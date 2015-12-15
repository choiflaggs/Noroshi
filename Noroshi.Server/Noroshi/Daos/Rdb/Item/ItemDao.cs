using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Noroshi.Core.Game.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Daos.Rdb.Item
{
    public class ItemDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record ReadByRaidTicket()
        {
            return _select("Type = @Type", new { Type = PossessionCategory.RaidTicket }).FirstOrDefault();
        }
    }
}
