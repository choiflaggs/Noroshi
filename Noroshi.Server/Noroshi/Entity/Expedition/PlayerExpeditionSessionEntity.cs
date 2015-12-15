using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Expedition;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Gacha;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Expedition;
using Noroshi.Server.Entity.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerExpeditionSessionSchema;

namespace Noroshi.Server.Entity.Expedition
{
    /// <summary>
    /// 冒険中の状態を管理するクラス。
    /// </summary>
    public class PlayerExpeditionSessionEntity : AbstractDaoWrapperEntity<PlayerExpeditionSessionEntity, PlayerExpeditionSessionDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerExpeditionSessionEntity ReadAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId }, readType));
        }
        /// <summary>
        /// レコードを参照してビルドする。該当レコードが存在しなければデフォルト値でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerExpeditionSessionEntity ReadOrDefaultAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return _loadAssociatedEntities(_instantiate((new PlayerExpeditionSessionDao()).ReadOrDefault(playerId, readType)));
        }
        /// <summary>
        /// レコードを作成してビルドする。既にレコードが存在していればロックを掛けて参照してビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <returns></returns>
        public static PlayerExpeditionSessionEntity CreateOrRead(uint playerId)
        {
            return _loadAssociatedEntities(_instantiate((new PlayerExpeditionSessionDao()).CreateOrRead(playerId)));
        }
        static PlayerExpeditionSessionEntity _loadAssociatedEntities(PlayerExpeditionSessionEntity entity)
        {
            if (entity == null) throw new InvalidOperationException();
            if (!entity.IsActive) return entity;
            var expedition = ExpeditionEntity.ReadAndBuild(entity.ExpeditionID.Value);
            if (expedition == null)
            {
                throw new SystemException(string.Join("\t", "Expedition Not Found", entity.ExpeditionID.Value));
            }
            var gachaContentIds = entity.GetAllRewardGachaContentIDs();
            var gachaContents = gachaContentIds.Count() > 0 ? GachaContentEntity.ReadAndBuildMulti(gachaContentIds) : new GachaContentEntity[0];
            entity._setExpeditionAndGachaContents(expedition, gachaContents);
            return entity;
        }


        ExpeditionEntity _expedition;
        Dictionary<uint, GachaContentEntity> _gachaContentMap;

        void _setExpeditionAndGachaContents(ExpeditionEntity expedition, IEnumerable<GachaContentEntity> gachaContents)
        {
            if (!IsActive || expedition.ID != ExpeditionID.Value) throw new InvalidOperationException();
            _expedition = expedition;
            _gachaContentMap = gachaContents.ToDictionary(gc => gc.ID);
        }

        /// <summary>
        /// プレイヤー ID。
        /// </summary>
        public uint PlayerID => _record.PlayerID;
        /// <summary>
        /// 冒険 ID。冒険中でない場合は null。
        /// </summary>
        public uint? ExpeditionID => IsActive ? (uint?)_record.ExpeditionID : null;
        /// <summary>
        /// 現在の冒険におけるクリアステップ。冒険中でない場合は null。
        /// </summary>
        public byte? ClearStep => IsActive ? (byte?)_record.ClearStep : null;
        /// <summary>
        /// 冒険状態。リセット時間をまたいでいた場合、強制的に Inactive になる。
        /// </summary>
        public PlayerExpeditionSessionState State => !ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.StartedAt) ? (PlayerExpeditionSessionState)_record.State : PlayerExpeditionSessionState.Inactive;

        /// <summary>
        /// 冒険中かどうか判定。
        /// </summary>
        public bool IsActive => (PlayerExpeditionSessionState)_record.State != PlayerExpeditionSessionState.Inactive;
        /// <summary>
        /// 報酬受け取り可否。
        /// </summary>
        public bool CanReceiveReward => State == PlayerExpeditionSessionState.OpenReward;

        /// <summary>
        /// 冒険設定取得。
        /// </summary>
        /// <returns></returns>
        public ExpeditionEntity GetExpedition()
        {
            if (_expedition == null) throw new InvalidOperationException();
            return _expedition;
        }
        /// <summary>
        /// 冒険ステージ設定取得。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExpeditionStageEntity> GetAllExpeditionStages()
        {
            return GetExpedition().GetAllStages();
        }
        /// <summary>
        /// 次のステージ ID を取得。
        /// </summary>
        /// <returns></returns>
        public uint? GetNextStageID()
        {
            return _expedition.GetStage((byte)(ClearStep + 1))?.ID;
        }
        /// <summary>
        /// 次のステージの対戦相手プレイヤー ID を取得。
        /// </summary>
        /// <returns></returns>
        public uint? GetNextStageEnemyPlayerID()
        {
            return GetStageSession((byte)(ClearStep + 1))?.PlayerID;
        }

        /// <summary>
        /// 全敵のプレイヤー ID を取得。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetAllEnemyPlayerIDs()
        {
            return GetAllStageSessions().Select(d => d.PlayerID).Distinct();
        }
        /// <summary>
        /// 全敵のプレイヤーキャラクター ID を取得。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetAllEnemyPlayerCharacterIDs()
        {
            return GetAllStageSessions().SelectMany(d => d.PlayerCharacterIDs).Distinct();
        }
        /// <summary>
        /// 全ての報酬ガチャコンテンツ ID を取得。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetAllRewardGachaContentIDs()
        {
            return GetAllStageSessions().Where(d => d.GachaContentIDs.Count() > 0).SelectMany(d => d.GachaContentIDs).Distinct();
        }
        /// <summary>
        /// ステージ情報を取得。
        /// </summary>
        /// <param name="step">対象ステップ</param>
        /// <returns></returns>
        public StageSession GetStageSession(byte step)
        {
            return GetAllStageSessions().Where(data => data.Step == step).FirstOrDefault();
        }
        /// <summary>
        /// 全てのステージ情報を取得。
        /// </summary>
        /// <returns></returns>
        public StageSession[] GetAllStageSessions()
        {
            if (!IsActive) throw new InvalidOperationException();
            return _deserializeFromText<StageData>(_record.StageData).StageSessions;
        }

        /// <summary>
        /// バトルに適用する初期状態を取得する。
        /// </summary>
        /// <param name="playerCharacterIds">バトルに連れていくプレイヤーキャラクター ID</param>
        /// <returns></returns>
        public InitialCondition GetInitialCondition(uint[] playerCharacterIds)
        {
            var map = GetOwnPlayerCharacterCondition();
            return new InitialCondition
            {
                OwnPlayerCharacterConditions = playerCharacterIds.Where(id => map.ContainsKey(id)).Select(id => map[id]).ToArray(),
            };
        }
        /// <summary>
        /// プレイヤーキャラクター状態取得。
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, InitialCondition.PlayerCharacterCondition> GetOwnPlayerCharacterCondition()
        {
            return _deserializeFromText<Dictionary<uint, InitialCondition.PlayerCharacterCondition>>(_record.PlayerCharacterData) ?? new Dictionary<uint, InitialCondition.PlayerCharacterCondition>();
        }
        /// <summary>
        /// 次回バトルに適用するための自プレイヤーキャラクター初期状態を更新する。
        /// </summary>
        /// <param name="initialOwnPlayerCharacterConditions"></param>
        public void SetInitialOwnPlayerCharacterConditions(IEnumerable<InitialCondition.PlayerCharacterCondition> initialOwnPlayerCharacterConditions)
        {
            var playerCharacterData = GetOwnPlayerCharacterCondition();
            foreach (var playerCharacterCondition in initialOwnPlayerCharacterConditions)
            {
                if (playerCharacterData.ContainsKey(playerCharacterCondition.PlayerCharacterID))
                {
                    playerCharacterData[playerCharacterCondition.PlayerCharacterID] = playerCharacterCondition;
                }
                else
                {
                    playerCharacterData.Add(playerCharacterCondition.PlayerCharacterID, playerCharacterCondition);
                }
            }
            var record = _cloneRecord();
            record.PlayerCharacterData = _serializeToText(playerCharacterData);
            _changeLocalRecord(record);
        }

        /// <summary>
        /// 冒険を開始する。
        /// </summary>
        /// <param name="expedition">対象冒険</param>
        /// <param name="stageData">ステージ情報</param>
        /// <returns></returns>
        public void Start(ExpeditionEntity expedition, StageData stageData)
        {
            var record = _cloneRecord();
            record.ExpeditionID = expedition.ID;
            record.ClearStep = 0;
            record.State = (byte)PlayerExpeditionSessionState.Battle;
            record.StageData = _serializeToText(stageData);
            record.PlayerCharacterData = "";
            record.StartedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
            // 必要な情報をロード。
            var gachaContentIds = GetAllRewardGachaContentIDs();
            var gachaContents = gachaContentIds.Count() > 0 ? GachaContentEntity.ReadAndBuildMulti(gachaContentIds) : new GachaContentEntity[0];
            _setExpeditionAndGachaContents(expedition, gachaContents);
        }
        /// <summary>
        /// 現在の冒険をやめる。
        /// </summary>
        /// <returns></returns>
        public void Reset()
        {
            var record = _cloneRecord();
            record.ExpeditionID = 0;
            record.ClearStep = 0;
            record.State = (byte)PlayerExpeditionSessionState.Inactive;
            record.StageData = "";
            record.PlayerCharacterData = "";
            record.StartedAt = 0;
            _changeLocalRecord(record);
        }

        /// <summary>
        /// 報酬を獲得できる状態にする。
        /// </summary>
        public void OpenReward()
        {
            var record = _cloneRecord();
            record.State = (byte)PlayerExpeditionSessionState.OpenReward;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// 報酬受け取り状態にする（現ステップをクリア状態にする）。
        /// </summary>
        public void ReceiveReward()
        {
            var record = _cloneRecord();
            record.ClearStep++;
            record.State = (byte)PlayerExpeditionSessionState.Battle;
            _changeLocalRecord(record);
        }

        /// <summary>
        /// 現報酬取得。
        /// </summary>
        /// <returns></returns>
        public List<PossessionParam> GetCurrentRewards(PlayerVipLevelBonusEntity playerVipLevelBonus)
        {
            if (!ClearStep.HasValue) throw new InvalidOperationException();
            return GetRewards((byte)(ClearStep.Value + 1), playerVipLevelBonus);
        }
        /// <summary>
        /// 報酬取得
        /// </summary>
        /// <param name="step">対象ステップ</param>
        /// <returns></returns>
        public List<PossessionParam> GetRewards(byte step, PlayerVipLevelBonusEntity playerVipLevelBonus)
        {
            var stage = GetStageSession(step);
            var rewards = new List<PossessionParam>();
            if (stage == null) return rewards;
            if (stage.Gold > 0) rewards.Add(PossessionManager.GetGoldParam((uint)(stage.Gold * (playerVipLevelBonus.ExpeditionGoldBonus + 1.00))));
            if (stage.ExpeditionPoint > 0)
            {
                rewards.Add(PossessionManager.GetExpeditionPointParam((uint)(stage.ExpeditionPoint * (playerVipLevelBonus.ExpeditionPointBonus + 1.00))));
            }
            if (stage.GachaContentIDs.Count() > 0)
            {
                rewards.AddRange(stage.GachaContentIDs.Select(gcid => _gachaContentMap[gcid].GetPossessableParam()));
            }
            return rewards;
        }
        /// <summary>
        /// 全報酬取得。
        /// </summary>
        /// <returns></returns>
        public List<PossessionParam> GetAllRewards(PlayerVipLevelBonusEntity playerVipLevelBonus)
        {
            return GetAllStageSessions().SelectMany(stage => GetRewards(stage.Step, playerVipLevelBonus)).ToList();
        }

        public class StageData
        {
            public StageSession[] StageSessions;
        }
        public class StageSession
        {
            public byte Step;
            public uint PlayerID;
            public uint[] PlayerCharacterIDs;
            public uint[] GachaContentIDs;
            public uint Gold;
            public uint ExpeditionPoint;
        }
    }
}
