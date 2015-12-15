using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Trial;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrialStageSchema;

namespace Noroshi.Server.Entity.Trial
{
    /// <summary>
    /// プレイヤーの試練ステージ状態を管理するクラス。
    /// </summary>
    public class PlayerTrialStageEntity : AbstractDaoWrapperEntity<PlayerTrialStageEntity, PlayerTrialStageDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// レコードを参照してビルドする。該当レコードが存在しなければデフォルト値でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trialStageIds">対象試練ステージ ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerTrialStageEntity> ReadOrDefaultAndBuildMulti(uint playerId, IEnumerable<uint> trialStageIds)
        {
            return _instantiate((new PlayerTrialStageDao()).ReadOrDefaultMulti(playerId, trialStageIds));
        }
        /// <summary>
        /// レコードを作成してビルドする。既にレコードが存在していればロックを掛けて参照してビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trialStageId">対象試練ステージ ID</param>
        /// <returns></returns>
        public static PlayerTrialStageEntity CreateOrReadAndBuild(uint playerId, uint trialStageId)
        {
            return _instantiate((new PlayerTrialStageDao()).CreateOrRead(playerId, trialStageId));
        }


        /// <summary>
        /// 試練ステージ ID。
        /// </summary>
        public uint TrialStageID => _record.TrialStageID;
        /// <summary>
        /// ランク。
        /// </summary>
        public byte Rank => _record.Rank;

        /// <summary>
        /// 最高ランクのセットを試行する。
        /// </summary>
        /// <param name="rank">ランク</param>
        /// <returns></returns>
        public bool TryToSetHightRank(byte rank)
        {
            if (rank <= Rank) return false;
            var record = _cloneRecord();
            record.Rank = rank;
            _changeLocalRecord(record);
            return true;
        }
    }
}
