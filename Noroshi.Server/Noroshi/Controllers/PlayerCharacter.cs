using LightNode.Server;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerCharacter : AbstractController
    {
        [Get]
        public Core.WebApi.Response.PlayerCharacter[] Get()
            => PlayerCharacterService.Get(ContextContainer.GetWebContext().Player.ID);

        [Get]
        public Core.WebApi.Response.PlayerCharacter[] GetAllCharacters()
            => PlayerCharacterService.GetAllCharacters(ContextContainer.GetWebContext().Player.ID);

        [Post]
        public PlayerCharacterAndPlayerItemsResponse UpPromotionLevel(uint playerCharacterId)
            => PlayerCharacterService.UpPromotionLevel(ContextContainer.GetWebContext().Player.ID, playerCharacterId);

        [Post]
        public PlayerCharacterAndStatusResponse UpActionLevel(uint playerCharacterId, ushort level, byte index)
            => PlayerCharacterService.UpActionLevel(ContextContainer.GetWebContext().Player.ID, playerCharacterId, level, index);

        [Post]
        public PlayerCharacterChangeGearResponse EquipCharacter(uint playerCharacterId, uint gearId, byte index)
            => PlayerCharacterService.EquipCharacter(ContextContainer.GetWebContext().Player.ID, playerCharacterId, gearId, index);
    }
}
