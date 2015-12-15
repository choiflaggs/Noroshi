using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb.Schemas;

namespace Noroshi.Server.Daos.Kvs
{
    public class GearEnthantCalculatedDao : AbstractDao<GearEnthantCalculatedDao.Key, GearEnthantCalculatedDao.Value>
    {
        public class Value
        {
            public Dictionary<uint, Dictionary<byte, uint>> LevelToNecessaryExpMap { get; set; }
            public Dictionary<uint, byte> MaxLevel { get; set; }
            public Dictionary<uint, uint> MaxExp { get; set; }
        }
        public class Key
        {
            public override string ToString() => "gear_enchant_Calculated";
        }
    }
}