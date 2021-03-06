﻿using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleStorySchema;

namespace Noroshi.Server.Daos.Rdb.Battle
{
    public class CpuBattleStoryDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByBattleIDs(IEnumerable<uint> battleIds)
        {
            return _select("BattleID IN @BattleIDs", new { BattleIDs = battleIds });
        }
    }
}
