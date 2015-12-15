namespace Noroshi.Core.WebApi.Response.Expedition
{
    public class ResetResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public ExpeditionError Error { get; set; }
        /// <summary>
        /// 自プレイヤーの冒険データ。
        /// </summary>
        public PlayerExpedition PlayerExpedition { get; set; }
    }
}
