namespace Noroshi.Core.WebApi.Request.Gacha
{
    public class LotRequest
    {
        /// <summary>
        /// ガチャエントリーポイントID。
        /// </summary>
        public uint GachaEntryPointID { get; set; }
        /// <summary>
        /// 無料試行。
        /// </summary>
        public bool Free { get; set; }
    }
}
