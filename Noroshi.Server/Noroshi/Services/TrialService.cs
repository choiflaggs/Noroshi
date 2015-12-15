using System;
using System.Linq;
using Noroshi.Core.WebApi.Response.Trial;
using Noroshi.Core.Game.GameContent;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Trial;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Services
{
    public class TrialService
    {
        public static ListResponse List(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            // コンテンツオープン判定。
            if (!GameContent.IsOpen(GameContentID.Trial, playerStatus.Level))
            {
                throw new InvalidOperationException(string.Join("\t", "Trial Not Open", playerStatus.PlayerID, playerStatus.Level));
            }
            // 試練関連情報を取得。
            var trials = TrialEntity.ReadAndBuildVisibleAll();
            var trialStages = trials.SelectMany(trial => trial.GetStages());
            var playerTrials = PlayerTrialEntity.ReadOrDefaultAndBuildMulti(playerId, trials.Select(trial => trial.ID));
            var playerTrialStages = PlayerTrialStageEntity.ReadOrDefaultAndBuildMulti(playerId, trialStages.Select(ts => ts.ID));

            var possessionParams = trialStages.SelectMany(ts => ts.GetDroppableRewards()).Distinct();
            var possessionManager = new PossessionManager(playerId, possessionParams);
            possessionManager.Load();

            /* レスポンス */

            var trialIdToPlayerTrialMap = playerTrials.ToDictionary(pt => pt.TrialID);
            var trialIdToTrialStageIdToTrueMap = trialStages.ToLookup(ts => ts.TrialID)
                .ToDictionary(group => group.Key, group => group.ToDictionary(ts => ts.ID, ts => true));

            return new ListResponse
            {
                Trials = trials.Select(trial =>
                {
                    var playerTrial = trialIdToPlayerTrialMap[trial.ID];
                    var thisPlayerTrialStages = playerTrialStages
                        .Where(pts => trialIdToTrialStageIdToTrueMap[trial.ID].ContainsKey(pts.TrialStageID));
                    return trial.ToResponseData(playerTrial, thisPlayerTrialStages, possessionManager);
                }).ToArray(),
            };
        }
    }
}
