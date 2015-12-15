using Noroshi.Core.Game.LoginBonus;

namespace Noroshi.Core.WebApi.Response.LoginBonus
{
    /// <summary>
    /// ログインボーナス。
    /// </summary>
    public class LoginBonus
    {
        /// <summary>
        /// ログインボーナスID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ログインボーナスのTextKey。
        /// </summary>
        public string TextKey { get; set; }
        /// <summary>
        /// ログインボーナスの種類。
        /// </summary>
        public LoginBonusCategory Category { get; set; }
        /// <summary>
        /// ログイン回数。
        /// </summary>
        public byte CurrentNum { get; set; }
        /// <summary>
        /// 報酬。
        /// </summary>
        public LoginBonusReward[] Rewards { get; set; }
    }
}
