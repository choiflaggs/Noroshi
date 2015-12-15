namespace Noroshi.Core.WebApi.Response.RaidBoss
{
    /// <summary>
    /// レイドボス詳細を取得する際のレスポンス。
    /// </summary>
    public class ShowResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public RaidBossError Error { get; set; }
        /// <summary>
        /// レイドボス。
        /// </summary>
        public RaidBoss RaidBoss { get; set; }
        /// <summary>
        /// 履歴。
        /// </summary>
        public RaidBossLog[] Logs { get; set; }
        /// <summary>
        /// ダメージランキング。
        /// </summary>
        public PlayerGuildRaidBoss[] DamageRanking { get; set; }
    }
}
