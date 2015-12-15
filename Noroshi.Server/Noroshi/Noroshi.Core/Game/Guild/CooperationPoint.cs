using System.Linq;

namespace Noroshi.Core.Game.Guild
{
    /// <summary>
    /// 獲得友情ポイントを扱うクラス。
    /// </summary>
    public class CooperationPoint
    {
        readonly ushort _cooperationPoint;

        public CooperationPoint(ushort cooperationPoint)
        {
            _cooperationPoint = cooperationPoint;
        }

        /// <summary>
        /// 格付け取得。
        /// </summary>
        /// <returns></returns>
        public GuildRank GetGuildRank()
        {
            return Constant.GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP
                .OrderByDescending(kv => kv.Value)
                .First(kv => kv.Value <= _cooperationPoint)
                .Key;
        }
    }
}
