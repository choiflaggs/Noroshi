using System;
using Noroshi.Core.WebApi.Response.Debug;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Debug;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Debug
{
    public class TimeDebugService
    {
        public static TimeDebug ChangeTime(int year, int month, int day, int hour, int minute, int second)
        {
            var hostName = Environment.MachineName;
            TimeDebugEntity entity = null;
            ContextContainer.NoroshiTransaction(tx =>
            {
                entity = TimeDebugEntity.ReadAndBuild(hostName, ReadType.Lock);
                var result = entity.ChangeTime(year, month, day, hour, minute, second);
                tx.Commit();
                return result;
            });
            return entity.ToResponseData();
        }

        public static TimeDebug Get()
        {
            var hostName = Environment.MachineName;
            return TimeDebugEntity.ReadAndBuild(hostName).ToResponseData();
        }
    }
}