using System.Linq;
using MySql.Data.MySqlClient;
using Dapper;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Daos.Rdb
{
    public class SequentialIDTable
    {
        string _targetTable;

        public SequentialIDTable(string targetTable)
        {
            _targetTable = targetTable;
        }

        public uint GenerateID()
        {
            var connection = _getDbConnection();
            connection.Execute($"UPDATE sequential_id_{_targetTable} SET ID = LAST_INSERT_ID(ID + 1)");

            var row = connection.Query($"SELECT LAST_INSERT_ID() AS ID").First();
            return (uint)row.ID;
        }
        public uint[] GenerateIDs(int num)
        {
            // TODO : 最適化。
            return Enumerable.Range(0, num).Select(_ => GenerateID()).ToArray();
        }

        MySqlConnection _getDbConnection()
        {
            return ContextContainer.GetContext().Cache.MySqlConnection.GetConnection(MySqlConnectionHandler.Database.Noroshi, MySqlConnectionHandler.ConnectionType.Master);
        }
    }
}