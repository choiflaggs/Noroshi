using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.LoginBonus
{
    /// <summary>
    /// 指定プレイヤーに指定ログインボーナス報酬を受け取らせた際のレスポンス。
    /// </summary>
    public class ReceiveRewardResponse
    {
        /// <summary>
        /// VIPレベル。
        /// </summary>
        public ushort VipLevel { get; set; }
        /// /// <summary>
        /// VIP報酬フラグ。
        /// </summary>
        public bool HasVipReward { get; set; }
        /// <summary>
        /// 受け取った報酬。
        /// </summary>
        public PossessionObject[] RewardPossessionObjects { get; set; }
    }
}
