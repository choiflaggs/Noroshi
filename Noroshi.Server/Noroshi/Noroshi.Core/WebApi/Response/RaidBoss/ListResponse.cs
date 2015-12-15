namespace Noroshi.Core.WebApi.Response.RaidBoss
{
    /// <summary>
    /// レイドボス一覧を取得する際のレスポンス。
    /// </summary>
    public class ListResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public RaidBossError Error { get; set; }
        /// <summary>
        /// 所属ギルド。
        /// </summary>
        public Guild.Guild Guild { get; set; }
        /// <summary>
        /// 出現中レイドボス。
        /// </summary>
        public RaidBoss[] ActiveRaidBosses { get; set; }
        /// <summary>
        /// 報酬獲得可能レイドボス。
        /// </summary>
        public RaidBoss[] RewardUnreceivedRaidBosses { get; set; }
    }
}
