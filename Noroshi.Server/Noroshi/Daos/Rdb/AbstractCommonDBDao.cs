using System.Data;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Daos.Rdb
{
    public abstract class AbstractCommonDBDao<TPK, TRECORD> : AbstractDao<TPK, TRECORD>
        where TPK : IPrimaryKey
        where TRECORD : class, IRecord
    {
        protected override IDbConnection _getMasterDbConnection()
        {
            return ContextContainer.GetContext().Cache.MySqlConnection.GetConnection(MySqlConnectionHandler.Database.Noroshi, MySqlConnectionHandler.ConnectionType.Master);
        }
        protected override IDbConnection _getSlaveDbConnection()
        {
            return ContextContainer.GetContext().Cache.MySqlConnection.GetConnection(MySqlConnectionHandler.Database.Noroshi, MySqlConnectionHandler.ConnectionType.Slave);
        }
    }
}
