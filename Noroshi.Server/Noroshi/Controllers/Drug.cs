using LightNode.Server;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class Drug : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Drug[] MasterData() => DrugService.MasterData();

        [Get]
        public PlayerDrug[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return DrugService.GetAll(playerId);
        }

        [Post]
        public CharacterExpDopingResponse UseDrugWithCharacter(uint drugId, uint characterId,
            ushort usePossessionsCount)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return DrugService.ConsumeDrug(playerId, characterId, drugId, usePossessionsCount);
        }
    }
}
