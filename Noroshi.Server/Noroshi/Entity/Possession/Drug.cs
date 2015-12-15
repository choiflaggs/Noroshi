using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class Drug : IPossessionObject
    {
        private readonly DrugEntity _drug;

        public Drug(DrugEntity drug, uint num, uint possessionNum)
        {
            _drug = drug;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.Drug;
        public uint ID => _drug.DrugID;
        public uint Num
        { get; }

        public string TextKey => _drug.TextKey;
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
