using System;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Services
{
    public class CraftService
    {
        public static GearCraft GearCraft(uint playerId, uint gearId)
        {
            var gearRecipeData = GearRecipeEntity.ReadAndBuildByCraftGearID(gearId);

            if (gearRecipeData == null) {
                throw new InvalidOperationException("Not Found Craft Data : " + gearId);
            }

            var allPossessableParams = gearRecipeData.ToList().Select(e => e.GetUsingMaterialPossessionParam()).ToList();
            allPossessableParams.Add(gearRecipeData.First().GetCraftingGearPosssesionParam());



            var possessionManager = new PossessionManager(playerId, allPossessableParams);
            possessionManager.Load();

            var isRemove = true;
            gearRecipeData.ToList().ForEach(data => isRemove = isRemove && possessionManager.CanRemove(data.GetUsingMaterialPossessionParam()));
            if (!isRemove) {
                throw new InvalidOperationException("Cannot Remove Possession : " + gearId);
            }

            ContextContainer.ShardTransaction(tx =>
            {
                gearRecipeData.ToList().ForEach(data => possessionManager.Remove(data.GetUsingMaterialPossessionParam()));
                possessionManager.Add(gearRecipeData.First().GetCraftingGearPosssesionParam());
                tx.Commit();
            });

            return new GearCraft
            {
                UseMaterialPossessionObjects = gearRecipeData.Select(
                    data =>
                        possessionManager.GetPossessionObject(data.GetUsingMaterialPossessionParam()).ToResponseData())
                    .ToArray(),
            CreateGearPossessionObject = possessionManager.GetPossessionObject(gearRecipeData.First().GetCraftingGearPosssesionParam()).ToResponseData()
            };
        }
    }
}