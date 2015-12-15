using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Guild;

namespace Noroshi.Server.Controllers
{
    public class Guild : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Guild.GetOwnResponse GetOwn()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.GetOwn(playerId);
        }
        [Get]
        public Core.WebApi.Response.Guild.GetResponse Get(uint guildId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.Get(playerId, guildId);
        }
        [Get]
        public Core.WebApi.Response.Guild.GetRecommendedGuildsResponse GetRecommendedGuilds()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.GetRecommendedGuilds(playerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.JoinResponse JoinBeginnerGuild()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.JoinBeginnerGuild(playerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.JoinAutomaticallyResponse JoinAutomatically(uint countryId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.JoinAutomatically(playerId, countryId);
        }
        [Post]
        public Core.WebApi.Response.Guild.CreateResponse Create(bool isOpen, uint countryId, ushort necessaryPlayerLevel, string name, string introduction)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.Create(playerId, isOpen, countryId, necessaryPlayerLevel, name, introduction);
        }
        [Post]
        public Core.WebApi.Response.Guild.JoinResponse Join(uint guildId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.Join(playerId, guildId);
        }
        [Post]
        public Core.WebApi.Response.Guild.HandleRequestResponse Request(uint guildId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.Request(playerId, guildId);
        }
        [Post]
        public Core.WebApi.Response.Guild.HandleRequestResponse CancelRequest()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.CancelRequest(playerId);
        }

        [Post]
        public Core.WebApi.Response.Guild.HandleReceivedRequestResponse AcceptRequest(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.AcceptRequest(playerId, targetPlayerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.HandleReceivedRequestResponse RejectRequest(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildService.RejectRequest(playerId, targetPlayerId);
        }

        [Post]
        public Core.WebApi.Response.Guild.ConfigureResponse Configure(bool? isOpen, uint? countryId, ushort? necessaryPlayerLevel, string name, string introduction)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildLeaderService.Configure(playerId, isOpen, countryId, necessaryPlayerLevel, name, introduction);
        }
        [Post]
        public Core.WebApi.Response.Guild.AddExecutiveRoleResponse AddExecutiveRole(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildLeaderService.AddExecutiveRole(playerId, targetPlayerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.RemoveExecutiveRoleResponse RemoveExecutiveRole(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildLeaderService.RemoveExecutiveRole(playerId, targetPlayerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.ChangeLeaderResponse ChangeLeader(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildLeaderService.ChangeLeader(playerId, targetPlayerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.LayOffResponse LayOff(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildLeaderService.LayOff(playerId, targetPlayerId);
        }

        [Get]
        public Core.WebApi.Response.Guild.GetTakableRentalCharactersResponse GetTakableRentalCharacters()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RentalCharacterService.GetTakableRentalCharacters(playerId);
        }
        [Get]
        public Core.WebApi.Response.Guild.GetRentalCharactersResponse GetRentalCharacters()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RentalCharacterService.GetRentalCharacters(playerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.AddRentalCharacterResponse AddRentalCharacter(byte no, uint playerCharacterId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RentalCharacterService.AddRentalCharacter(playerId, no, playerCharacterId);
        }
        [Post]
        public Core.WebApi.Response.Guild.RemoveRentalCharacterResponse RemoveRentalCharacters()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RentalCharacterService.RemoveRentalCharacters(playerId);
        }

        [Post]
        public Core.WebApi.Response.Guild.GreetResponse Greet(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return GreetService.Greet(playerId, targetPlayerId);
        }
        [Post]
        public Core.WebApi.Response.Guild.ReceiveGreetedRewardResponse ReceiveGreetedReward()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return GreetService.ReceiveGreetedReward(playerId);
        }
    }
}
