using System;
using System.Collections.Generic;
using Noroshi.Core.Game.Training;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Training;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrainingSchema;

namespace Noroshi.Server.Entity.Training
{
    /// <summary>
    /// プレイヤーの修行状態を管理するクラス。
    /// </summary>
    public class PlayerTrainingEntity : AbstractDaoWrapperEntity<PlayerTrainingEntity, PlayerTrainingDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// レコードを参照してビルドする。該当レコードが存在しなければデフォルト値でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trainingIds">対象修行 ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerTrainingEntity> ReadOrDefaultAndBuildMulti(uint playerId, IEnumerable<uint> trainingIds)
        {
            return _instantiate((new PlayerTrainingDao()).ReadOrDefaultMulti(playerId, trainingIds));
        }
        /// <summary>
        /// レコードを作成してビルドする。既にレコードが存在していればロックを掛けて参照してビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trainingId">対象修行 ID</param>
        /// <returns></returns>
        public static PlayerTrainingEntity CreateOrReadAndBuild(uint playerId, uint trainingId)
        {
            return _instantiate((new PlayerTrainingDao()).CreateOrRead(playerId, trainingId));
        }


        /// <summary>
        /// 修行 ID。
        /// </summary>
        public uint TrainingID => _record.TrainingID;
        /// <summary>
        /// 再度開く日時。
        /// </summary>
        public uint? ReopenedAt => _record.ReopenedAt > 0 ? (uint?)_record.ReopenedAt : null;

        /// <summary>
        /// バトル回数。
        /// </summary>
        public byte BattleNum => ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastBattledAt) ? (byte)0 : _record.LastBattleNum;

        /// <summary>
        /// バトル可否チェック。
        /// </summary>
        /// <param name="training">対象修行</param>
        /// <param name="trainingStage">対象修行ステージ</param>
        /// <param name="playerLevel">バトル実行者プレイヤーレベル</param>
        /// <returns></returns>
        public bool CanBattle(TrainingEntity training, TrainingStageEntity trainingStage, ushort playerLevel)
        {
            if (training.ID != TrainingID || trainingStage.TrainingID != TrainingID) throw new InvalidOperationException();
            if (!trainingStage.IsOpen(training, playerLevel)) return false;
            return BattleNum < Constant.MAX_BATTLE_NUM && (!ReopenedAt.HasValue || ReopenedAt <= ContextContainer.GetContext().TimeHandler.UnixTime);
        }

        /// <summary>
        /// バトル回数インクリメント。
        /// </summary>
        public void IncrementBattleNum()
        {
            _setBattleNum((byte)(BattleNum + 1));
            var record = _cloneRecord();
            record.ReopenedAt = ContextContainer.GetContext().TimeHandler.UnixTime + (uint)Constant.BATTLE_COOL_TIME.TotalSeconds;
            _changeLocalRecord(record);
        }
        void _setBattleNum(byte battleNum)
        {
            var record = _cloneRecord();
            record.LastBattleNum = battleNum;
            record.LastBattledAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }
    }
}
