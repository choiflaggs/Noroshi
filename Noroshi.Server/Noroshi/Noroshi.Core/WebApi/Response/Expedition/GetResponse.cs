namespace Noroshi.Core.WebApi.Response.Expedition
{
    public class GetResponse
    {
        /// <summary>
        /// 現在の冒険（プレイヤーの選択の余地なし）。
        /// </summary>
        public Expedition CurrentExpedition { get; set; }
        /// <summary>
        /// 自プレイヤーの冒険データ。初回アクセス時も初期データを返すため、常に null にはならない。
        /// </summary>
        public PlayerExpedition PlayerExpedition { get; set; }
    }
}
