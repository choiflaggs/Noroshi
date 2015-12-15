using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Noroshi.Server.Daos.Rdb
{
    public abstract class AbstractDaoWrapperEntity<TEntity, TDao, TPrimaryKey, TRecord>
        where TEntity : AbstractDaoWrapperEntity<TEntity, TDao, TPrimaryKey, TRecord>, new()
        where TDao : IDao<TPrimaryKey, TRecord>, new()
        where TPrimaryKey : IPrimaryKey
        where TRecord : class, IRecord
    {

        public static TEntity ReadAndBuild(TPrimaryKey pk, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new TDao()).ReadByPK(pk, readType));
        }
        public static IEnumerable<TEntity> ReadAndBuildMulti(IEnumerable<TPrimaryKey> pks, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new TDao()).ReadMultiByPKs(pks, readType));
        }

        public static TEntity Create(TRecord record)
        {
            return _instantiate((new TDao()).Create(record));
        }

        protected static IEnumerable<TEntity> _instantiate(IEnumerable<TRecord> records)
        {
            return records.Select(r => _instantiate(r));
        }
        protected static TEntity _instantiate(TRecord record)
        {
            if (record == null) return null;
            var instance = new TEntity();
            instance._setRecord(record);
            return instance;
        }


        public bool HasRecord => _record != null;
        public bool HasLocalChange { get; private set; }
        protected TRecord _record { get; private set; }


        void _setRecord(TRecord record)
        {
            _record = record;
            HasLocalChange = false;
        }
        protected void _changeLocalRecord(TRecord record)
        {
            _record = record;
            HasLocalChange = true;
        }

        public virtual bool Save()
        {
            if (_record == null)
            {
                throw new InvalidOperationException();
            }
            if (_record.GetReadType() != ReadType.Lock)
            {
                throw new InvalidOperationException();
            }

            HasLocalChange = false;
            return (new TDao()).Update(_record);
        }

        public bool Delete()
        {
            var record = _record;
            _record = null;
            HasLocalChange = false;
            return (new TDao()).Delete(record);
        }

        protected TRecord _cloneRecord()
        {
            return (TRecord)_record.Clone();
        }

        protected string _serializeToText<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
        protected T _deserializeFromText<T>(string text)
        {
            return string.IsNullOrEmpty(text) ? default(T) : JsonConvert.DeserializeObject<T>(text);
        }
    }
}
