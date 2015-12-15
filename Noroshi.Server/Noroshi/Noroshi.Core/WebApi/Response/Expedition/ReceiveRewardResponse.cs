using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Expedition
{
    public class ReceiveRewardResponse
    {
        /// <summary>
        /// 自プレイヤーの冒険データ。
        /// </summary>
        public PlayerExpedition PlayerExpedition { get; set; }
        /// <summary>
        /// 獲得した報酬。
        /// </summary>
        public PossessionObject[] Rewards { get; set; }
        /// <summary>
        /// 獲得したお金。
        /// </summary>
        public uint RewardGold { get; set; }
    }
}
