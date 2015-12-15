using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// 自動売却を実行する際のレスポンス。
    /// </summary>
    public class SellAutomaticallyResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public ShopError Error { get; set; }
        /// <summary>
        /// 売却で得たゴールド。
        /// </summary>
        public uint Gold { get; set; }
        /// <summary>
        /// 売却で得たソウルポイント。
        /// </summary>
        public uint SoulPoint { get; set; }
        /// <summary>
        /// 売却物。
        /// </summary>
        public PossessionObject[] SoldObjects { get; set; }
    }
}
