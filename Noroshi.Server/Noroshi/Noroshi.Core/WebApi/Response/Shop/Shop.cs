using System;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// ショップ。
    /// </summary>
    public class Shop
    {
        /// <summary>
        /// ショップID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ショップ名。
        /// </summary>
        public string TextKey { get; set; }
        /// <summary>
        /// ショップ説明。
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// オープンフラグ。
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 陳列されている商品。
        /// </summary>
        public Merchandise[] Merchandises { get; set; }
        /// <summary>
        /// 次の定期商品入れ替え時刻。
        /// </summary>
        public DateTime? NextMerchandiseScheduledUpdateTime { get; set; }
        /// <summary>
        /// 手動更新できるかフラグ。
        /// </summary>
        public bool CanUpdateMerchandisesManually { get; set; }
        /// <summary>
        /// 本日の商品手動更新数。
        /// </summary>
        public ushort CurrentMerchandiseManualUpdateNum { get; set; }
        /// <summary>
        /// デイリー最大商品手動更新数。
        /// </summary>
        public ushort MaxMerchandiseManualUpdateNum { get; set; }
        /// <summary>
        /// 商品手動更新に必要な対価。
        /// </summary>
        public PossessionObject ManualUpdatePossessionObject { get; set; }
        /// <summary>
        /// ショップとしての対価 Possession。
        /// 実際の対価は各商品毎に設定されているので、主にプレイヤー保有量などの表示用。
        /// </summary>
        public PossessionObject PaymentPossessionObject { get; set; }
    }
}
