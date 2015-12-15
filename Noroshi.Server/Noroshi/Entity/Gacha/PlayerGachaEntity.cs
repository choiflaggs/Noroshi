using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Gacha;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerGachaSchema;

namespace Noroshi.Server.Entity.Gacha
{
    /// <summary>
    /// プレイヤーのガチャ利用状況を扱うクラス。ただし、ガチャエントリーポイント経由のガチャのみに利用する。
    /// </summary>
    public class PlayerGachaEntity : AbstractDaoWrapperEntity<PlayerGachaEntity, PlayerGachaDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// レコードが存在しなければ作成しつつロックを掛けてビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="gachaId">対象ガチャ ID</param>
        /// <returns></returns>
        public static PlayerGachaEntity CreateOrReadAndBuild(uint playerId, uint gachaId)
        {
            return _instantiate((new PlayerGachaDao()).CreateOrRead(playerId, gachaId));
        }

        public uint PlayerID => _record.PlayerID;
        public uint GachaID => _record.GachaID;
        /// <summary>
        /// 累積当たり回数。
        /// </summary>
        public uint HitNum => _record.HitNum;
        /// <summary>
        /// 前回の当たりからの外れ回数。
        /// </summary>
        public uint MissLotNum => _record.MissLotNum;

        /// <summary>
        /// 抽選回数をインクリメントする。
        /// </summary>
        /// <param name="isHit">大当たりしているかどうか</param>
        public void IncrementLotNum(bool isHit)
        {
            var newRecord = _cloneRecord();
            if (isHit)
            {
                newRecord.MissLotNum = 0;
                newRecord.HitNum++;
            }
            else
            {
                newRecord.MissLotNum++;
            }
            _changeLocalRecord(newRecord);
        }
    }
}
