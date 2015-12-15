using System;
using System.Dynamic;
using System.Linq;
using Dapper;

namespace Noroshi.Server.Daos.Rdb.Debug
{
    public class TimeDebugDao : AbstractCommonDBDao<TimeDebugDao.PrimaryKey, TimeDebugDao.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => "time_debug";

        public class Record : AbstractRecord
        {
            public string HostName
            { get; set; }
            public int ModifyTime
            { get; set; }
        }
        public class PrimaryKey : IPrimaryKey
        {
            public string HostName
            { get; set; }
        }

        /* ここまでテーブルマッピング設定 */

        public Record ReadByHostName(string hostName, ReadType readType = ReadType.Slave)
        {
            var test = _select("HostName = @HostName",
                new
                {
                    HostName =
                        new DbString { Value = hostName, IsFixedLength = true, Length = hostName.Length, IsAnsi = true }
                }, readType);
            return test?.FirstOrDefault();
        }
        public Record CreateOrSelect(string hostName)
        {
            var record = Create(hostName) ?? ReadByHostName(hostName, ReadType.Lock);
            return record;
        }

        public Record Create(string hostName)
        {
            var record = new Record { HostName = hostName, ModifyTime = 0 };
            return Create(record);
        }
    }
}
