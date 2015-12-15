using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using Dapper;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Daos.Rdb
{
    public abstract class AbstractDao<TPrimaryKey, TRecord> : IDao<TPrimaryKey, TRecord>
        where TPrimaryKey : IPrimaryKey
        where TRecord : class, IRecord
    {
        static IDictionary<string, PropertyInfo> _recordPropertyInfos;
        static IDictionary<string, PropertyInfo> _pkPropertyInfos;

        static void _cacheRecordPropertyInfos()
        {
            if (_recordPropertyInfos != null) return;
            _recordPropertyInfos = typeof(TRecord).GetProperties().ToDictionary(p => p.Name);
        }
        static void _cachePkPropertyInfos()
        {
            if (_pkPropertyInfos != null) return;
            _pkPropertyInfos = typeof(TPrimaryKey).GetProperties().ToDictionary(p => p.Name);
        }

        static string _getRecordPropertyValueString(TRecord record, string propertyName)
        {
            _cacheRecordPropertyInfos();
            var obj = _recordPropertyInfos[propertyName].GetValue(record);
            return obj is string ? $"'{obj}'" : obj.ToString();
        }
        static string _getPkPropertyValueString(TPrimaryKey pk, string propertyName)
        {
            _cachePkPropertyInfos();
            var obj = _pkPropertyInfos[propertyName].GetValue(pk);
            return obj is string ? $"'{obj}'" : obj.ToString();
        }

        static IEnumerable<string> _getRecordColumnNames()
        {
            _cacheRecordPropertyInfos();
            return _recordPropertyInfos.Keys;
        }
        static IEnumerable<string> _getPkColumnNames()
        {
            _cachePkPropertyInfos();
            return _pkPropertyInfos.Keys;
        }


        public TRecord ReadByPK(TPrimaryKey pk, ReadType readType = ReadType.Slave)
        {
            var whereStmt = string.Join(
                " AND ",
                _getPkColumnNames().Select(s => $"{s} = {_getPkPropertyValueString(pk, s)}")
            );
            return _select(whereStmt, readType).FirstOrDefault();
        }
        public IEnumerable<TRecord> ReadMultiByPKs(IEnumerable<TPrimaryKey> pks, ReadType readType = ReadType.Slave)
        {
            var whereStmt = string.Join(
                " OR ",
                pks.Select(pk =>
                {
                    return string.Join(
                        " AND ",
                        _getPkColumnNames().Select(s => $"{s} = {_getPkPropertyValueString(pk, s)}")
                    );
                })
            );
            return _select(whereStmt, readType);
        }

        public IEnumerable<TRecord> ReadAll()
        {
            return _query($"SELECT * FROM `{_tableName}`", ReadType.Slave);
        }

        protected IEnumerable<TRecord> _select(string statement, ReadType readType = ReadType.Slave)
        {
            var sql = readType == ReadType.Lock ? "SELECT * FROM `{0}` WHERE {1} FOR UPDATE" : "SELECT * FROM `{0}` WHERE {1}";
            return _query(string.Format(sql, _tableName, statement), readType);
        }
        protected IEnumerable<TRecord> _select(string statement, object bindParam, ReadType readType = ReadType.Slave)
        {
            var sql = readType == ReadType.Lock ? "SELECT * FROM `{0}` WHERE {1} FOR UPDATE" : "SELECT * FROM `{0}` WHERE {1}";
            return _queryWithBindParam(string.Format(sql, _tableName, statement), bindParam, readType);
        }

        public TRecord Create(TRecord record)
        {
            if (_create(_getMasterDbConnection(), record))
            {
                record.SetReadType(ReadType.Lock);
                return record;
            }
            return null;
        }
        protected bool _create(IDbConnection connection, TRecord record)
        {
            _createTimeStamp(record);
            var columnNames = _getRecordColumnNames();
            var enumerable = columnNames as string[] ?? columnNames.ToArray();
            var sql =
                $"INSERT IGNORE INTO `{_tableName}` ({string.Join(", ", enumerable)}) VALUES ({string.Join(", ", enumerable.Select(c => _getRecordPropertyValueString(record, c)))})";
            return connection.Execute(sql) == 1;
        }

        public IEnumerable<TRecord> CreateMulti(IEnumerable<TRecord> records)
        {
            return records.Select(r => Create(r));
        }
        public bool Update(TRecord record)
        {
            _updateTimeStamp(record);
            var pkNames = _getPkColumnNames();
            var enumerable = pkNames as string[] ?? pkNames.ToArray();
            var columnNames = _getRecordColumnNames().Except(enumerable);

            var whereStmt = string.Join(
                " AND ",
                enumerable.Select(s => $"{s} = {_getRecordPropertyValueString(record, s)}")
            );

            var sql =
                $"UPDATE `{_tableName}` SET {string.Join(", ", columnNames.Select(c => $"{c} = {_getRecordPropertyValueString(record, c)}"))} WHERE {whereStmt}";

            return _getMasterDbConnection().Execute(sql) == 1;
        }

        public bool Delete(TRecord record)
        {
            var pkNames = _getPkColumnNames();
            var whereStmt = string.Join(
                " AND ",
                pkNames.Select(s => $"{s} = {_getRecordPropertyValueString(record, s)}")
            );
            var sql = "DELETE FROM `{0}` WHERE {1}";
            return _getDbConnection(ReadType.Lock).Execute(string.Format(sql, _tableName, whereStmt)) == 1;
        }

        public void Truncate()
        {
            _getDbConnection(ReadType.Lock).Execute($"TRUNCATE TABLE `{_tableName}`");
        }

        protected IEnumerable<TRecord> _queryWithBindParam(string statement, object bindParam, ReadType readType)
        {
            return _getDbConnection(readType).Query<TRecord>(statement, bindParam).Select(r =>
            {
                r.SetReadType(readType);
                return r;
            });
        }
        protected IEnumerable<TRecord> _query(string statement, ReadType readType)
        {
            return _getDbConnection(readType).Query<TRecord>(statement).Select(r =>
            {
                r.SetReadType(readType);
                return r;
            });
        }

        void _createTimeStamp(TRecord record)
        {
            _cacheRecordPropertyInfos();
            var columnName = "CreatedAt";
            if (!_recordPropertyInfos.ContainsKey(columnName)) return;
            _recordPropertyInfos[columnName].SetValue(record, ContextContainer.GetContext().TimeHandler.UnixTime);
            _updateTimeStamp(record);
        }

        void _updateTimeStamp(TRecord record)
        {
            _cacheRecordPropertyInfos();
            var columnName = "UpdatedAt";
            if (!_recordPropertyInfos.ContainsKey(columnName)) return;
            _recordPropertyInfos[columnName].SetValue(record, ContextContainer.GetContext().TimeHandler.UnixTime);
        }

        protected abstract string _tableName { get; }

        IDbConnection _getDbConnection(ReadType readType)
        {
            IDbConnection dbConnection = null;
            switch (readType)
            {
                case ReadType.Lock:
                    dbConnection = _getMasterDbConnection();
                    break;
                case ReadType.Master:
                    dbConnection = _getMasterDbConnection();
                    break;
                case ReadType.Slave:
                    dbConnection = _getSlaveDbConnection();
                    break;
            }
            return dbConnection;
        }

        protected abstract IDbConnection _getMasterDbConnection();
        protected abstract IDbConnection _getSlaveDbConnection();
    }
}
