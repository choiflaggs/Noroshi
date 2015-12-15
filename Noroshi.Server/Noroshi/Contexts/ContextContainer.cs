using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Contexts
{
    public class ContextContainer
    {
        const string HTTP_CONTEXT_KEY = "ContextContainer";
        static ContextContainer _cliInstance = null;

        static ContextContainer _instance => _cliInstance ?? (ContextContainer)HttpContext.Current.Items[HTTP_CONTEXT_KEY];

        public static void Initialize(WebContext context)
        {
            HttpContext.Current.Items[HTTP_CONTEXT_KEY] = new ContextContainer(context);
            context.Initialize();
        }
        public static void Initialize(CliContext context)
        {
            _cliInstance = new ContextContainer(context);
            context.Initialize();
        }

        public static IContext GetContext()
        {
            return _instance._getContext();
        }
        public static WebContext GetWebContext()
        {
            return (WebContext)_instance._getContext();
        }

        public static void ClearContext()
        {
            if (_instance == null) return;
            _instance._disposeContext();
            if (HttpContext.Current != null && HttpContext.Current.Items.Contains(HTTP_CONTEXT_KEY)) HttpContext.Current.Items.Remove(HTTP_CONTEXT_KEY);
            if (_cliInstance != null) _cliInstance = null;
        }

        // ショートカット
        public static T NoroshiTransaction<T>(Func<TransactionContainer, T> func)
        {
            var connections = new List<MySqlConnection>();
            connections.Add(GetContext().Cache.MySqlConnection.GetConnection(MySqlConnectionHandler.Database.Noroshi, MySqlConnectionHandler.ConnectionType.Master));
            if (GetContext().ShardID.HasValue) connections.Add(GetContext().Cache.MySqlConnection.GetShardConnection(GetContext().ShardID.Value, MySqlConnectionHandler.ConnectionType.Master));

            return GetContext().Cache.MySqlConnection.Transaction<T>(connections, func);
        }


        public static void CommonTransaction(Action<TransactionContainer> action)
        {
            CommonTransaction(tx =>
            {
                action.Invoke(tx);
                return true;
            });
        }
        public static T CommonTransaction<T>(Func<TransactionContainer, T> func)
        {
            var connection = GetContext().Cache.MySqlConnection.GetConnection(MySqlConnectionHandler.Database.Noroshi, MySqlConnectionHandler.ConnectionType.Master);
            return GetContext().Cache.MySqlConnection.Transaction(new[] { connection }, func);
        }

        public static void ShardTransaction(Action<TransactionContainer> action)
        {
            var connections = new List<MySqlConnection>();
            connections.Add(GetContext().Cache.MySqlConnection.GetShardConnection(GetContext().ShardID.Value, MySqlConnectionHandler.ConnectionType.Master));
            GetContext().Cache.MySqlConnection.Transaction(connections, tx =>
            {
                action(tx);
                return true;
            });
        }
        public static T ShardTransaction<T>(Func<TransactionContainer, T> func)
        {
            var connections = new List<MySqlConnection>();
            connections.Add(GetContext().Cache.MySqlConnection.GetShardConnection(GetContext().ShardID.Value, MySqlConnectionHandler.ConnectionType.Master));
            return GetContext().Cache.MySqlConnection.Transaction(connections, func);
        }
        public static void CommonAndShardTransaction(uint shardId, Action<TransactionContainer> action)
        {
            var connections = new List<MySqlConnection>();
            connections.Add(GetContext().Cache.MySqlConnection.GetConnection(MySqlConnectionHandler.Database.Noroshi, MySqlConnectionHandler.ConnectionType.Master));
            connections.Add(GetContext().Cache.MySqlConnection.GetShardConnection(shardId, MySqlConnectionHandler.ConnectionType.Master));
            GetContext().Cache.MySqlConnection.Transaction<bool>(connections, tx =>
            {
                action(tx);
                return true;
            });
        }

        IContext _context;

        ContextContainer(IContext context)
        {
            _context = context;
        }
        IContext _getContext()
        {
            return _context;
        }
        void _disposeContext()
        {
            _context?.Dispose();
        }
    }
}