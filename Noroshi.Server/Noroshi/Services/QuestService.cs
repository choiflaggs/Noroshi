using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Player;
using Noroshi.Core.WebApi.Response.Quest;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Quest;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services
{
    public class QuestService
    {
        /// <summary>
        /// 指定プレイヤーが閲覧可能な依頼一覧を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.Quest.ListResponse List(uint playerId)
        {
            return _list(playerId, QuestEntity.ReadAndBuildAll, PlayerQuestTriggerEntity.ReadAndBuildByPlayerID);
        }

        /// <summary>
        /// 指定プレイヤーが指定依頼の報酬を受け取る。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="questId">対象依頼ID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.Quest.ReceiveRewardResponse ReceiveReward(uint playerId, uint questId)
        {
            return _receiveReward(
                playerId, questId,
                QuestEntity.ReadAndBuild, QuestEntity.ReadAndBuildByTriggerID,
                PlayerQuestTriggerEntity.ReadAndBuild
            );
        }

        /// <summary>
        /// 指定プレイヤーが閲覧可能なデイリー依頼一覧を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.Quest.ListResponse DailyList(uint playerId)
        {
            return _list(playerId, DailyQuestEntity.ReadAndBuildAll, PlayerDailyQuestTriggerEntity.ReadAndBuildByPlayerIDAndTimedQuestCompensate);
        }

        /// <summary>
        /// 指定プレイヤーが指定デイリー依頼の報酬を受け取る。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="dailyQuestId">対象デイリー依頼ID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.Quest.ReceiveRewardResponse ReceiveDailyReward(uint playerId, uint dailyQuestId)
        {
            return _receiveReward(
                playerId, dailyQuestId,
                DailyQuestEntity.ReadAndBuild, DailyQuestEntity.ReadAndBuildByTriggerID,
                PlayerDailyQuestTriggerEntity.CreateOrReadAndBuildAndTimedQuestCompensate
            );
        }

        static Core.WebApi.Response.Quest.ListResponse _list<T>(
            uint playerId,
            Func<IEnumerable<T>> readAndBuildAllQuests,
            Func<uint, IEnumerable<IPlayerQuestTriggerEntity<T>>> readAndBuildPlayerQuestTriggersByPlayerId
        )
            where T : IQuestEntity
        {
            // プレイヤー依頼状況を取得し、トリガーID => プレイヤー依頼状況 のマッピングを作る。
            var triggerIdToPlayerQuestTrigger = readAndBuildPlayerQuestTriggersByPlayerId(playerId).ToDictionary(p => p.TriggerID);

            // 開放されているゲームコンテンツを取得し、ゲームコンテンツID => ゲームコンテンツのマッピングを作る。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            // 呼び出しタイミング的に PlayerStatus が存在しないことはあり得ない。
            if (playerStatus == null)
            {
                throw new InvalidOperationException("Player Status Not Found");
            }
            var openGameContentMap = playerStatus.GetOpenGameContents().ToDictionary(gameContent => gameContent.ID);

            // 依頼がゲームコンテンツに紐付いている場合は開放されているものだけにフィルタリングし、
            var quests = readAndBuildAllQuests().Where(quest => quest.IsOpen && ( !quest.GameContentID.HasValue || openGameContentMap.ContainsKey(quest.GameContentID.Value) ))
                // トリガー毎に、
                .ToLookup(quest => quest.TriggerID)
                .Select(igroup => igroup
                    // プレイヤー依頼状況がない、もしくはまだ受け取っていないものだけにフィルタリングし、
                    .Where(quest => !triggerIdToPlayerQuestTrigger.ContainsKey(quest.TriggerID) || !triggerIdToPlayerQuestTrigger[quest.TriggerID].HasAlreadyReceivedReward(quest.Threshold))
                    // 敷居値の一番小さいものを選ぶ。
                    .OrderBy(quest => quest.Threshold)
                    .FirstOrDefault()
                )
                // ただし、該当の依頼がないトリガー分は外す。
                .Where(quest => quest != null);

            // 報酬情報をロード。
            var possessionParams = quests.SelectMany(quest => quest.GetPossessionParams());
            var possessionManager = new PossessionManager(playerId, possessionParams);
            possessionManager.Load();

            /* レスポンス */

            return new Core.WebApi.Response.Quest.ListResponse
            {
                Quests = quests.Select(quest =>
                {
                    var playerQuestTrigger = triggerIdToPlayerQuestTrigger.ContainsKey(quest.TriggerID) ? triggerIdToPlayerQuestTrigger[quest.TriggerID] : null;
                    var possessionObjects = possessionManager.GetPossessionObjects(quest.GetPossessionParams());
                    return quest.ToResponseData(playerQuestTrigger, possessionObjects);
                })
                .ToArray(),
            };
        }

        static Core.WebApi.Response.Quest.ReceiveRewardResponse _receiveReward<T>(
            uint playerId, uint questId,
            Func<uint, T> readAndBuildQuest, Func<uint, IEnumerable<T>> readAndBuildQuestsByTriggerId,
            Func<uint, uint, ReadType, IPlayerQuestTriggerEntity<T>> readAndBuildPlayerQuestTrigger
        )
            where T : IQuestEntity
        {

            // 対象依頼取得。
            var quest = readAndBuildQuest(questId);
            // 遷移上、存在しない依頼を指定されることはない。
            if (quest == null)
            {
                throw new InvalidOperationException("Quest Not Found : " + questId);
            }

            // ユーザが公開していないクエストの報酬を受け取ろうとした.
            if (!quest.IsOpen)
            {
                // 時限式クエストの報酬を受け取る導線を、ユーザが時間外まで放置して押す可能性があるのでエラーをクライアントに返す.
                return new Core.WebApi.Response.Quest.ReceiveRewardResponse { QuestError = new QuestError() { IsNotOpen = true}};
            }

            // 対象プレイヤー依頼状況取得。
            var playerQuestTrigger = readAndBuildPlayerQuestTrigger(playerId, quest.TriggerID, ReadType.Slave);
            // 遷移上、プレイヤーデイリー依頼状況が存在しないことはあり得ない。
            if (playerQuestTrigger == null)
            {
                throw new InvalidOperationException("Player Quest Trigger Not Found : " + quest.ID);
            }
            // 遷移上、同一トリガーで敷居値を飛び越えるように報酬を受け取ることはあり得ない。
            var sameTriggerQuests = readAndBuildQuestsByTriggerId(quest.TriggerID);
            var currentQuest = playerQuestTrigger.GetCurrentQuest(sameTriggerQuests);
            if (currentQuest == null || currentQuest.ID != quest.ID)
            {
                throw new InvalidOperationException("Not Next Quest : " + quest.ID);
            }

            // 遷移上、報酬を受け取れないことはあり得ない。
            if (!playerQuestTrigger.CanReceiveReward(quest.Threshold))
            {
                throw new InvalidOperationException("Cannot Receive Quest Reward : " + quest.ID);
            }

            // 報酬付与準備。
            var possessionManager = new PossessionManager(playerId, quest.GetPossessionParams());

            /* トランザクション */

            playerQuestTrigger = ContextContainer.NoroshiTransaction(tx =>
            {
                // 報酬以外の依頼固有更新処理実行。
                var playerQuestTriggerWithLock = _receiveReward(playerId, quest, sameTriggerQuests, readAndBuildPlayerQuestTrigger);
                // 報酬付与。
                possessionManager.Add(quest.GetPossessionParams());

                // チュートリアル進捗処理。
                var tutorialStep = typeof(T) == typeof(QuestEntity) ? TutorialStep.ReceiveQuestReward : TutorialStep.ReceiveDailyQuestReward;
                PlayerStatusEntity.TryToProceedTutorialStep(playerId, tutorialStep);

                tx.Commit();

                return playerQuestTriggerWithLock;
            });

            /* レスポンス */

            var newQuest = playerQuestTrigger.GetCurrentQuest(sameTriggerQuests);
            var addPlayerExpResult = possessionManager.GetAddPlayerExpResult();
            return new Core.WebApi.Response.Quest.ReceiveRewardResponse
            {
                Quest = quest.ToResponseData(playerQuestTrigger, possessionManager.GetPossessionObjects(quest.GetPossessionParams())),
                NewQuest = newQuest != null ? newQuest.ToResponseData(playerQuestTrigger, possessionManager.GetPossessionObjects(quest.GetPossessionParams())) : null,
                AddPlayerExpResult = addPlayerExpResult != null ? addPlayerExpResult.ToResponseData() : null,
            };
        }
        static IPlayerQuestTriggerEntity<T> _receiveReward<T>(
            uint playerId, IQuestEntity quest, IEnumerable<T> sameTriggerQuests,
            Func<uint, uint, ReadType, IPlayerQuestTriggerEntity<T>> readAndBuildPlayerQuestTrigger
        )
            where T : IQuestEntity
        {
            // 対象プレイヤー依頼状況取得。更新に利用するのでロック付き。
            var playerQuestTrigger = readAndBuildPlayerQuestTrigger(playerId, quest.TriggerID, ReadType.Lock);
            // 遷移上、プレイヤーデイリー依頼状況が存在しないことはあり得ない。
            if (playerQuestTrigger == null)
            {
                throw new InvalidOperationException("Player Quest Trigger Not Found : " + quest.ID);
            }
            // 遷移上、同一トリガーで敷居値を飛び越えるように報酬を受け取ることはあり得ない。
            var currentQuest = playerQuestTrigger.GetCurrentQuest(sameTriggerQuests);
            if (currentQuest == null || currentQuest.ID != quest.ID)
            {
                throw new InvalidOperationException("Not Next Quest : " + quest.ID);
            }
            // 遷移上、報酬を受け取れないことはあり得ない。
            if (!playerQuestTrigger.CanReceiveReward(quest.Threshold))
            {
                throw new InvalidOperationException("Cannot Receive Quest Reward : " + quest.ID);
            }

            // 報酬受け取り処理（報酬付与は別途）。
            playerQuestTrigger.ReceiveReward(quest.Threshold);

            return playerQuestTrigger;
        }
    }
}
