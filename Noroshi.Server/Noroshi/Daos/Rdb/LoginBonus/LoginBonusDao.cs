using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.LoginBonusSchema;

namespace Noroshi.Server.Daos.Rdb.LoginBonus
{
    public class LoginBonusDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOpenLoginBonus(uint currentUnixTime)
        {
            return _select("OpenedAt <= @CurrentUnixTime AND @CurrentUnixTime < ClosedAt", new { CurrentUnixTime = currentUnixTime });
        }
    }
}
