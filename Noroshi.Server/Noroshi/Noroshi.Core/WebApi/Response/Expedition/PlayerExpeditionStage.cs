using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Expedition
{
    public class PlayerExpeditionStage
    {
        /// <summary>
        /// 冒険ステージID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ステップ。
        /// </summary>
        public byte Step { get; set; }
        /// <summary>
        /// 対戦相手プレイヤーの名前。
        /// </summary>
        public string PlayerName { get; set; }
        /// <summary>
        /// 対戦相手プレイヤーのレベル。
        /// </summary>
        public ushort PlayerLevel { get; set; }
        /// <summary>
        /// 対戦相手プレイヤーのデッキ。
        /// </summary>
        public PlayerCharacter[] PlayerCharacters { get; set; }
        /// <summary>
        /// 宝箱で獲得できる報酬（ゴールドとポイントを含む）。
        /// </summary>
        public PossessionObject[] Rewards { get; set; }
    }
}
