namespace Noroshi.Core.WebApi.Response.Guild
{
    public class GetOwnResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// ギルド情報。
        /// </summary>
        public Guild Guild { get; set; }
        /// <summary>
        /// ギルドメンバー情報。
        /// </summary>
        public GuildMemberPlayerStatus[] GuildMembers { get; set; }
        /// <summary>
        /// 届いている加入リクエスト。
        /// </summary>
        public GuildRequest[] Requests { get; set; }
        /// <summary>
        /// 自プレイヤーが加入リクエストを出しているギルド。
        /// </summary>
        public Guild RequestingGuild { get; set; }
        /// <summary>
        /// 最大挨拶数。
        /// </summary>
        public byte MaxGreetingNum { get; set; }
        /// <summary>
        /// 挨拶数。
        /// </summary>
        public byte GreetingNum { get; set; }
        /// <summary>
        /// 未確認被挨拶数。
        /// </summary>
        public uint UnconfirmedGreetedNum { get; set; }
    }
}
