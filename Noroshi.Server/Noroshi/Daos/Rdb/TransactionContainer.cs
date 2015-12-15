using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MySql.Data.MySqlClient;

namespace Noroshi.Server.Daos.Rdb
{
    public class TransactionContainer : IDisposable
    {
        const IsolationLevel ISOLATION_LEVEL = IsolationLevel.RepeatableRead;


        MySqlTransaction[] _mySqlTransactions;
        List<Action> _afterCommitActions = new List<Action>();

        public TransactionContainer(IEnumerable<MySqlConnection> connections)
        {
            _mySqlTransactions = connections.Select(connection => connection.BeginTransaction(ISOLATION_LEVEL)).ToArray();
        }

        public void AddAfterCommitAction(Action action)
        {
            _afterCommitActions.Add(action);
        }

        public void Commit()
        {
            foreach (var mySqlTransaction in _mySqlTransactions)
            {
                mySqlTransaction.Commit();
            }
            foreach (var action in _afterCommitActions)
            {
                action.Invoke();
            }
            _afterCommitActions.Clear();
        }

        public void Rollback()
        {
            foreach (var mySqlTransaction in _mySqlTransactions)
            {
                try
                {
                    mySqlTransaction.Rollback();
                }
                catch(Exception)
                {
                }
            }
        }

        public void Dispose()
        {
            foreach (var mySqlTransaction in _mySqlTransactions)
            {
                mySqlTransaction.Dispose();
            }
        }
    }
}
