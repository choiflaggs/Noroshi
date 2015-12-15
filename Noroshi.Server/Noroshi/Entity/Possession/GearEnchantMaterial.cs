using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class GearEnchantMaterial : IPossessionObject
    {
        private readonly GearEnchantMaterialEntity _gearEnchantMaterial;

        public GearEnchantMaterial(GearEnchantMaterialEntity gearEnchantMaterial, uint num, uint possessionNum)
        {
            _gearEnchantMaterial = gearEnchantMaterial;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.GearEnchantMaterial;
        public uint ID => _gearEnchantMaterial.GearEnchantMaterialID;
        public uint Num
        { get; }

        public string TextKey => _gearEnchantMaterial.TextKey;
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
