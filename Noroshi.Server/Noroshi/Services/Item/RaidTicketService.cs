using System.Collections.Generic;
using System.IO;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.WebApi.Response.Possession;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Possession;
using RaidTicket = Noroshi.Core.WebApi.Response.RaidTicket;

namespace Noroshi.Server.Services.Item
{
    public class RaidTicketService
    {
        public static PlayerRaidTicket[] GetAll(uint playerId)
        {
            return PlayerRaidTicketEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static RaidTicket[] MasterData()
        {
            return RaidTicketEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }

        public static UseRaidTicketResponse UseRaidTicket(uint playerId, uint stageId, byte count)
        {
            var raidTicketData = RaidTicketEntity.ReadAndBuildAll().First();
            var stageData = StoryStageEntity.ReadAndBuild(stageId);
            var battleData = CpuBattleEntity.ReadAndBuild(stageData.BattleID);

            var getGoldParam = PossessionManager.GetGoldParam(battleData.Gold * count);
            var getPlayerExpParam = PossessionManager.GetPlayerExpParam((ushort)(stageData.Stamina * count));
            var rewards = battleData.LotRewardsForAutoBattle(count);
            PlayerStatusEntity playerStatus = null;
            var addItemParams = rewards.SelectMany(pps => pps);
            var possesionManager = new PossessionManager(playerId,
            new List<PossessionParam> { getGoldParam, getPlayerExpParam, raidTicketData.GetUsingRaidTicketPosssesionParam(count) }.Concat(addItemParams));
            ContextContainer.ShardTransaction(tx =>
            {
                possesionManager.Load();
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                if (playerStatus.ConsumeStamina((ushort)(stageData.Stamina * count)) &&
                    possesionManager.CanRemove(raidTicketData.GetUsingRaidTicketPosssesionParam(count))) {
                    throw new InvalidDataException();
                }
                playerStatus.SetStamina((ushort)(playerStatus.Stamina - (stageData.Stamina * count)));
                possesionManager.Remove(raidTicketData.GetUsingRaidTicketPosssesionParam(count));
                possesionManager.Add(getGoldParam);
                possesionManager.Add(getPlayerExpParam);
                possesionManager.Add(addItemParams);
                tx.Commit();
            });
            return new UseRaidTicketResponse
            {
                DropRewards = rewards.Select(pps => possesionManager.GetPossessionObjects(pps).Select(po => po.ToResponseData()).ToArray()).ToArray(),
                GetExp = possesionManager.GetPossessionObject(getPlayerExpParam) as PossessionObject,
                GetGold = possesionManager.GetPossessionObject(getGoldParam) as PossessionObject,
                Stamina = playerStatus.Stamina
            };
        }
    }
}
