using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.PlayerCounter;

using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerCounterSchema;

namespace Noroshi.Server.Daos.Rdb.PlayerCounter
{
    public class PlayerCounterDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record CreateOrSelect(uint counterId)
        {
            return Create(DefaultRecord(counterId)) ?? ReadByPK(new Schema.PrimaryKey { ID = counterId }, ReadType.Lock);
        }

        public Schema.Record DefaultRecord(uint counterId)
        {
            return new Schema.Record
            {
                ID = counterId,
                Count = 0
            };
        }

    }
}
