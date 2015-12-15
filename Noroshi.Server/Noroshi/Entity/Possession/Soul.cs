using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class Soul : IPossessionObject
    {
        private readonly SoulEntity _soul;
        
        public Soul(SoulEntity soul, uint num, uint possessionNum)
        {
            _soul = soul;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.Soul;
        public uint ID => _soul.SoulID;
        public uint Num { get; }

        public string TextKey => _soul.TextKey;
        public uint PossessingNum { get; }

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
