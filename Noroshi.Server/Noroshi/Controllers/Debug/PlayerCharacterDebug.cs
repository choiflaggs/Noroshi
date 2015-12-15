using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Debug;

namespace Noroshi.Server.Controllers.Debug
{
    public class PlayerCharacterDebug : AbstractController
    {
        [Post]
        public Core.WebApi.Response.PlayerCharacter RemoveEquip(uint playerCharacterId)
            => PlayerCharacterDebugService.RemoveEquip(ContextContainer.GetWebContext().Player.ID, playerCharacterId);

        [Post]
        public Core.WebApi.Response.PlayerCharacter AllEquip(uint playerCharacterId)
            => PlayerCharacterDebugService.AllEquip(ContextContainer.GetWebContext().Player.ID, playerCharacterId);
        [Post]
        public Core.WebApi.Response.PlayerCharacter ChangeLevel(uint playerCharacterId, ushort level)
            => PlayerCharacterDebugService.ChangeLevel(ContextContainer.GetWebContext().Player.ID, playerCharacterId, level);

        [Post]
        public Core.WebApi.Response.PlayerCharacter ChangePromotionLevel(uint playerCharacterId, byte level)
            => PlayerCharacterDebugService.ChangePromotionLevel(ContextContainer.GetWebContext().Player.ID, playerCharacterId, level);

        [Post]
        public Core.WebApi.Response.PlayerCharacter ChangeEvolutionLevel(uint playerCharacterId, byte level)
            => PlayerCharacterDebugService.ChangeEvolutionLevel(ContextContainer.GetWebContext().Player.ID, playerCharacterId, level);

        [Post]
        public Core.WebApi.Response.PlayerCharacter ChangeActionLevel(uint playerCharacterId, ushort level, ushort index)
            => PlayerCharacterDebugService.ChangeActionLevel(ContextContainer.GetWebContext().Player.ID, playerCharacterId, level, index);
    }
}