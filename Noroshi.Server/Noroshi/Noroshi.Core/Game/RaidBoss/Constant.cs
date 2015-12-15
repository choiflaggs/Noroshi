using System;
using System.Collections.Generic;

namespace Noroshi.Core.Game.RaidBoss
{
    public class Constant
    {
        /// <summary>
        /// 最大同時出現レイドボス数。
        /// </summary>
        public const byte MAX_ACTIVE_RAID_BOSS_NUM_PER_GUILD = 5;
        /// <summary>
        /// ギルドレイドボスレコードの論理的な生存期間。これを過ぎると参照できなくなる。
        /// </summary>
        public static readonly TimeSpan GUILD_RAID_BOSS_RECORD_LIFE_TIME = TimeSpan.FromDays(3);
        /// <summary>
        /// レイドボス撃破後から報酬を獲得できる期間。
        /// </summary>
        public static readonly TimeSpan REWARD_RECEIVABLE_TIME = TimeSpan.FromHours(24);
        /// <summary>
        /// ダメージランキングにおいて閲覧可能な最大プレイヤー数。
        /// </summary>
        public const ushort MAX_DAMAGE_RANKING_SHOWABLE_PLAYER_NUM = 50;
        /// <summary>
        /// 閲覧可能な最大ギルドレイドボスログ数。
        /// </summary>
        public const ushort MAX_SHOWABLE_GUILD_RAID_BOSS_LOG_NUM = 250;
        /// <summary>
        /// 閲覧可能な最大報酬未獲得レイドボス数。
        /// </summary>
        public const ushort MAX_SHOWABLE_UNRECEIVED_GUILD_RAID_BOSS_NUM = 250;
        /// <summary>
        /// 現レイドボス出現数に応じた出現確率補正係数。
        /// </summary>
        public const float ACTIVE_RAID_BOSS_NUM_APPEARANCE_PROBABILITY_COEFFICIENT = 0.7f;
        /// <summary>
        /// レイドボスバトル最低参加者数毎の獲得友情ポイント。
        /// </summary>
        public static readonly Dictionary<byte, byte> MIN_RAID_BOSS_BATTLE_PLAYER_NUM_TO_COOPERATION_POIN_MAP = new Dictionary<byte, byte>
        {
            {  0, 1 },
            {  6, 2 },
            { 16, 3 },
            { 31, 5 },
        };
        /// <summary>
        /// 使用 BP 毎のバトル内のダメージ係数。
        /// </summary>
        public static readonly Dictionary<byte, float> CONSUMING_BP_TO_DAMAGE_COEFFICIENT_MAP = new Dictionary<byte, float>
        {
            { 0, 1 },
            { 1, 1 },
            { 2, 3 },
            { 3, 5 },
        };
    }
}
