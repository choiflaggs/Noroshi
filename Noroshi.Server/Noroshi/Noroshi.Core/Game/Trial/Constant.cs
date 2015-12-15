using System;

namespace Noroshi.Core.Game.Trial
{
    public class Constant
    {
        /// <summary>
        /// 最大バトル数。
        /// </summary>
        public const byte MAX_BATTLE_NUM = 5;
        /// <summary>
        /// バトル後、再度バトルができるようになるまでに必要な時間。
        /// </summary>
        public static readonly TimeSpan BATTLE_COOL_TIME = TimeSpan.FromMinutes(10);
    }
}
