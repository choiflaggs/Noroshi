using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public struct PossessionParam : IPossessionParam
    {
        public PossessionCategory Category { get; set; }
        public uint ID { get; set; }
        public uint Num { get; set; }
    }
}
