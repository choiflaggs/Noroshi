using System;
using System.Linq;
using Noroshi.Core.WebApi.Response.Training;
using Noroshi.Core.Game.GameContent;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Training;

namespace Noroshi.Server.Services
{
    public class TrainingService
    {
        public static ListResponse List(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            // コンテンツオープン判定。
            if (!GameContent.IsOpen(GameContentID.Training, playerStatus.Level))
            {
                throw new InvalidOperationException(string.Join("\t", "Training Not Open", playerStatus.PlayerID, playerStatus.Level));
            }
            // 修行関連情報を取得。
            var trainings = TrainingEntity.ReadAndBuildVisibleAll();
            var trainingStages = trainings.SelectMany(training => training.GetStages());
            var playerTrainings = PlayerTrainingEntity.ReadOrDefaultAndBuildMulti(playerId, trainings.Select(training => training.ID));
            var playerTrainingStages = PlayerTrainingStageEntity.ReadOrDefaultAndBuildMulti(playerId, trainingStages.Select(ts => ts.ID));

            /* レスポンス */

            var trainingIdToPlayerTrainingMap = playerTrainings.ToDictionary(pt => pt.TrainingID);
            var trainingIdToTrainingStageIdToTrueMap = trainingStages.ToLookup(ts => ts.TrainingID)
                .ToDictionary(group => group.Key, group => group.ToDictionary(ts => ts.ID, ts => true));

            return new ListResponse
            {
                Trainings = trainings.Select(training =>
                {
                    var playerTraining = trainingIdToPlayerTrainingMap[training.ID];
                    var thisPlayerTrainingStages = playerTrainingStages
                        .Where(pts => trainingIdToTrainingStageIdToTrueMap[training.ID].ContainsKey(pts.TrainingStageID));
                    return training.ToResponseData(playerStatus.Level, playerTraining, thisPlayerTrainingStages);
                }).ToArray(),
            };
        }
    }
}
