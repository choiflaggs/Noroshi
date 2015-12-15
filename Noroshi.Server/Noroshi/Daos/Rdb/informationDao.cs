using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.InformationSchema;

namespace Noroshi.Server.Daos.Rdb.information
{
    public class InformationDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOpenInformation(uint currentUnixTime)
        {
            return _select("OpenedAt <= @CurrentUnixTime AND @CurrentUnixTime < ClosedAt", new { CurrentUnixTime = currentUnixTime });
        }
    }
}
