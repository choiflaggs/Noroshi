using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Entity
{
    public class ShardDispatcherEntity
    {
        IEnumerable<ShardConfig> _configs;
        
        public ShardDispatcherEntity()
        {
            _configs = new ShardConfig[]
            {
                new ShardConfig { ID = 1, Weight = 100, },
//                new ShardConfig { ID = 2, Weight = 100, },
            };
        }

        public uint GetShardID()
        {
            return ContextContainer.GetContext().RandomGenerator.Lot(_configs, c => c.Weight).ID;
        }

        class ShardConfig
        {
            public uint ID { get; set; }
            public uint Weight { get; set; }
        }
    }
}