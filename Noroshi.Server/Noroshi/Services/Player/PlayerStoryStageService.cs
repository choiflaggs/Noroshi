using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb.Schemas;
using Noroshi.Server.Entity;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Services.Player
{
    public class PlayerStoryStageService
    {
        public static PlayerStoryStage Get(uint stageId)
        {
            var stageData = StoryStageEntity.ReadAndBuild(stageId);
            if (stageData == null) {
                throw new InvalidOperationException();
            }
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var queryStageData = PlayerStoryStageEntity.Get(playerId, stageId);
            ContextContainer.ShardTransaction(tx =>
            {
                queryStageData.ResetTimeCheck(stageId);
                queryStageData.Save();
                tx.Commit();
            });
            var battleData = CpuBattleEntity.ReadAndBuild(stageData.BattleID);
            var possessionManager = new PossessionManager(playerId, battleData.GetDroppableRewards());
            possessionManager.Load();
            return queryStageData.ToResponseData(stageData.ToResponseData(battleData, possessionManager));
        }

        public static PlayerStoryStage[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var queryStageData = PlayerStoryStageEntity.ReadAndBuildMultiByPlayerID(playerId);
            if (!queryStageData.Any())
            {
                var data = new List<PlayerStoryStage>();
                return data.ToArray();
            }
            var stages = StoryStageEntity.ReadAndBuildMulti(queryStageData.Select(d => d.StageID));
            var battleIds = stages.Select(stage => stage.BattleID);
            var battleMap = CpuBattleEntity.ReadAndBuildMulti(battleIds).ToDictionary(battle => battle.ID);
            var possessionManager = new PossessionManager(playerId, battleMap.Values.SelectMany(battle => battle.GetDroppableRewards()));
            possessionManager.Load();
            var stageResponce =
                stages.Select(stage => stage.ToResponseData(battleMap[stage.BattleID], possessionManager)).ToDictionary(d => d.ID);
            return queryStageData.Select(e => e.ToResponseData(stageResponce[e.StageID])).ToArray();
        }

        public static PlayerStoryStage[] GetByEpisodeID(uint episodeId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var stages = StoryStageEntity.ReadAndBuildByEpisodeID(episodeId);
            var queryStageData = PlayerStoryStageEntity.ReadAndBuildMulti(stages.Select(e => new PlayerStoryStageSchema.PrimaryKey {StageID = e.ID, PlayerID = playerId}));
            var battleIds = stages.Select(stage => stage.BattleID);
            var battleMap = CpuBattleEntity.ReadAndBuildMulti(battleIds).ToDictionary(battle => battle.ID);
            var possessionManager = new PossessionManager(playerId, battleMap.Values.SelectMany(battle => battle.GetDroppableRewards()));
            possessionManager.Load();
            var stageResponce =
                stages.Select(stage => stage.ToResponseData(battleMap[stage.BattleID], possessionManager)).ToDictionary(d => d.ID);
            return queryStageData.Select(e => e.ToResponseData(stageResponce[e.StageID])).ToArray();
        }

        public static PlayerStoryStage UsePlayCount(uint stageId)
        {
            var stageData = StoryStageEntity.ReadAndBuild(stageId);
            var playerId = ContextContainer.GetWebContext().Player.ID;
            PlayerStoryStageEntity queryStageData = null;
            ContextContainer.NoroshiTransaction(tx =>
            {
                queryStageData = PlayerStoryStageEntity.UsePlayCount(playerId, stageId);
                var result = queryStageData.Save();
                tx.Commit();
                return result;
            });
            var battleData = CpuBattleEntity.ReadAndBuild(stageData.BattleID);
            var possessionManager = new PossessionManager(playerId, battleData.GetDroppableRewards());
            possessionManager.Load();
            return queryStageData.ToResponseData(stageData.ToResponseData(battleData, possessionManager));
        }

        public static PlayerStoryStage ChangeProgress(uint stageId, byte progress)
        {
            var stageData = StoryStageEntity.ReadAndBuild(stageId);
            var playerId = ContextContainer.GetWebContext().Player.ID;
            PlayerStoryStageEntity queryStageData = null;
            ContextContainer.NoroshiTransaction(tx =>
            {
                queryStageData = PlayerStoryStageEntity.ChangedProgress(playerId, stageId, progress);
                var result = queryStageData.Save();
                tx.Commit();
                return result;
            });
            var battleData = CpuBattleEntity.ReadAndBuild(stageData.BattleID);
            var possessionManager = new PossessionManager(playerId, battleData.GetDroppableRewards());
            possessionManager.Load();
            return queryStageData.ToResponseData(stageData.ToResponseData(battleData, possessionManager));
        }
    }
}