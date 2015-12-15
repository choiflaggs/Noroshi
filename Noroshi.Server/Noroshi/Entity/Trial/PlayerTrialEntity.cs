using System;
using System.Collections.Generic;
using Noroshi.Core.Game.Trial;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Traial;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrialSchema;

namespace Noroshi.Server.Entity.Trial
{
    /// <summary>
    /// プレイヤーの試練状態を管理するクラス。
    /// </summary>
    public class PlayerTrialEntity : AbstractDaoWrapperEntity<PlayerTrialEntity, PlayerTrialDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// レコードを参照してビルドする。該当レコードが存在しなければデフォルト値でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trialIds">対象試練 ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerTrialEntity> ReadOrDefaultAndBuildMulti(uint playerId, IEnumerable<uint> trialIds)
        {
            return _instantiate((new PlayerTrialDao()).ReadOrDefaultMulti(playerId, trialIds));
        }
        /// <summary>
        /// レコードを作成してビルドする。既にレコードが存在していればロックを掛けて参照してビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trialId">対象試練 ID</param>
        /// <returns></returns>
        public static PlayerTrialEntity CreateOrReadAndBuild(uint playerId, uint trialId)
        {
            return _instantiate((new PlayerTrialDao()).CreateOrRead(playerId, trialId));
        }


        /// <summary>
        /// 試練 ID。
        /// </summary>
        public uint TrialID => _record.TrialID;
        /// <summary>
        /// クリア済みレベル。
        /// </summary>
        public byte ClearLevel => _record.ClearLevel;
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
        /// <param name="trial">対象試練</param>
        /// <param name="trialStage">対象試練ステージ</param>
        /// <returns></returns>
        public bool CanBattle(TrialEntity trial, TrialStageEntity trialStage)
        {
            if (trial.ID != TrialID || trialStage.TrialID != TrialID) throw new InvalidOperationException();
            if (!trialStage.IsOpen(trial, this)) return false;
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

        public bool TryToSetClearLevel(byte clearLevel)
        {
            if (clearLevel <= ClearLevel) return false;
            var record = _cloneRecord();
            record.ClearLevel = clearLevel;
            _changeLocalRecord(record);
            return true;
        }
    }
}
