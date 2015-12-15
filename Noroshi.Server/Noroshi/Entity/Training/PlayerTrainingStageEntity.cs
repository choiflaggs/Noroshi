using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Training;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrainingStageSchema;

namespace Noroshi.Server.Entity.Training
{
    /// <summary>
    /// プレイヤーの修行ステージ状態を管理するクラス。
    /// </summary>
    public class PlayerTrainingStageEntity : AbstractDaoWrapperEntity<PlayerTrainingStageEntity, PlayerTrainingStageDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// レコードを参照してビルドする。該当レコードが存在しなければデフォルト値でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trainingStageIds">対象修行ステージ ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerTrainingStageEntity> ReadOrDefaultAndBuildMulti(uint playerId, IEnumerable<uint> trainingStageIds)
        {
            return _instantiate((new PlayerTrainingStageDao()).ReadOrDefaultMulti(playerId, trainingStageIds));
        }
        /// <summary>
        /// レコードを作成してビルドする。既にレコードが存在していればロックを掛けて参照してビルド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="trainingStageId">対象修行ステージ ID</param>
        /// <returns></returns>
        public static PlayerTrainingStageEntity CreateOrReadAndBuild(uint playerId, uint trainingStageId)
        {
            return _instantiate((new PlayerTrainingStageDao()).CreateOrRead(playerId, trainingStageId));
        }


        /// <summary>
        /// 修行ステージ ID。
        /// </summary>
        public uint TrainingStageID => _record.TrainingStageID;
        /// <summary>
        /// スコア。修行種別によって表す内容は変わる。
        /// </summary>
        public uint Score => _record.Score;

        /// <summary>
        /// ハイスコアのセットを試行する。
        /// </summary>
        /// <param name="score">スコア</param>
        /// <returns></returns>
        public bool TryToSetHightScore(uint score)
        {
            if (score <= Score) return false;
            var record = _cloneRecord();
            record.Score = score;
            _changeLocalRecord(record);
            return true;
        }
    }
}
