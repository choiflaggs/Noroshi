using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Gacha;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaEntryPointSchema;

namespace Noroshi.Server.Entity.Gacha
{
    public class GachaEntryPointEntity : AbstractDaoWrapperEntity<GachaEntryPointEntity, GachaEntryPointDao, Schema.PrimaryKey, Schema.Record>
    {
        const uint TUTORIAL_GACHA_ENTRY_POINT_ID = 5;

        public static IEnumerable<GachaEntryPointEntity> ReadAndBuildAllWithoutTutorialEntryPoint()
        {
            // チュートリアル用は除く。
            var entities = (new GachaEntryPointDao()).ReadAll().Select(r => _instantiate(r)).Where(entity => entity.ID != TUTORIAL_GACHA_ENTRY_POINT_ID);
            return _loadAssociatedEntities(entities);
        }
        public static GachaEntryPointEntity ReadAndBuildWithoutTutorialEntryPoint(uint id)
        {
            return ReadAndBuildMultiWithoutTutorialEntryPoint(new uint[] { id }).FirstOrDefault();
        }
        public static IEnumerable<GachaEntryPointEntity> ReadAndBuildMultiWithoutTutorialEntryPoint(IEnumerable<uint> ids)
        {
            // チュートリアル用は除く。
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id })).Where(entity => entity.ID != TUTORIAL_GACHA_ENTRY_POINT_ID));
        }
        public static GachaEntryPointEntity ReadAndBuildTutorialEntryPoint()
        {
            return _loadAssociatedEntities(ReadAndBuildMulti((new uint[] { TUTORIAL_GACHA_ENTRY_POINT_ID }).Select(id => new Schema.PrimaryKey { ID = id }))).FirstOrDefault();
        }
        static IEnumerable<GachaEntryPointEntity> _loadAssociatedEntities(IEnumerable<GachaEntryPointEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var gachaMap = GachaEntity.ReadAndBuildMulti(entities.Select(entity => entity.GachaID)).ToDictionary(gacha => gacha.ID);
            return entities.Select(entity =>
            {
                entity._setGacha(gachaMap[entity.GachaID]);
                return entity;
            });
        }


        GachaEntity _gacha;

        void _setGacha(GachaEntity gacha)
        {
            if (gacha.ID != GachaID) throw new InvalidOperationException();
            _gacha = gacha;
        }


        public uint ID => _record.ID;
        public string TextKey => "Master.GachaEntryPoint." + _record.TextKey;
        public uint? GameContentID => _record.GameContentID > 0 ? (uint?)_record.GameContentID : null;
        public uint? OpenedAt => _record.OpenedAt > 0 ? (uint?)_record.OpenedAt : null;
        public uint? ClosedAt => _record.ClosedAt > 0 ? (uint?)_record.ClosedAt : null;
        public uint GachaID => _record.GachaID;
        /// <summary>
        /// 試行あたりの抽選回数。
        /// </summary>
        public byte LotNum => _record.LotNum;
        /// <summary>
        /// 最大抽選可能回数。
        /// </summary>
        public byte? MaxTotalLotNum => _record.MaxTotalLotNum > 0 ? (byte?)_record.MaxTotalLotNum : null;
        /// <summary>
        /// 日時最大無料抽選可能回数。
        /// </summary>
        public byte? MaxDailyFreeLotNum => _record.MaxDailyFreeLotNum > 0 ? (byte?)_record.MaxDailyFreeLotNum : null;
        /// <summary>
        /// 無料抽選後に必要となるクールタイム。
        /// </summary>
        public TimeSpan? FreeLotCoolTime => _record.FreeLotCoolTimeMinute > 0 ? (TimeSpan?)TimeSpan.FromMinutes(_record.FreeLotCoolTimeMinute) : null;
        public PossessionCategory PaymentPossessionCategory => (PossessionCategory)_record.PaymentPossessionCategory;
        public uint PaymentPossessionID => _record.PaymentPossessionID;
        public uint PaymentPossessionNum => _record.PaymentPossessionNum;
        /// <summary>
        /// 保証 Possession カテゴリー。
        /// </summary>
        public PossessionCategory? GuaranteedPossessionCategory => _record.GuaranteedPossessionCategory > 0 ? (PossessionCategory?)_record.GuaranteedPossessionCategory : null;

        /// <summary>
        /// オープン中かどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            if (OpenedAt.HasValue && ContextContainer.GetContext().TimeHandler.UnixTime < OpenedAt.Value) return false;
            if (ClosedAt.HasValue && ClosedAt.Value <= ContextContainer.GetContext().TimeHandler.UnixTime) return false;
            return true;
        }
        /// <summary>
        /// 無料で引けるチャンスがあるかどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsFreeLottable()
        {
            return FreeLotCoolTime.HasValue;
        }

        public List<PossessionParam> GetAllPossessionParams()
        {
            var possessionParams = new List<PossessionParam>();
            possessionParams.Add(GetPaymentPossessionParam());
            possessionParams.AddRange(_gacha.GetPossessionParamCandidates());
            return possessionParams;
        }

        public PossessionParam GetPaymentPossessionParam()
        {
            return new PossessionParam
            {
                Category = PaymentPossessionCategory,
                ID = PaymentPossessionID,
                Num = PaymentPossessionNum,
            };
        }

        public IEnumerable<GachaContentEntity> Lot(byte lotNum, Func<GachaContentEntity, byte, bool> contentFilter)
        {
            return _gacha.Lot(lotNum, contentFilter);
        }

        /// <summary>
        /// 大当たりかどうか判定する。
        /// </summary>
        /// <param name="content">判定対象</param>
        /// <returns></returns>
        public bool IsHit(GachaContentEntity content)
        {
            // 今はキャラクターであれば無条件に当たり。
            return content.GetPossessableParam().Category == PossessionCategory.Character;
        }

        public Core.WebApi.Response.Gacha.GachaEntryPoint ToResponseData(PlayerGachaEntryPointEntity playerGachaEntryPoint, PossessionManager possessionManager)
        {
            return new Core.WebApi.Response.Gacha.GachaEntryPoint
            {
                ID = ID,
                TextKey = TextKey,
                OpenedAt = OpenedAt,
                ClosedAt = ClosedAt,
                LotNum = LotNum,
                MaxTotalLotNum = MaxTotalLotNum,
                MaxDailyFreeLotNum = MaxDailyFreeLotNum,
                FreeLotCoolTime = (uint?)FreeLotCoolTime?.TotalSeconds,
                FreeReopenedAt = IsFreeLottable() && (!MaxDailyFreeLotNum.HasValue || playerGachaEntryPoint.FreeLotNum < MaxDailyFreeLotNum.Value) ? (uint?)playerGachaEntryPoint.FreeReopenedAt : null,
                TodayFreeLotNum = MaxDailyFreeLotNum.HasValue ? (byte?)playerGachaEntryPoint.FreeLotNum : null,
                CanLot = playerGachaEntryPoint.CanLot(this, false) && possessionManager.CanRemove(GetPaymentPossessionParam()),
                CanFreeLot = playerGachaEntryPoint.CanLot(this, true),
                Payment = possessionManager.GetPossessionObject(GetPaymentPossessionParam()).ToResponseData(),
                LotCandidates = possessionManager.GetPossessionObjects(_gacha.GetPossessionParamCandidates()).Select(po => po.ToResponseData()).ToArray(),
            };
        }
    }
}
