using System;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.PlayerCounter;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerCounterSchema;

namespace Noroshi.Server.Entity.PlayerCounter
{
    public enum PlayerCounterType
    {
        ArenaRank = 1,  
    }

    public class PlayerCounterEntity : AbstractDaoWrapperEntity<PlayerCounterEntity, PlayerCounterDao, Schema.PrimaryKey, Schema.Record>
    {
        // カウントする為だけのクラスであり続ける.


        protected uint _generateNumber(PlayerCounterType counterType)
        {
            var entity = _instantiate((new PlayerCounterDao()).CreateOrSelect((uint)counterType));
            var record = entity._cloneRecord();
            record.Count++;

            entity._changeLocalRecord(record);
            entity.Save();
            return record.Count;
        }

        protected uint _getLastNumber(PlayerCounterType counterType)
        {
            return _instantiate((new PlayerCounterDao()).CreateOrSelect((uint)counterType)).Count;
        }

        uint Count => _record.Count;

    }
}
