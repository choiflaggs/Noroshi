using Noroshi.Core.WebApi.Response.Possession;
using Noroshi.Core.Game.RaidBoss;

namespace Noroshi.Core.WebApi.Response.RaidBoss
{
    /// <summary>
    /// レイドボス。
    /// </summary>
    public class RaidBoss
    {
        /// <summary>
        /// ギルドレイドボス ID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// レイドボス ID。
        /// </summary>
        public uint RaidBossID { get; set; }
        /// <summary>
        /// 出現日時。
        /// </summary>
        public uint CreatedAt { get; set; }
        /// <summary>
        /// コンボ数。
        /// </summary>
        public uint ComboNum { get; set; }
        /// <summary>
        /// 多言語対応用テキストキー。
        /// </summary>
        public string TextKey { get; set; }
        /// <summary>
        /// レイドボス種別。
        /// </summary>
        public RaidBossGroupType Type { get; set; }
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level { get; set; }
        /// <summary>
        /// 出現から逃走までの時間（秒）
        /// </summary>
        public uint Lifetime { get; set; }
        /// <summary>
        /// 逃走予定日時。
        /// </summary>
        public uint EscapedAt { get; set; }
        /// <summary>
        /// 撃破済みかどうか。
        /// </summary>
        public bool IsDefeated { get; set; }
        /// <summary>
        /// 現 HP。
        /// </summary>
        public uint CurrentHP { get; set; }
        /// <summary>
        /// 最大 HP。
        /// </summary>
        public uint MaxHP { get; set; }
        /// <summary>
        /// （獲得できるかもしれない）発見者報酬。
        /// </summary>
        public PossessionObject[] DiscoveryRewards { get; set; }
        /// <summary>
        /// （獲得できるかもしれない）参加者報酬。
        /// </summary>
        public PossessionObject[] EntryRewards { get; set; }
        /// <summary>
        /// 自プレイヤーが与えたダメージ。
        /// </summary>
        public uint? OwnPlayerDamage { get; set; }
    }
}
