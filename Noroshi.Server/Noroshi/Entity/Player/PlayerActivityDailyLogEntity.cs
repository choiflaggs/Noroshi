using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerActivityDailyLogSchema;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerActivityDailyLogEntity : AbstractDaoWrapperEntity<PlayerActivityDailyLogEntity, PlayerActivityDailyLogDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<PlayerActivityDailyLogEntity> CreateOrReadAndBuildTodayActivityLogs(IEnumerable<uint> playerIds)
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC;
            return playerIds.Select(playerId => _instantiate((new PlayerActivityDailyLogDao()).CreateOrRead(playerId, createdOn)));
        }
        public static IEnumerable<PlayerActivityDailyLogEntity> ReadAndBuildThisWeekByPlayerIDs(IEnumerable<uint> playerIds)
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC - 3600 * 24 * 7;
            return _instantiate((new PlayerActivityDailyLogDao()).ReadByPlayerIDsAndMinCreatedOn(playerIds, createdOn));
        }

        public uint PlayerID => _record.PlayerID;
        public uint StaminaConsuming => _record.StaminaConsuming;
        public uint BPConsuming => _record.BPConsuming;

        public void ConsumeStamina(ushort stamina)
        {
            var record = _cloneRecord();
            record.StaminaConsuming += stamina;
            _changeLocalRecord(record);
        }
        public void ConsumeBP(byte bp)
        {
            var record = _cloneRecord();
            record.BPConsuming += bp;
            _changeLocalRecord(record);
        }
    }
}
