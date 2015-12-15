using Noroshi.Core.Game.Guild;
using Noroshi.Core.Game.Player;

namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// ギルド。
    /// </summary>
    public class Guild
    {
        /// <summary>
        /// ギルドID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ギルドカテゴリー。
        /// </summary>
        public GuildCategory Category { get; set; }
        /// <summary>
        /// 国。
        /// </summary>
        public Country Country { get; set; }
        /// <summary>
        /// 必要最低プレイヤーレベル。
        /// </summary>
        public ushort? NecessaryPlayerLevel { get; set; }
        /// <summary>
        /// ギルド名。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ギルド紹介文。
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// ギルドメンバー数。
        /// </summary>
        public uint MemberNum { get; set; }
        /// <summary>
        /// ギルドメンバー最大数。
        /// </summary>
        public uint? MaxMemberNum { get; set; }
        /// <summary>
        /// （前日分の）獲得友情ポイントによる格付け。
        /// </summary>
        public GuildRank GuildRank { get; set; }
        /// <summary>
        /// 最大友情ポイント。
        /// </summary>
        public ushort MaxCooperationPoint { get; set; }
        /// <summary>
        /// 友情ポイント。
        /// </summary>
        public ushort CooperationPoint { get; set; }
    }
}
