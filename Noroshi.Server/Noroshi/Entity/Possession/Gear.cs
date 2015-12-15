using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class Gear : IPossessionObject
    {
        private readonly GearEntity _gear;

        public Gear(GearEntity gear, uint num, uint possessionNum)
        {
            _gear = gear;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.Gear;
        public uint ID => _gear.GearID;
        public uint Num
        { get; }

        public string TextKey => _gear.TextKey;
        public uint PossessingNum
        { get; }

        public Core.WebApi.Response.Possession.PossessionObject ToResponseData()
        {
            return new Core.WebApi.Response.Possession.PossessionObject
            {
                Category = (byte)Category,
                ID = ID,
                Num = Num,

                Name = TextKey,
                PossessingNum = PossessingNum,
            };
        }
    }
}
