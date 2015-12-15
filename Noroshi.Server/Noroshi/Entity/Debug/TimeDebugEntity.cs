using System;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Debug;
using Noroshi.Server.Services.Utility;

namespace Noroshi.Server.Entity.Debug
{
    public class TimeDebugEntity : AbstractDaoWrapperEntity<TimeDebugEntity, TimeDebugDao, TimeDebugDao.PrimaryKey, TimeDebugDao.Record>
    {

        public static TimeDebugEntity ReadAndBuild(string hostName, ReadType readType = ReadType.Slave)
        {
            var record = (new TimeDebugDao()).ReadByHostName(hostName, readType);
            var entity = _instantiate(record);
            return entity;
        }

        public static TimeDebugEntity Create(string hostName)
        {
            var entity = _instantiate((new TimeDebugDao()).Create(hostName));
            return entity;
        }

        public static TimeDebugEntity CreateOrSelect(string hostName)
        {
            var entity = _instantiate((new TimeDebugDao()).CreateOrSelect(hostName));
            return entity;
        }


        public bool ChangeTime(int year, int month, int day, int hour, int minute, int second)
        {
            var time = (int)(UnixTime.ToUnixTime(DateTime.UtcNow.AddSeconds(second)
                .AddMinutes(minute)
                .AddHours(hour)
                .AddDays(day)
                .AddMonths(month)
                .AddYears(year)
                ) - UnixTime.ToUnixTime(DateTime.UtcNow));
            _changeModifyTime(time);
            return Save();
        }

        private void _changeModifyTime(int time)
        {
            var record = _record.Clone() as TimeDebugDao.Record;
            record.ModifyTime = time;
            _changeLocalRecord(record);
        }

        public string HostName => _record.HostName;
        public int ModifyTime => _record.ModifyTime;

        public Core.WebApi.Response.Debug.TimeDebug ToResponseData()
        {
            return new Core.WebApi.Response.Debug.TimeDebug
            {
                ModifyTime = ModifyTime,
                ServerTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time")),
                DebugServerTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time")).AddSeconds(_record.ModifyTime)
            };
        }

    }
}