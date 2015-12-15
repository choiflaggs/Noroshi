using Noroshi.Server.Daos.Kvs;
namespace Noroshi.Server.Contexts
{
    public class Cache
    {
        public Daos.Rdb.MySqlConnectionHandler MySqlConnection { get; }

        public Cache()
        {
            MySqlConnection = new Daos.Rdb.MySqlConnectionHandler();
        }

        public void ClearRdbConnections()
        {
            MySqlConnection.Clear();
        }

        public void Dispose()
        {
            MySqlConnection.Dispose();
        }
    }
}
