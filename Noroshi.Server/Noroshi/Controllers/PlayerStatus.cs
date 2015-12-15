using LightNode.Server;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerStatus : AbstractController
    {
        [Get]
        public Core.WebApi.Response.PlayerStatus Get() => PlayerStatusService.Get();

        [Post]
        public Core.WebApi.Response.OtherPlayerStatus GetOther(uint id) => PlayerStatusService.GetOhter(id);

        [Post]
        public Core.WebApi.Response.PlayerStatusLevelUpResponse AddExp(uint exp) => PlayerStatusService.AddExp(exp);

        [Post]
        public Core.WebApi.Response.PlayerStatus AddVipExp(uint exp) => PlayerStatusService.AddVipExp(exp);

        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeAvaterCharacterID(ushort id) => PlayerStatusService.ChangeAvaterCharacterID(id);

        [Post]
        public Core.WebApi.Response.PlayerStatus AddChargeGem(uint gem) => PlayerStatusService.AddChargeGem(gem);

        [Post]
        public Core.WebApi.Response.PlayerStatus AddFreeGem(uint gem) => PlayerStatusService.AddFreeGem(gem);

        [Post]
        public Core.WebApi.Response.PlayerStatus UseGemWithPlayer(uint gem, ushort count, byte type) => PlayerStatusService.UseGemWithPlayer(gem, count, type);

        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeName(string name) => PlayerStatusService.ChangeName(name);

        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeExp(uint exp) => PlayerStatusService.ChangeExp(exp);
        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeLevel(ushort level) => PlayerStatusService.ChangeLevel(level);
        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeGold(uint gold) => PlayerStatusService.ChangeGold(gold);
        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeGem(uint gem) => PlayerStatusService.ChangeGem(gem);
        [Post]
        public Core.WebApi.Response.PlayerStatus ChangeStamina(ushort stamina) => PlayerStatusService.ChangeStamina(stamina);
    }
}