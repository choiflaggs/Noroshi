using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Expedition;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Gacha;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerExpeditionSchema;

namespace Noroshi.Server.Entity.Expedition
{
    /// <summary>
    /// 冒険プレイヤー状態を管理するクラス。
    /// </summary>
    public class PlayerExpeditionEntity : AbstractDaoWrapperEntity<PlayerExpeditionEntity, PlayerExpeditionDao, Schema.PrimaryKey, Schema.Record>
    {
        const byte LEVEL_RANGE_SEARCH_ROW_COUNT_PER_QUERY = 30;

        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerExpeditionEntity ReadAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId }, readType);
        }
        /// <summary>
        /// レコードを参照してビルドする。該当レコードが存在しなければデフォルト値でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerExpeditionEntity ReadOrDefaultAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerExpeditionDao()).ReadOrDefault(playerId, readType));
        }
        /// <summary>
        /// レコードを作成してビルドする。既にレコードが存在していればロックを掛けて参照してビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <returns></returns>
        public static PlayerExpeditionEntity CreateOrRead(uint playerId)
        {
            return _instantiate((new PlayerExpeditionDao()).CreateOrRead(playerId));
        }

        /// <summary>
        /// プレイヤー ID。
        /// </summary>
        public uint PlayerID => _record.PlayerID;
        /// <summary>
        /// クリア済み冒険レベル。
        /// </summary>
        public byte ClearLevel => _record.ClearLevel;
        /// <summary>
        /// リセット回数
        /// </summary>
        public byte ResetNum => ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastResetedAt) ? (byte)0 : _record.LastResetNum;

        /// <summary>
        /// 最大リセット回数。
        /// </summary>
        /// <param name="vipLevel"></param>
        /// <returns></returns>
        public byte GetMaxResetNum(ushort vipLevel)
        {
            var playerVipLevelBonus = PlayerVipLevelBonusEntity.ReadAndBuild(vipLevel);
            return playerVipLevelBonus.MaxResetExpeditionNum;
        }

        /// <summary>
        /// ステージデータを作成する。負荷が高いメソッド。
        /// </summary>
        /// <param name="playerId">プレイヤー ID</param>
        /// <param name="expedition">対象冒険</param>
        /// <returns></returns>
        public PlayerExpeditionSessionEntity.StageData MakeStageData(uint playerId, ExpeditionEntity expedition)
        {
            var stages = expedition.GetAllStages().ToArray();
            var gachaMap = GachaEntity.ReadAndBuildMulti(stages.Select(s => s.GachaID)).ToDictionary(g => g.ID);
            var playerIds = _searchEnemyPlayerIDs(playerId, stages).ToArray();
            var stageSessions = new List<PlayerExpeditionSessionEntity.StageSession>();
            var playerIdToPlayerCharacterIds = PlayerArenaEntity.GetPlayerCharactersInDeckOrGetVirtualDeck(playerIds);
            for (var i = 0; i < playerIds.Length; i++)
            {
                stageSessions.Add(new PlayerExpeditionSessionEntity.StageSession
                {
                    Step = stages[i].Step,
                    PlayerID = playerIds[i],
                    PlayerCharacterIDs = playerIdToPlayerCharacterIds[playerIds[i]].ToArray(),
                    GachaContentIDs = gachaMap[stages[i].GachaID].Lot(stages[i].GachaLotNum).Select(c => c.ID).ToArray(),
                    Gold = stages[i].Gold,
                    ExpeditionPoint = stages[i].ExpeditionPoint,
                });
            }
            return new PlayerExpeditionSessionEntity.StageData { StageSessions = stageSessions.ToArray() };
        }
        IEnumerable<uint> _searchEnemyPlayerIDs(uint searcherPlayerId, IEnumerable<ExpeditionStageEntity> stages)
        {
            var playerIds = new List<uint>();
            // どうしても対象プレイヤーが見つからなかった時に差し込むトッププレイヤー。
            var poolTopPlayerIds = PlayerStatusEntity.ReadAndBuildByLevelRange(1, (ushort)PlayerLevelEntity.GetMaxLevel(), LEVEL_RANGE_SEARCH_ROW_COUNT_PER_QUERY).Where(ps => ps.PlayerID != searcherPlayerId).Select(ps => ps.PlayerID).ToArray();
            var poolTopPlayerIndex = 0;
            var levelToPlayerIds = new Dictionary<ushort, Queue<uint>>();
            // 多くのクエリを発行するが一旦気にしない。
            // 本メソッド内でキャッシュする選択肢もあるが、CPU を犠牲にする部分もあるので一旦見送り。
            // 負荷が高くなってきたら本メソッド内でキャッシュするか、Redis に載せるかを考える。
            foreach (var stage in stages)
            {
                var playerStatuses = PlayerStatusEntity.ReadAndBuildByLevelRange(stage.MinPlayerLevel, stage.MaxPlayerLevel, LEVEL_RANGE_SEARCH_ROW_COUNT_PER_QUERY)
                    .Where(ps => ps.PlayerID != searcherPlayerId);
                if (playerStatuses.Count() > 0)
                {
                    var queue = new Queue<PlayerStatusEntity>(playerStatuses);
                    playerIds.Add(playerStatuses.First().PlayerID);
                }
                else if (poolTopPlayerIds.Length > 0)
                {
                    playerIds.Add(poolTopPlayerIds[poolTopPlayerIndex++]);
                    if (poolTopPlayerIndex == poolTopPlayerIds.Length) poolTopPlayerIndex = 0;
                }
            }
            return playerIds;
        }

        /// <summary>
        /// 冒険開始可否チェック。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        /// <param name="expedition">対象冒険</param>
        /// <returns></returns>
        public bool CanStart(PlayerExpeditionSessionEntity session, ExpeditionEntity expedition)
        {
            if (session.PlayerID != PlayerID) throw new InvalidOperationException();
            // クリア済みレベルの一つ上まで許可。
            if (_record.ClearLevel + 1 < expedition.Level) return false;
            // セッション状態がアクティブだと開始付加。
            if (session.IsActive) return false;
            return true;
        }
        /// <summary>
        /// 冒険開始。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        /// <param name="expedition">対象冒険</param>
        /// <param name="stageData">事前に生成したステージ情報</param>
        public void Start(PlayerExpeditionSessionEntity session, ExpeditionEntity expedition, PlayerExpeditionSessionEntity.StageData stageData)
        {
            if (session.PlayerID != PlayerID) throw new InvalidOperationException();
            session.Start(expedition, stageData);
        }

        /// <summary>
        /// 報酬受け取り可否チェック。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        /// <returns></returns>
        public bool CanReceiveReward(PlayerExpeditionSessionEntity session)
        {
            if (session.PlayerID != PlayerID) throw new InvalidOperationException();
            return session.CanReceiveReward;
        }
        /// <summary>
        /// 報酬受け取り。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        public void ReceiveReward(PlayerExpeditionSessionEntity session)
        {
            if (session.PlayerID != PlayerID) throw new InvalidOperationException();
            session.ReceiveReward();
            // 試練レベルアップ
            if (session.ClearStep == session.GetExpedition().GetMaxStep() && session.GetExpedition().Level > _record.ClearLevel)
            {
                var newRecord = _cloneRecord();
                newRecord.ClearLevel++;
                _changeLocalRecord(newRecord);
            }
        }

        /// <summary>
        /// リセット可否チェック。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        /// <param name="vipLevel">プレイヤー VIP レベル</param>
        /// <returns></returns>
        public bool CanReset(PlayerExpeditionSessionEntity session, ushort vipLevel)
        {
            if (session.PlayerID != PlayerID) throw new InvalidOperationException();
            if (!session.IsActive) return false;
            if (ResetNum >= GetMaxResetNum(vipLevel)) return false;
            return true;
        }
        /// <summary>
        /// リセット。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        public void Reset(PlayerExpeditionSessionEntity session)
        {
            if (session.PlayerID != PlayerID) throw new InvalidOperationException();
            var record = _cloneRecord();
            record.LastResetNum = (byte)(ResetNum + 1);
            record.LastResetedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
            session.Reset();
        }

        /// <summary>
        /// レスポンスデータへ変換。
        /// </summary>
        /// <param name="session">プレイヤー冒険セッション</param>
        /// <param name="possessionManager"></param>
        /// <returns></returns>
        public Core.WebApi.Response.Expedition.PlayerExpedition ToResponseData(ushort vipLevel, PlayerExpeditionSessionEntity session, PossessionManager possessionManager, PlayerVipLevelBonusEntity playerVipLevelBonus)
        {
            if (session.IsActive)
            {
                var playerStatusMap = PlayerStatusEntity.ReadAndBuildMulti(session.GetAllEnemyPlayerIDs()).ToDictionary(ps => ps.PlayerID);
                var playerCharacterMap = PlayerCharacterEntity.ReadAndBuildMulti(session.GetAllEnemyPlayerCharacterIDs()).ToDictionary(pc => pc.ID);
                var stages = session.GetAllExpeditionStages().ToArray();
                var stageResponses = new List<Core.WebApi.Response.Expedition.PlayerExpeditionStage>();
                foreach (var stage in stages)
                {
                    var stageSession = session.GetStageSession(stage.Step);
                    var enemyPlayerStatus = playerStatusMap[stageSession.PlayerID];
                    var playerCharacters = stageSession.PlayerCharacterIDs.Select(id => playerCharacterMap[id]);
                    var rewards = possessionManager.GetPossessionObjects(session.GetRewards(stage.Step, playerVipLevelBonus));
                    stageResponses.Add(_toStageResponseData(stage, enemyPlayerStatus, playerCharacters, rewards));
                }
                return new Core.WebApi.Response.Expedition.PlayerExpedition
                {
                    IsActive = session.IsActive,
                    ExpeditionID = session.ExpeditionID,
                    ClearStep = session.ClearStep,
                    CanReceiveReward = session.CanReceiveReward,
                    Stages = stageResponses.ToArray(),
                    PlayerCharacterConditions = session.GetOwnPlayerCharacterCondition().Values.ToArray(),
                    ResetNum = ResetNum,
                    MaxResetNum = GetMaxResetNum(vipLevel),
                };
            }
            return new Core.WebApi.Response.Expedition.PlayerExpedition
            {
                IsActive = session.IsActive,
                CanReceiveReward = false,
                Stages = new Core.WebApi.Response.Expedition.PlayerExpeditionStage[0],
                PlayerCharacterConditions = new InitialCondition.PlayerCharacterCondition[0],
                ResetNum = ResetNum,
                MaxResetNum = GetMaxResetNum(vipLevel),
            };
        }
        Core.WebApi.Response.Expedition.PlayerExpeditionStage _toStageResponseData(ExpeditionStageEntity stage, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> playerCharacters, IEnumerable<IPossessionObject> rewards)
        {
            return new Core.WebApi.Response.Expedition.PlayerExpeditionStage
            {
                ID = stage.ID,
                Step = stage.Step,
                PlayerName = playerStatus.Name,
                PlayerLevel = playerStatus.Level,
                PlayerCharacters = playerCharacters.Select(pc => pc.ToResponseData()).ToArray(),
                Rewards = rewards.Select(reward => reward.ToResponseData()).ToArray(),
            };
        }
    }
}
