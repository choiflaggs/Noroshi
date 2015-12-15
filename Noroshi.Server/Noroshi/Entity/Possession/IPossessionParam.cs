using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public interface IPossessionParam
    {
        PossessionCategory Category { get; }
        uint ID { get; }
        uint Num { get; }
    }
}
