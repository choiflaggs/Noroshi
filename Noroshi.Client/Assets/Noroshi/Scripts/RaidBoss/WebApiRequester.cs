using UniRx;
using Noroshi.Core.WebApi.Response.RaidBoss;

namespace Noroshi.RaidBoss
{
    public class WebApiRequester
    {
        /// <summary>
        /// 出現中のギルドレイドボス一覧を取得する。
        /// </summary>
        public static IObservable<ListResponse> List()
        {
            return _getWebApiRequester().Request<ListResponse>("RaidBoss/List");
        }
        /// <summary>
        /// ギルドレイドボス詳細情報を取得する。
        /// </summary>
        /// <param name="guildRaidBossId">対象ギルドレイドボス ID</param>
        public static IObservable<ShowResponse> Show(uint guildRaidBossId)
        {
            var request = new GuildRaidBossIDRequest { GuildRaidBossID = guildRaidBossId };
            return _getWebApiRequester().Request<GuildRaidBossIDRequest, ShowResponse>("RaidBoss/Show", request);
        }
        /// <summary>
        /// 報酬を受け取る。
        /// </summary>
        /// <param name="guildRaidBossId">対象ギルドレイドボス ID</param>
        public static IObservable<ReceiveRewardResponse> ReceiveReward(uint guildRaidBossId)
        {
            var request = new GuildRaidBossIDRequest { GuildRaidBossID = guildRaidBossId };
            return _getWebApiRequester().Post<GuildRaidBossIDRequest, ReceiveRewardResponse>("RaidBoss/ReceiveReward", request);
        }
        
        class GuildRaidBossIDRequest
        {
            public uint GuildRaidBossID { get; set; }
        }
        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
