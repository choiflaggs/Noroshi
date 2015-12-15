using System.Collections.Generic;

namespace Noroshi.Core.Game.Character
{
    public class Constant
    {
        /// <summary>
        /// 最大進化レベル。
        /// </summary>
        public const byte MAX_EVOLUTION_LEVEL = 5;

        public const uint FIRST_CHARACTER_ID = 105;
        public const uint SECOND_CHARACTER_ID = 101;
        public const uint THIRD_CHARACTER_ID = 303;
        public const uint FOURTH_CHARACTER_ID = 206;
        public const byte FIRST_GEAR_EQUIP_POSITION = 1;
        public const byte LAST_GEAR_EQUIP_POSITION = 6;
        public const byte FIRST_ACTION_LEVEL_NO = 1;
        public const byte LAST_ACTION_LEVEL_NO = 5;

        public static readonly Dictionary<byte, uint> PROMOTION_RANK_MAP = new Dictionary<byte, uint>()
        {
            {1,  0},
            {2,  2},
            {3,  3},
            {4,  6},
            {5, 11},
        };

    }
}
