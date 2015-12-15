using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Gacha;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerGachaEntryPointSchema;

namespace Noroshi.Server.Entity.Gacha
{
    /// <summary>
    /// プレイヤーのガチャエントリポイント利用状況を扱うクラス。
    /// </summary>
    public class PlayerGachaEntryPointEntity : AbstractDaoWrapperEntity<PlayerGachaEntryPointEntity, PlayerGachaEntiryPointDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerGachaEntryPointEntity ReadAndBuild(uint playerId, uint gachaEntryPointId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildByPlayerIDAndGachaEntryPointIDs(playerId, new[] { gachaEntryPointId }, readType).FirstOrDefault();
        }
        public static IEnumerable<PlayerGachaEntryPointEntity> ReadAndBuildByPlayerIDAndGachaEntryPointIDs(uint playerId, IEnumerable<uint> gachaEntryPointIds, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(gachaEntryPointIds.Select(gachaEntryPointId => new Schema.PrimaryKey { PlayerID = playerId, GachaEntryPointID = gachaEntryPointId }), readType);
        }
        /// <summary>
        /// レコードが存在しなければ作成せずにビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="gachaEntryPointIds">対象ガチャエントリーポイント ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerGachaEntryPointEntity> ReadOrDefaultBuildByPlayerIDAndGachaEntryPointIDs(uint playerId, IEnumerable<uint> gachaEntryPointIds)
        {
            return _instantiate((new PlayerGachaEntiryPointDao()).ReadOrDefaultByPlayerIDAndGachaEntryPointIDs(playerId, gachaEntryPointIds));
        }
        /// <summary>
        /// レコードが存在しなければ作成しつつロックを掛けてビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="gachaEntryPointId">対象ガチャエントリーポイント ID</param>
        /// <returns></returns>
        public static PlayerGachaEntryPointEntity CreateOrReadAndBuild(uint playerId, uint gachaEntryPointId)
        {
            return _instantiate((new PlayerGachaEntiryPointDao()).CreateOrRead(playerId, gachaEntryPointId));
        }


        PlayerGachaEntity _playerGacha = null;

        public uint GachaEntryPointID => _record.GachaEntryPointID;
        /// <summary>
        /// 無料抽選数（リセットあり）。
        /// </summary>
        public byte FreeLotNum => ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastFreeLottedAt) ? (byte)0 : _record.LastFreeLotNum;
        /// <summary>
        /// 無料抽選が次に開く日時。
        /// </summary>
        public uint FreeReopenedAt => _record.FreeReopenedAt;

        /// <summary>
        /// 抽選可否チェック。
        /// </summary>
        /// <param name="entryPoint">対象ガチャエントリーポイント</param>
        /// <param name="free">無料試行かどうか</param>
        /// <returns></returns>
        public bool CanLot(GachaEntryPointEntity entryPoint, bool free)
        {
            if (entryPoint.ID != GachaEntryPointID) throw new InvalidOperationException();
            // オープンしているか
            if (!entryPoint.IsOpen()) return false;
            // 最大抽選数以内か
            if (entryPoint.MaxTotalLotNum.HasValue && entryPoint.MaxTotalLotNum.Value <= _record.TotalLotNum) return false;
            if (free)
            {
                // 無料で引けるガチャではない。
                if (!entryPoint.IsFreeLottable()) return false;
                // オープンしているか。
                if (ContextContainer.GetContext().TimeHandler.UnixTime < FreeReopenedAt) return false;
                // 最大数未満か。
                if (entryPoint.MaxDailyFreeLotNum.HasValue && entryPoint.MaxDailyFreeLotNum.Value <= FreeLotNum) return false;
            }
            return true;
        }

        /// <summary>
        /// 抽選する。
        /// </summary>
        /// <param name="entryPoint">対象ガチャエントリーポイント</param>
        /// <param name="free">無料試行かどうか</param>
        /// <returns></returns>
        public List<GachaContentEntity> Lot(GachaEntryPointEntity entryPoint, bool free)
        {
            if (entryPoint.ID != GachaEntryPointID) throw new InvalidOperationException();

            // プレイヤーガチャもビルドしておく。抽選時、内部ロジックでしか利用しないのでここでビルドで十分。
            _playerGacha = PlayerGachaEntity.CreateOrReadAndBuild(_record.PlayerID, entryPoint.GachaID);

            var hitNumToGuaranteedLotMap = GachaGuaranteedLotEntity.ReadAndBuildByGachaID(entryPoint.GachaID)
                .ToDictionary(ggl => ggl.HitNum);

            var lottedContents = new List<GachaContentEntity>();
            for (var i = 0; i < entryPoint.LotNum; i++)
            {
                var lottedContent = entryPoint.Lot(1, (content, no) =>
                {
                    // 保証付きの場合、最初の一回は保証内容でフィルタリング。
                    if (entryPoint.GuaranteedPossessionCategory.HasValue && i == 0)
                    {
                        return entryPoint.GuaranteedPossessionCategory.Value == content.GetPossessableParam().Category;
                    }
                    // 最低保証ライン設定が存在し、それ以上にヒットしていなければフィルタリング。
                    if (hitNumToGuaranteedLotMap.ContainsKey(_playerGacha.HitNum) && hitNumToGuaranteedLotMap[_playerGacha.HitNum].MissLotNum <= _playerGacha.MissLotNum)
                    {
                        var guaranteedPossessionCategory = hitNumToGuaranteedLotMap[_playerGacha.HitNum].GuaranteedPossessionCategory;
                        return guaranteedPossessionCategory == content.GetPossessableParam().Category;
                    }
                    return true;
                })
                .First();
                // 抽選数インクリメント。
                _incrementLotNum(entryPoint.IsHit(lottedContent));
                lottedContents.Add(lottedContent);
            }
            // 無料試行時の無料抽選数カウントアップ。
            if (free) _countUpFreeLot(entryPoint.LotNum, entryPoint.FreeLotCoolTime.Value);

            return lottedContents;
        }

        /// <summary>
        /// 抽選回数をインクリメントする。
        /// </summary>
        /// <param name="isHit">大当たりしているかどうか</param>
        void _incrementLotNum(bool isHit)
        {
            // プレイヤーガチャの方もインクリメントする。
            _playerGacha.IncrementLotNum(isHit);
            var newRecord = _cloneRecord();
            newRecord.TotalLotNum++;
            _changeLocalRecord(newRecord);
        }
        /// <summary>
        /// 無料抽選情報をセットする。
        /// </summary>
        /// <param name="prepareTime">無料抽選から次に開放される期間</param>
        void _countUpFreeLot(byte lotNum, TimeSpan prepareTime)
        {
            var newRecord = _cloneRecord();
            newRecord.FreeReopenedAt = ContextContainer.GetContext().TimeHandler.UnixTime + (uint)prepareTime.TotalSeconds;
            newRecord.LastFreeLotNum = (byte)(FreeLotNum + lotNum);
            newRecord.LastFreeLottedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
        }

        public override bool Save()
        {
            // 内部的にしか利用していないのでここで Save してしまう。
            if (!_playerGacha.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Gacha", _playerGacha.PlayerID, _playerGacha.GachaID));
            }
            return base.Save();
        }
    }
}
