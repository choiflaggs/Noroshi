namespace Noroshi.Core.Game.Guild
{
    /// <summary>
    /// ギルドカテゴリー。
    /// </summary>
    public enum GuildCategory
    {
        /// <summary>
        /// 初心者ギルド。
        /// </summary>
        Beginner = 1,
        /// <summary>
        /// オープン通常ギルド。
        /// </summary>
        NormalOpen = 2,
        /// <summary>
        /// クローズ通常ギルド。
        /// </summary>
        NormalClose = 3,
    }
    /// <summary>
    /// ギルド内での役割。
    /// </summary>
    public enum GuildRole
    {
        /// <summary>
        /// リーダー。
        /// </summary>
        Leader = 1,
        /// <summary>
        /// 幹部。
        /// </summary>
        Executive = 2,
    }
    /// <summary>
    /// プレイヤーギルド状態。
    /// </summary>
    public enum PlayerGuildState
    {
        /// <summary>
        /// 通常。
        /// </summary>
        Default = 0,
        /// <summary>
        /// 加入リクエスト中。
        /// </summary>
        Request = 1,
    }
    /// <summary>
    /// 獲得友情ポイントによる格付け。
    /// </summary>
    public enum GuildRank
    {
        S = 11,
        A = 12,
        B = 13,
        C = 14,
        D = 15,
        E = 16,
    }

    public enum GuildMessageCategory
    {
        Normal = 0
    }

}
