using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Gacha
{
    public class LotResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GachaError Error { get; set; }
        /// <summary>
        /// ガチャったエントリーポイント。
        /// </summary>
        public GachaEntryPoint GachaEntryPoint { get; set; }
        /// <summary>
        /// ガチャって当たったモノ。
        /// </summary>
        public PossessionObject[] LotPossessionObjects { get; set; }
    }
}
