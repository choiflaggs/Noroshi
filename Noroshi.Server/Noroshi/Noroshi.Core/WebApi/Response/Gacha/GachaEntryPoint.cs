using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Gacha
{
    public class GachaEntryPoint
    {
        /// <summary>
        /// ガチャエントリーポイントID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// 多言語対応用テキストキー。
        /// </summary>
        public string TextKey { get; set; }
        /// <summary>
        /// オープン日時。
        /// </summary>
        public uint?OpenedAt { get; set; }
        /// <summary>
        /// クローズ日時。
        /// </summary>
        public uint? ClosedAt { get; set; }
        /// <summary>
        /// 試行あたりの抽選回数。
        /// </summary>
        public byte LotNum { get; set; }
        /// <summary>
        /// 最大抽選可能回数。
        /// </summary>
        public byte? MaxTotalLotNum { get; set; }
        /// <summary>
        /// 日時最大無料抽選可能回数。
        /// </summary>
        public byte? MaxDailyFreeLotNum { get; set; }
        /// <summary>
        /// 無料抽選後に必要となるクールタイム（秒）。
        /// </summary>
        public uint? FreeLotCoolTime { get; set; }
        /// <summary>
        /// 無料抽選オープン日時。
        /// </summary>
        public uint? FreeReopenedAt { get; set; }
        /// <summary>
        /// 本日の無料抽選回数。
        /// </summary>
        public byte? TodayFreeLotNum { get; set; }
        /// <summary>
        /// ガチャれるかどうか。
        /// </summary>
        public bool CanLot { get; set; }
        /// <summary>
        /// 無料でガチャれるかどうか。
        /// </summary>
        public bool CanFreeLot { get; set; }
        /// <summary>
        /// ガチャるのに必要なモノ。
        /// </summary>
        public PossessionObject Payment { get; set; }
        /// <summary>
        /// ガチャったら当たるモノ候補。
        /// </summary>
        public PossessionObject[] LotCandidates { get; set; }
    }
}
