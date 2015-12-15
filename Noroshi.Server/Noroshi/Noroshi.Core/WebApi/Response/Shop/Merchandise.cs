using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// ショップに並ぶ商品。
    /// </summary>
    public class Merchandise
    {
        /// <summary>
        /// 商品 ID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ディスプレイ ID。購入時の商品識別に利用。
        /// </summary>
        public uint DisplayID { get; set; }
        /// <summary>
        /// 陳列番号。ショップ内の枠を表現する。
        /// </summary>
        public byte DisplayNo { get; set; }
        /// <summary>
        /// 購入した際に獲得できるモノ。
        /// </summary>
        public PossessionObject MerchandisePossessionObject { get; set; }
        /// <summary>
        /// 購入した際に支払うモノ。
        /// </summary>
        public PossessionObject PaymentPossessionObject { get; set; }
        /// <summary>
        /// 既に購入済みかどうか。
        /// </summary>
        public bool HasAlreadyBought { get; set; }
        /// <summary>
        /// キャラクターを保有していないため購入できない召喚石かどうか。
        /// </summary>
        public bool IsClosedSoul { get; set; }
        /// <summary>
        /// 購入できるかどうか。支払いが足りない、もしくは、購入済みだと偽。
        /// </summary>
        public bool CanBuy { get; set; }
    }
}
