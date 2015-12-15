using LightNode.Server;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.WebApi.Response.Character;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class Soul : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Soul[] MasterData() => SoulService.MasterData();

        [Get]
        public Core.WebApi.Response.PlayerSoul[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return SoulService.GetAll(playerId);
        }

        [Post]
        public CreateCharacter UseSoulWithCreateCharacter(uint soulId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return SoulService.UseSoulWithCreateCharacter(playerId, soulId);
        }

        [Post]
        public CharacterEvolutionLevelUpResponse UseSoulWithEvolutionLevel(uint soulId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return SoulService.UseSoulWithEvolutionLevel(playerId, soulId);
        }

    }
}