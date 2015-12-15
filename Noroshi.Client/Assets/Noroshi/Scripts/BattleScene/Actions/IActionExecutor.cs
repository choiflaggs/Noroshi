using System;
using Noroshi.Grid;

namespace Noroshi.BattleScene.Actions
{
    public interface IActionExecutor
    {
        /// キャラクター ID。
        uint CharacterID { get; }
        /// 物理攻撃力。
        uint PhysicalAttack { get; }
        /// 魔法攻撃力。
        uint MagicPower { get; }
        /// 物理クリティカル。
        uint PhysicalCrit { get; }
        /// 魔法クリティカル。
        uint MagicCrit { get; }
        /// 防御貫通。
        uint ArmorPenetration { get; }
        /// 魔法耐性無視。
        uint IgnoreMagicResistance { get; }
        /// 命中。
        byte Accuracy { get; }
        /// 与ダメージ係数。レイドボスバトル時の BP 効果や、特攻に利用。
        float? DamageCoefficient { get; }

        Force Force { get; }
        Force CurrentForce { get; }
        Grid.GridPosition? GetGridPosition();
        Direction GetDirection();

        int SkinLevel { get; }

        void SendActionEvent(ActionEvent actionEvent);

        void HorizontalMove(short horizontalDiff, float duration);
        void HorizontalMove(Func<short> getHorizontalDiff, float duration);
        void GoStraight(float duration);
        void Appear();
        bool TryToChangeDirection();
        bool HasMissDamage();

        int GetShadowNum();
        IShadow BuildShadow(uint shadowCharacterId);

        // TODO IActionExecutorView へ
        IActionExecutorView GetViewAsActionExecutorView();
    }
}