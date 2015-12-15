using System;

namespace Noroshi.Core.Game.Player
{
    public class Constant
    {
        public const ushort INITIAL_MAX_STAMINA = 60;
        public static readonly TimeSpan STAMINA_RECOVERY_TIME_SPAN = TimeSpan.FromMinutes(6);
        public static readonly TimeSpan ACTION_LEVEL_POINT_RECOVERY_TIME_SPAN = TimeSpan.FromMinutes(5);

        public const byte MAX_BP = 3;
        public const byte MAX_ACTION_LEVEL_POINT = 20;
        public static readonly TimeSpan BP_RECOVERY_TIME_SPAN = TimeSpan.FromMinutes(10);

        public const uint RECOVER_GEM_TO_GOLD_POINT = 10;
        public const uint RECOVER_GEM_TO_ACTION_LEVEL_POINT = 20;
        public const uint RECOVER_GEM_POINT = 100;

        public const uint CONVERT_GEM_TO_GOLD_NUM = 20000;

        public const uint MIN_VIP_LEVEL = 0;
        /// <summary>
        /// 最大ゴールド。
        /// </summary>
        public const uint MAX_GOLD = 999999999;
        /// <summary>
        /// 最大ギルドポイント。
        /// </summary>
        public const uint MAX_GUILD_POINT = 999999999;
        /// <summary>
        /// 最大冒険ポイント。
        /// </summary>
        public const uint MAX_EXPEDITION_POINT = 999999999;
        /// <summary>
        /// 最大ソウルポイント。
        /// </summary>
        public const uint MAX_SOUL_POINT = 999999999;
    }
}
