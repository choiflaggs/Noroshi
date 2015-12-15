using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.LoginBonus
{
    /// <summary>
    /// ログインボーナス報酬。
    /// </summary>
    public class LoginBonusReward
    {
        /// <summary>
        /// 報酬を獲得するために必要なログイン日数。
        /// </summary>
        public byte Threshold { get; set; }
        /// <summary>
        /// 報酬を受け取れるかどうかフラグ。
        /// </summary>
        public bool CanReceiveReward { get; set; }
        /// <summary>
        /// 既に報酬を受け取ったかどうかフラグ。
        /// </summary>
        public bool HasAlreadyReceivedReward { get; set; }
        /// <summary>
        /// VIP報酬を受け取れるかのフラグ
        /// </summary>
        public bool HasVipReward { get; set; }
        /// <summary>
        /// 報酬実体。
        /// </summary>
        public PossessionObject[] PossessionObjects { get; set; }
    }
}
