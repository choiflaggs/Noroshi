using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Action;
using System.Linq;

namespace Noroshi.Core.Game.Character
{
    /// 文字列でキャラクターのタグを管理するクラス。
    public class CharacterTagSet: IActionTagSet
    {
        const char TRUE = '1';
        const int DECA_INDEX = 0;

        static Dictionary<int, CharacterTag> _characterTagMasters = new Dictionary<int, CharacterTag>()
        {
            {1, new CharacterTag(1, null)}, // ジャイアント
            {2, new CharacterTag(2, ActionTargetAttribute.Fire)}, // 火
            {3, new CharacterTag(3, ActionTargetAttribute.Water)}, // 水
            {4, new CharacterTag(4, ActionTargetAttribute.Thunder)}, // 雷
            {5, new CharacterTag(5, ActionTargetAttribute.Earth)}, // 地
            {6, new CharacterTag(6, ActionTargetAttribute.Wind)}, // 風
            {7, new CharacterTag(7, ActionTargetAttribute.Flying)}, // 飛行
            {8, new CharacterTag(8, ActionTargetAttribute.Machine)}, // 機械
            {9, new CharacterTag(9, ActionTargetAttribute.Animal)}, // 獣
            {10, new CharacterTag(10, null)}, // アルナディア
            {11, new CharacterTag(11, null)}, // ガルザニア
            {12, new CharacterTag(12, null)}, // クヴィーク
            {13, new CharacterTag(13, null)}, // ラルザス王国軍
            {14, new CharacterTag(14, null)}, // 銃士隊
            {15, new CharacterTag(15, null)}, // 王室諜報部
            {16, new CharacterTag(16, null)}, // 貴族隊
            {17, new CharacterTag(17, null)}, // 東方軍
            {18, new CharacterTag(18, null)}, // 反乱軍
            {19, new CharacterTag(19, null)}, // 三騎士
            {20, new CharacterTag(20, null)}, // 魔帝
            {21, new CharacterTag(21, null)}, // 古代英雄
            {22, new CharacterTag(22, null)}, // 五大魔竜
            {23, new CharacterTag(23, null)}, // 四魔王
            {24, new CharacterTag(24, null)}, // 魔鱗軍
            {25, new CharacterTag(25, null)}, // 魔獣軍
            {26, new CharacterTag(26, null)}, // 魔屍軍
            {27, new CharacterTag(27, null)}, // 魔義軍
            {28, new CharacterTag(28, null)}, // 親衛隊
            {29, new CharacterTag(29, null)}, // 金牙元老院
            {30, new CharacterTag(30, null)}, // モモルガ族
            {31, new CharacterTag(31, null)}, // トビネミ族
            {32, new CharacterTag(32, null)}, // ハムリン族
            {33, new CharacterTag(33, null)}, // ドブネミ族
            {34, new CharacterTag(34, null)}, // ヤーマラ族
            {35, new CharacterTag(35, null)}, // ビーバー族
            {36, new CharacterTag(36, null)}, // ヴィーカーマシン
        };

        char[] _tags;

        public CharacterTagSet(string tagFlags)
        {
            _tags = tagFlags.ToCharArray();
        }

        public bool IsDeca
        {
            get
            {
                return HasTag(DECA_INDEX);
            }
        }
        public bool HasTag(int index)
        {
            return _getTag(index) == TRUE;
        }
        public bool HasTag(CharacterTag tag)
        {
            return HasTag(tag.ID - 1);
        }
        char _getTag(int index)
        {
            // 文字列ベースなので逆から数える
            return _tags[_tags.Length - 1 - index];
        }

        public List<CharacterTag> GetTags()
        {
            var tags = new List<CharacterTag>();
            for (var i = 0; i < _tags.Length; i++)
            {
                if (HasTag(i)) tags.Add( _characterTagMasters[i + 1]);
            }
            return tags;
        }

        public IEnumerable<ActionTargetAttribute> GetActionTargetAttributes()
        {
            return GetTags().Where(tag => tag.ActionTargetAttribute.HasValue).Select(tag => tag.ActionTargetAttribute.Value);
        }
    }
}
