using Noroshi.Core.Game.Enums;

namespace Noroshi.Core.Game.Character
{
    /// 文字列でキャラクターのタグを管理するクラス。
    public struct CharacterTag
    {
        const int DECA_INDEX = 0;
        public readonly int ID;
        public readonly ActionTargetAttribute? ActionTargetAttribute;

        public CharacterTag(int id, ActionTargetAttribute? actionTargetAttribute)
        {
            ID = id;
            ActionTargetAttribute = actionTargetAttribute;
        }
    }
}
