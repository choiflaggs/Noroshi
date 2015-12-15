using System.Data;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Daos.Rdb
{
    public abstract class AbstractShardDBDao<TPK, TRECORD> : AbstractDao<TPK, TRECORD>
        where TPK : IPrimaryKey
        where TRECORD : class, IRecord
    {

        public bool Create(TRECORD record, uint shardId)
        {
            return _create(_getMasterDbConnection(shardId), record);
        }

        protected override IDbConnection _getMasterDbConnection()
        {
            return _getMasterDbConnection(ContextContainer.GetContext().ShardID.Value);
        }
        protected override IDbConnection _getSlaveDbConnection()
        {
            return _getSlaveDbConnection(ContextContainer.GetContext().ShardID.Value);
        }
        protected IDbConnection _getMasterDbConnection(uint shardId)
        {
            return ContextContainer.GetContext().Cache.MySqlConnection.GetShardConnection(shardId, MySqlConnectionHandler.ConnectionType.Master);
        }
        protected IDbConnection _getSlaveDbConnection(uint shardId)
        {
            return ContextContainer.GetContext().Cache.MySqlConnection.GetShardConnection(shardId, MySqlConnectionHandler.ConnectionType.Slave);
        }
    }
}
