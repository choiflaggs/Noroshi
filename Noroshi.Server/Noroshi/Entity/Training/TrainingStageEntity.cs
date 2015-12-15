using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Training;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Training;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrainingStageSchema;

namespace Noroshi.Server.Entity.Training
{
    /// <summary>
    /// 修行ステージ設定を扱うクラス。
    /// </summary>
    public class TrainingStageEntity : AbstractDaoWrapperEntity<TrainingStageEntity, TrainingStageDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="id">修行ステージ ID</param>
        /// <returns></returns>
        public static TrainingStageEntity ReadAndBuild(uint id)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { ID = id }));
        }
        /// <summary>
        /// 修行 ID 指定ビルド。
        /// </summary>
        /// <param name="trainingIds">修行 ID</param>
        /// <returns></returns>
        public static IEnumerable<TrainingStageEntity> ReadAndBuildByTrainingIDs(IEnumerable<uint> trainingIds)
        {
            return _loadAssociatedEntities(_instantiate((new TrainingStageDao()).ReadByTrainingIDs(trainingIds)));
        }
        static TrainingStageEntity _loadAssociatedEntities(TrainingStageEntity entity)
        {
            if (entity == null) return null;
            return _loadAssociatedEntities(new[] { entity }).FirstOrDefault();
        }
        static IEnumerable<TrainingStageEntity> _loadAssociatedEntities(IEnumerable<TrainingStageEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var cpuBattleMap = CpuBattleEntity.ReadAndBuildMulti(entities.Select(entity => entity.CpuBattleID)).ToDictionary(cb => cb.ID);
            return entities.Select(entity =>
            {
                entity._setCpuBattle(cpuBattleMap[entity.CpuBattleID]);
                return entity;
            });
        }


        CpuBattleEntity _cpuBattle;

        void _setCpuBattle(CpuBattleEntity cpuBattle)
        {
            if (cpuBattle.ID != CpuBattleID) throw new InvalidOperationException();
            _cpuBattle = cpuBattle;
        }


        /// <summary>
        /// 修行ステージ ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 修行 ID。
        /// </summary>
        public uint TrainingID => _record.TrainingID;
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level => _record.Level;
        /// <summary>
        /// キャラクター経験値係数。
        /// </summary>
        public float CharacterExpCoefficient => _record.CharacterExpCoefficient;
        /// <summary>
        /// ゴールド係数。
        /// </summary>
        public float GoldCoefficient => _record.GoldCoefficient;
        /// <summary>
        /// 必要なプレイヤーレベル。
        /// </summary>
        public ushort NecessaryPlayerLevel => _record.NecessaryPlayerLevel;
        /// <summary>
        /// CPU バトル ID。
        /// </summary>
        public uint CpuBattleID => _record.CpuBattleID;

        /// <summary>
        /// オープンフラグ。
        /// </summary>
        /// <param name="training">親修行</param>
        /// <param name="playerLevel">対象者のプレイヤーレベル</param>
        /// <returns></returns>
        public bool IsOpen(TrainingEntity training, ushort playerLevel)
        {
            if (training.ID != TrainingID) throw new InvalidOperationException();
            return training.IsOpen() && NecessaryPlayerLevel <= playerLevel;
        }

        /// <summary>
        /// 報酬を取得。
        /// </summary>
        /// <param name="training">親修行</param>
        /// <param name="defeatingNum">撃破数</param>
        /// <param name="damage">与ダメージ</param>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetRewards(TrainingEntity training, ushort defeatingNum, uint damage)
        {
            switch (training.Type)
            {
                case TrainingType.DefeatingNum:
                    return GetDefeatingNumRewards(training, defeatingNum);
                case TrainingType.Damage:
                    return GetDamageRewards(training, damage);
                default:
                    throw new SystemException();
            }
        }
        /// <summary>
        /// 撃破数報酬を取得。
        /// </summary>
        /// <param name="training">親修行</param>
        /// <param name="defeatingNum">撃破数</param>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetDefeatingNumRewards(TrainingEntity training, ushort defeatingNum)
        {
            if (training.ID != TrainingID) throw new InvalidOperationException();
            if (training.Type != TrainingType.DefeatingNum) throw new InvalidOperationException();
            return _getRewards(_getDefeatingNumCharacterExp(defeatingNum), _getDefeatingNumGold(defeatingNum));
        }
        /// <summary>
        /// ダメージ報酬を取得。
        /// </summary>
        /// <param name="training">親修行</param>
        /// <param name="damage">与ダメージ</param>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetDamageRewards(TrainingEntity training, uint damage)
        {
            if (training.ID != TrainingID) throw new InvalidOperationException();
            if (training.Type != TrainingType.Damage) throw new InvalidOperationException();
            return _getRewards(_getDamageCharacterExp(damage), _getDamageGold(damage));
        }
        public IEnumerable<PossessionParam> _getRewards(uint characterExp, uint gold)
        {
            var possessionParams = new List<PossessionParam>();
            possessionParams.Add(PossessionManager.GetGoldParam(gold));
            return possessionParams;
        }

        uint _getDefeatingNumCharacterExp(ushort defeatingNum)
        {
            return (uint)(CharacterExpCoefficient * defeatingNum);
        }
        uint _getDefeatingNumGold(ushort defeatingNum)
        {
            return (uint)(GoldCoefficient * defeatingNum);
        }
        uint _getDamageGold(uint damage)
        {
            return (uint)(GoldCoefficient * damage);
        }
        uint _getDamageCharacterExp(uint damage)
        {
            return (uint)(CharacterExpCoefficient * damage);
        }

        public Core.WebApi.Response.Training.TrainingStage ToResponseData(TrainingEntity training, ushort playerLevel, PlayerTrainingStageEntity playerTrainingStage)
        {
            return new Core.WebApi.Response.Training.TrainingStage
            {
                ID = ID,
                Level = Level,
                NecessaryPlayerLevel = NecessaryPlayerLevel,
                IsOpen = IsOpen(training, playerLevel),
                Score = playerTrainingStage.Score,
            };
        }
    }
}
