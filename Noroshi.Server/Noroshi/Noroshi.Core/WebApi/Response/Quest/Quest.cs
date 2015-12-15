using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Quest
{
    /// <summary>
    /// クライアントに返される（デイリー）クエスト情報を表現するクラス。
    /// </summary>
    public class Quest
    {
        /// <summary>
        /// （デイリー）クエストID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ゲームコンテンツID。0 の場合はなし。
        /// </summary>
        public uint GameContentID { get; set; }
        /// <summary>
        /// 現在値。これが何を表すかはクエストによって異なる。
        /// </summary>
        public uint Current { get; set; }
        /// <summary>
        /// 閾値。現在値がこれに達すればクエストクリアとなる。
        /// </summary>
        public uint Threshold { get; set; }
        /// <summary>
        /// 報酬を受け取れるかどうかフラグ。
        /// </summary>
        public bool CanReceiveReward { get; set; }
        /// <summary>
        /// 既に報酬を受け取ったかどうかフラグ。
        /// </summary>
        public bool HasAlreadyReceivedReward { get; set; }
        /// <summary>
        /// （デイリー）クエスト名。
        /// </summary>
        public string TextKey { get; set; }
        /// <summary>
        /// （デイリー）クエスト詳細。
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 報酬。複数のこともある。
        /// </summary>
        public PossessionObject[] PossessionObjects { get; set; }
    }
}
