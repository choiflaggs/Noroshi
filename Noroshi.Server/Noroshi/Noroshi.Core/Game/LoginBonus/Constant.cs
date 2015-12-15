using System;
using System.Collections.Generic;

namespace Noroshi.Core.Game.LoginBonus
{
    public class Constant
    {
        /// <summary>
        /// VIP特典が付く場合の報酬増加倍率。
        /// </summary>
        public const byte INCREASE_MAGNIFICATION = 2;
        
        /// <summary>
        /// マンスリーログインボーナスのVIP特典に必要な日別のVipレベル。
        /// </summary>
        public static readonly Dictionary<byte, ushort> MONTHLY_DAY_TO_VIP_LEVEL_MAP = new Dictionary<byte, ushort>
        {
            { 1, 1 },
            { 2, 4 },
            { 3, 4 },
            { 4, 4 },
            { 5, 4 },
            { 6, 4 },
            { 7, 4 },
            { 8, 7 },
            { 9, 7 },
            { 10, 7 },
            { 11, 7 },
            { 12, 7 },
            { 13, 7 },
            { 14, 7 },
            { 15, 10 },
            { 16, 10 },
            { 17, 10 },
            { 18, 10 },
            { 19, 10 },
            { 20, 10 },
            { 21, 10 },
            { 22, 13 },
            { 23, 13 },
            { 24, 13 },
            { 25, 13 },
            { 26, 13 },
            { 27, 13 },
            { 28, 13 },
            { 29, 15 },
            { 30, 15 },
            { 31, 15 },
        };

        /// <summary>
        /// スタートアップログインボーナスのVIP特典に必要な日別のVipレベル。
        /// </summary>
        public static readonly Dictionary<byte, ushort> START_UP_DAY_TO_VIP_LEVEL_MAP = new Dictionary<byte, ushort>
        {
            { 1, 1 },
            { 2, 4 },
            { 3, 4 },
            { 4, 4 },
            { 5, 4 },
            { 6, 7 },
            { 7, 7 },
            { 8, 7 },
            { 9, 7 },
            { 10, 7 },
        };
    }
}
