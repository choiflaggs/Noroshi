namespace Noroshi.Core.Game.RaidBoss
{
    /// <summary>
    /// レイドボス種別。
    /// </summary>
    public enum RaidBossGroupType
    {
        /// <summary>
        /// 通常レイドボス。
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 巨大レイドボス。
        /// </summary>
        Special = 2,
    }
    /// <summary>
    /// レイドボス状態。
    /// </summary>
    public enum RaidBossState
    {
        /// <summary>
        /// 生存。ただし、逃走しているかもしれない。
        /// </summary>
        Alive = 0,
        /// <summary>
        /// 撃破。
        /// </summary>
        Defeated = 1,
    }
    /// <summary>
    /// レイドボス報酬カテゴリ。
    /// </summary>
    public enum RaidBossRewardCategory
    {
        /// <summary>
        /// 発見者報酬。
        /// </summary>
        Discovery = 1,
        /// <summary>
        /// 参加報酬。
        /// </summary>
        Entry = 2,
    }
    /// <summary>
    /// プレイヤー、ギルドレイドボス間の状態。
    /// </summary>
    public enum PlayerGuildRaidBossState
    {
        None = 0,
        /// <summary>
        /// 交戦。
        /// </summary>
        Battle = 1,
        /// <summary>
        /// 報酬受け取り済み。
        /// </summary>
        HasReceivedRewards = 2,
    }
}
