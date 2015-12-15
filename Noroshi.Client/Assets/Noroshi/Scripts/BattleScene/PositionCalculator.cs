using System;
using System.Collections.Generic;
using UniLinq;
using Noroshi.Game;

namespace Noroshi.BattleScene
{
    public class PositionCalculator
    {
        static Dictionary<byte, byte> NO_TO_LINE_MAP = new Dictionary<byte, byte>
        {
            {5, 5},
            {3, 4},
            {1, 3},
            {2, 2},
            {4, 1},
        };
        static int BOTTOM_SHADOW_LINE_SIZE = Constant.MAX_SHADOW_CHARACTER_NUM_IN_FIELD_PER_FORCE - Constant.MAX_NORMAL_CHARACTER_NUM_IN_FIELD_PER_FORCE - 1;

        public PositionCalculator()
        {
        }

        public ushort GetVerticalSize()
        {
            return (Constant.MAX_NORMAL_CHARACTER_NUM_IN_FIELD_PER_FORCE + Constant.MAX_SHADOW_CHARACTER_NUM_IN_FIELD_PER_FORCE )* 2;
        }

        public ushort GetOwnCharacterVerticalIndex(byte noInSameCharacterPosition, byte characterPositionOffset, byte sameCharacterPositionNum)
        {
            var line = Enumerable.Range(characterPositionOffset + 1, sameCharacterPositionNum).Select(i => NO_TO_LINE_MAP[(byte)i]).OrderBy(i => i).ToArray()[noInSameCharacterPosition - 1];
            return (ushort)((line - 1) * 4 +  BOTTOM_SHADOW_LINE_SIZE + 2 + 1);
        }

        public ushort GetEnemyCharacterVerticalIndex(byte noInSameCharacterPosition, byte characterPositionOffset, byte sameCharacterPositionNum)
        {
            var line = Enumerable.Range(characterPositionOffset + 1, sameCharacterPositionNum).Select(i => NO_TO_LINE_MAP[(byte)i]).OrderBy(i => i).ToArray()[noInSameCharacterPosition - 1];
            return (ushort)((line - 1) * 4 +  BOTTOM_SHADOW_LINE_SIZE + 2);
        }

        public IEnumerable<ushort> GetShadowCharacterVerticalIndexes(Force force)
        {
            var diff = force == Force.Own ? 1 : 0;
            var groupNum = Constant.MAX_NORMAL_CHARACTER_NUM_IN_FIELD_PER_FORCE;
            var indexes = (new List<int>{0}).Select(i => i + diff).ToList();
            indexes.AddRange(Enumerable.Range(1, groupNum).Select(n => (n - 1) * (1 + 1) * 2 + BOTTOM_SHADOW_LINE_SIZE + diff).ToList());
            indexes.AddRange((new List<int>{22,24}).Select(i => i + diff));
            var center = GetVerticalSize() / 2;
            return indexes.Select(i => (ushort)i).OrderBy(i => Math.Abs(i - center)).ThenBy(i => i);
        }
    }
}
