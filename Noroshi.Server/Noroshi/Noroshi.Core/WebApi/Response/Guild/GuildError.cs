namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// ギルド関連処理のエラーを扱うクラス。
    /// </summary>
    public class GuildError
    {
        public bool GuildNotFound { get; set; }
        public bool TargetGuildNotFound { get; set; }
        public bool IsNotSameGuild { get; set; }
        /// <summary>
        /// 対象が存在しません。
        /// </summary>
        public bool NoTarget { get; set; }
        /// <summary>
        /// 権限がありません。
        /// </summary>
        public bool NoAuthority { get; set; }
        /// <summary>
        /// 加入できません。
        /// </summary>
        public bool CannotJoin { get; set; }
        /// <summary>
        /// 脱退できません。
        /// </summary>
        public bool CannotDropOut { get; set; }
        /// <summary>
        /// 加入申請できません。
        /// </summary>
        public bool CannotRequest { get; set; }
        /// <summary>
        /// 加入申請を受理できません。
        /// </summary>
        public bool CannotAcceptRequest { get; set; }
        /// <summary>
        /// 加入申請がありません。
        /// </summary>
        public bool NoRequest { get; set; }
        /// <summary>
        /// 加入申請があります。
        /// </summary>
        public bool HasRequest { get; set; }
    }
}
