using LightNode.Server;
using Newtonsoft.Json;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Battle : AbstractController
    {
        [Post]
        public CpuBattleStartResponse StartCpuBattle(byte category, uint id, uint[] playerCharacterIds, uint? rentalPlayerCharacterId, uint paymentNum)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return BattleService.StartCpuBattle((BattleCategory)category, id, playerId, playerCharacterIds, rentalPlayerCharacterId, paymentNum);
        }
        [Post]
        public CpuBattleFinishResponse FinishCpuBattle(byte category, uint id, byte victoryOrDefeat, byte rank, string result)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var battleResult = JsonConvert.DeserializeObject<BattleResult>(result);

            return BattleService.FinishCpuBattle((BattleCategory)category, id, playerId, (VictoryOrDefeat)victoryOrDefeat, rank, battleResult);
        }
        [Post]
        public PlayerBattleStartResponse StartPlayerBattle(byte category, uint id, uint[] playerCharacterIds, uint? rentalPlayerCharacterId, uint paymentNum)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return BattleService.StartPlayerBattle((BattleCategory)category, id, playerId, playerCharacterIds, rentalPlayerCharacterId, paymentNum);
        }
        [Post]
        public PlayerBattleFinishResponse FinishPlayerBattle(byte category, uint id, byte victoryOrDefeat, byte rank, string result)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var battleResult = JsonConvert.DeserializeObject<BattleResult>(result);

            return BattleService.FinishPlayerBattle((BattleCategory)category, id, playerId, (VictoryOrDefeat)victoryOrDefeat, rank, battleResult);
        }
    }
}
