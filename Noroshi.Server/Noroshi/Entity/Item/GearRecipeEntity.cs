using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearRecipeSchema;

namespace Noroshi.Server.Entity.Item
{
    public class GearRecipeEntity : AbstractDaoWrapperEntity<GearRecipeEntity, GearRecipeDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GearRecipeEntity> ReadAndBuildAll()
        {
            return _instantiate((new GearRecipeDao()).ReadAll());
        }

        public static IEnumerable<GearRecipeEntity> ReadAndBuildByCraftGearID(uint craftItemId)
        {
            return _instantiate((new GearRecipeDao()).ReadByCraftItemID(craftItemId));
        }

        public PossessionParam GetUsingMaterialPossessionParam()
        {
            return new PossessionParam
            {
                Category = Category,
                ID = MaterialItemID,
                Num = Count,
            };
        }

        // ItemBuilder作成中に直す
        public PossessionParam GetCraftingGearPosssesionParam()
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Gear,
                ID = CraftItemID,
                Num = 1,
            };
        }

        //// ItemBuilder作成中に直す
        //public PossessionParam GetUsingGoldPosssesionParam()
        //{
        //    return new PossessionParam
        //    {
        //        Category = PossessionCategory.Status,
        //        ID = (uint)PossessionStatusID.Gold,
        //        Num = 10000
        //    };
        //}



        public uint MaterialItemID => _record.MaterialItemID;

        public uint CraftItemID => _record.CraftItemID;
        public ushort Count => (ushort)_record.Count;
        public PossessionCategory Category => (PossessionCategory)_record.MaterialType;

        public Core.WebApi.Response.GearRecipe ToResponseData()
        {
            return new Core.WebApi.Response.GearRecipe
            {
                MaterialItemID = MaterialItemID,
                CraftItemID = CraftItemID,
                Count = Count
            };
        }

        public IEnumerable<PossessionParam> GetAllPossessableParams()
        {
            return new[]
            {
                GetUsingMaterialPossessionParam(),
                GetCraftingGearPosssesionParam()
            };
        }

    }
}
