using System.Linq;
using Noroshi.Server.Entity;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Services
{
    public class StoryService
    {
        /// <summary>
        /// コンテンツ「ストーリ」に関するマスターデータを返す。
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static Core.WebApi.Response.Story.MasterDataResponse MasterData(uint playerId)
        {
            // TODO : データ構造が変更になるのであとで直す。
            var stages = StoryStageEntity.ReadAndBuildAll();
            var battleIds = stages.Select(stage => stage.BattleID);
            var battleMap = CpuBattleEntity.ReadAndBuildMulti(battleIds).ToDictionary(battle => battle.ID);
            var possessionManager = new PossessionManager(playerId, battleMap.Values.SelectMany(battle => battle.GetDroppableRewards()));
            possessionManager.Load();

            return new Core.WebApi.Response.Story.MasterDataResponse
            {
                Stages = stages.Select(stage => stage.ToResponseData(battleMap[stage.BattleID], possessionManager)).ToArray(),
            };
        }
    }
}
