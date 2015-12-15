using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 挨拶実行時のレスポンス。
    /// </summary>
    public class GreetResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 最大挨拶数。
        /// </summary>
        public byte MaxGreetingNum { get; set; }
        /// <summary>
        /// 現挨拶数。
        /// </summary>
        public byte CurrentGreetingNum { get; set; }
        /// <summary>
        /// 獲得した挨拶報酬。
        /// </summary>
        public PossessionObject[] GreetingRewards { get; set; }
    }
}
