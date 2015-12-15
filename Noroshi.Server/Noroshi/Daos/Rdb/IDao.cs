using System.Collections.Generic;

namespace Noroshi.Server.Daos.Rdb
{
    public interface IDao<TPrimaryKey, TRecord>
        where TPrimaryKey : IPrimaryKey
        where TRecord : IRecord
    {
        TRecord ReadByPK(TPrimaryKey pk, ReadType readType);
        IEnumerable<TRecord> ReadMultiByPKs(IEnumerable<TPrimaryKey> pks, ReadType readType);
        IEnumerable<TRecord> ReadAll();
        TRecord Create(TRecord record);
        IEnumerable<TRecord> CreateMulti(IEnumerable<TRecord> records);
        bool Update(TRecord record);
        bool Delete(TRecord record);
    }
}
