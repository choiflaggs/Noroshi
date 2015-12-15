using System;
using Noroshi.Server.Daos.Kvs;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Contexts
{
    public interface IContextCache : IDisposable
    {
        MySqlConnectionHandler MySqlConnection { get; }
        RedisConnectionHandler RedisConnection { get; }
    }
}